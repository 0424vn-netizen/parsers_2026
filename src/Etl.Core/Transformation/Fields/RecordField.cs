using Etl.Core.Extraction;
using Microsoft.Extensions.DependencyInjection;

namespace Etl.Core.Transformation.Fields;

public class RecordField : TransformField
{
    public List<TransformField> Fields { get; set; } = new();

    public HashSet<string> IgnoreFields { get; set; } = new();

    protected internal override Type InstanceType => typeof(RecordFieldInst);
}

public class RecordFieldInst : ITransformFieldInst
{
    private readonly List<ITransformFieldInst> _fields = new();
    private ArrayFieldInst? _flatArray;

    public string Alias { get; private set; } = string.Empty;
    public string DataField { get; private set; } = string.Empty;
    public bool Required { get; private set; }

    void IInitialization.Initialize(object args, IServiceProvider sp)
        => Initialize((RecordField)args, sp);

    public void Initialize(RecordField recordField, IServiceProvider sp)
    {
        Alias = recordField.Alias;
        DataField = recordField.DataField;
        Required = recordField.Required;

        foreach (var e in recordField.Fields)
        {
            var item = (ITransformFieldInst)sp.GetRequiredService(e.InstanceType);

            if (e is ArrayField array && array.Flat)
            {
                if (_flatArray != null)
                    throw new InvalidOperationException($"Not except multiple flat {nameof(ArrayField)} in the same hierarchy.");

                _flatArray = (ArrayFieldInst)item;
            }
            else
                _fields.Add(item);

            item.Initialize(e, sp);
        }
    }

    object? ITransformFieldInst.Transform(ExtractedRecord record)
        => Transform(record);
    public virtual TransformResult Transform(ExtractedRecord record)
    {
        IRecord? newRecord = null;
        var result = _flatArray?.Transform(record) ?? new TransformResult();
        if (result.Batch.Count == 0)
            result.Batch.Add(newRecord = new Record());

        try
        {
            foreach (var field in _fields)
            {
                var val = field.Transform(record);
                if (val == null)
                    continue;

                if (newRecord != null)
                    newRecord[field.Alias] = val;
                else
                    result.Batch.ForEach(e => e[field.Alias] = val);
            }
        }
        catch (Exception ex)
        {
            return new TransformResult { TotalErrors = Math.Max(result.TotalRecords, 1) }
                .AddErorr(ex.Message);
        }

        return result;
    }


}
