using Etl.Core.Extraction;

namespace Etl.Core.Transformation.Actions;

public class ActionArgs
{
    public ExtractedRecord Record { get; }

    public ActionArgs(ExtractedRecord record)
    {
        Record = record;
    }
}
