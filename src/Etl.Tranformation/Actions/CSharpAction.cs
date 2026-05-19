using Etl.Core.Extraction;
using Etl.Core.Transformation.Actions;
using Etl.Core.Utils;
using System.Text;

namespace Etl.Tranformation.Actions;

public class CSharpAction : TransformAction<CSharpActionInst>
{
    private readonly object _lock = new();
    public string Code { get; set; } = String.Empty;
    public readonly Lazy<Func<string?, ExtractedRecord, object?>> Executor;

    public CSharpAction()
    {
        Executor = new Lazy<Func<string?, ExtractedRecord, object?>>(() =>
        {
            lock (_lock)
            {
                var namespaceName = "__AutoCodeGen__";
                var className = nameof(CSharpAction);
                var methodName = "AutoCodeGen";
                var sb = new StringBuilder($"namespace {namespaceName} {{ class {className} {{");
                sb.Append($"public static object {methodName}(string V, {nameof(ExtractedRecord)} R) {{ {Code};");
                sb.Append("}}}");

                var (method, instance) = CShapCompiler.BuildExecutor(sb.ToString(), namespaceName, className, methodName);
                return (value, record) => method.Invoke(instance, new object?[] { value, record });
            }
        });
    }
}

public class CSharpActionInst : TransformActionInst<CSharpAction, object?>
{
    private Func<string?, ExtractedRecord, object?>? _executor;

    protected override void Initialize(CSharpAction definition, IServiceProvider sp)
        => _executor = definition.Executor.Value;

    protected override object? Execute(object? input, ActionArgs args)
        => _executor?.Invoke(input as string , args.Record);


}
