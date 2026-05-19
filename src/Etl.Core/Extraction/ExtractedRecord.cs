using Etl.Core.Scanner;
using System.Runtime.Serialization;

namespace Etl.Core.Extraction;

public interface IExtractedInfo
{
    (int row, int col) From { get; }
    (int row, int col) To { get; }
}

class ExtractedInfo : IExtractedInfo
{
    public (int row, int col) From { get; private set; }
    public (int row, int col) To { get; private set; }

    public ExtractedInfo((int row, int col) from, (int row, int col) to)
    {
        From = from;
        To = to;
    }
}

[Serializable]
public class ExtractedRecord : Dictionary<string, IExtractedInfo>, IExtractedInfo
{
    public readonly TextBlock Block;
    public (int row, int col) From { get; }
    public (int row, int col) To { get; }

    public ExtractedRecord(TextBlock block, (int row, int col) from, (int row, int col) to)
    {
        Block = block;
        From = from;
        To = to;
    }

    protected ExtractedRecord(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Block = new(new List<TextLine>(0));
    }
}

class ExtractedArray : List<ExtractedRecord>, IExtractedInfo
{
    public (int row, int col) From { get; }
    public (int row, int col) To { get; }

    public ExtractedArray((int row, int col) from, (int row, int col) to)
    {
        From = from;
        To = to;
    }
}
