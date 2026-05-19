using Etl.Core.Transformation.Fields;

namespace Etl.Core.Load;

//IMPORTANT: ILoaderArgs is singleton, it is loaded from xml so it must be IMMUTABLE.

public record LoaderArgs(string InputFile, IReadOnlyCollection<TransformField> Fields);
