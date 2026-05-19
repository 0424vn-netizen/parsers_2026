using Etl.Core.Load;
using Etl.Core.Transformation.Fields;

namespace Etl.Core.Transformation;

public class Transformer
{
    public List<TransformField> Fields { get; set; } = new();

    public MassageDataCSharpCode Massage { get; set; } = new();
}
