using Etl.Core.Extraction;
using Etl.Core.Load;
using Etl.Core.Transformation.Fields;
using Etl.Core.Utils;
using System.Reflection;

namespace Etl.Core.Transformation;

public class TransformerCache
{
    private const string NAMESPACE = "tempNameSpace";
    private const string CLASS = "tempClass";
    private const string METHOD = "Execute";
    private readonly Assembly? _massageAssembly;
    private readonly RecordField _rootField;

    public IReadOnlyCollection<TransformField> AllFields { get; }

    public TransformerCache(Transformer transformDef, Layout layout)
    {
        _massageAssembly = CompileCSharpCode(transformDef.Massage);
        _rootField = new RecordField { Fields = new(transformDef.Fields) };

        AllFields = MergeAllFields(_rootField, layout);
    }

    public Func<ExtractedRecord, TransformResult> CreateTransformFunc(IServiceProvider sp)
    {
        var rootFieldInst = new RecordFieldInst();
        rootFieldInst.Initialize(_rootField, sp);

        return record => rootFieldInst.Transform(record);
    }

    public Func<Records, Records>? CreateMassageFunc()
    {
        Func<Records, Records>? applyMassage = default;
        if (_massageAssembly != null)
        {
            var instance = _massageAssembly.CreateInstance($"{NAMESPACE}.{CLASS}");
            var method = instance?.GetType().GetMethod(METHOD);
            applyMassage = batch => method?.Invoke(instance, new object[] { batch }) as Records ?? batch;
        }

        return applyMassage;
    }

    private static List<TransformField> MergeAllFields(RecordField rootField, Layout layout)
    {
        List<TransformField> extractedFields = new();
        CopyExtractedFields(layout, extractedFields);

        if (rootField.Fields.Count == 0)
        {
            rootField.Fields.AddRange(extractedFields);
            return new(rootField.Fields);
        }

        return MergeFields(rootField.Fields, extractedFields, rootField.IgnoreFields);
    }

    private static void CopyExtractedFields(Layout layout, List<TransformField> destination)
    {
        if (layout == null)
            return;

        if (!string.IsNullOrWhiteSpace(layout.DataField))
        {
            if (layout.Repeat)
            {
                ArrayField arr = new() { Alias = layout.DataField };
                destination.Add(arr);
                destination = arr.Fields;
            }
            else
                destination.Add(new StringField { Alias = layout.DataField });
        }

        if (layout.Children != null && layout.Children.Count > 0)
            layout.Children.ForEach(e => CopyExtractedFields(e, destination));
    }

    private static List<TransformField> MergeFields(List<TransformField> allTransformFields, List<TransformField> allExtractedFields, HashSet<string> ignoreParserFields)
    {
        var items = new List<TransformField>(allTransformFields.Where(e => e is not ArrayField));

        var dictionary = new Dictionary<string, TransformField>();
        foreach (var e in allTransformFields)
            if (string.IsNullOrEmpty(e.Alias))
                throw new InvalidOperationException($"{nameof(TransformField)} expects {nameof(TransformField.Alias)} or {nameof(TransformField.DataField)}");
            else
                dictionary[e.Alias] = e;

        foreach (var extractedField in allExtractedFields.Where(x => !ignoreParserFields.Contains(x.DataField)))
        {
            if (!dictionary.TryGetValue(extractedField.Alias, out TransformField? transformField))
            {
                allTransformFields.Add(extractedField);
                items.Add(extractedField);
            }
            else if (extractedField is ArrayField extractedArray)
            {
                if (transformField is not ArrayField transfromArray)
                    throw new InvalidOperationException($"Extract array field {extractedArray.DataField} does not match defined field {transformField.DataField} {transformField.GetType().Name}");

                var nestedItems = MergeFields(transfromArray.Fields, extractedArray.Fields, transfromArray.IgnoreFields);

                if (transfromArray.Flat)
                    items.AddRange(nestedItems);
                else
                    items.Add(transfromArray);
            }
            else
                items.Add(transformField);
        }

        return items;
    }

    private static Assembly? CompileCSharpCode(MassageDataCSharpCode? transform)
    {
        if (transform == null || string.IsNullOrWhiteSpace(transform.Code))
            return null;

        var sb = new StringBuilder($"namespace {NAMESPACE} {{ class {CLASS} {{");
        if (!string.IsNullOrWhiteSpace(transform.GlobalVariables))
            sb.Append($"public {transform.GlobalVariables}");
        sb.Append($"public List<IDictionary<string, object>> {METHOD}(List<IDictionary<string, object>> B) {{ {transform.Code};");
        sb.Append("}}}");

        return CShapCompiler.Compile(sb.ToString());
    }
}
