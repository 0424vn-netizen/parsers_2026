using Etl.Core.Extraction;
using System.Xml.Serialization;

namespace Etl.Core.Transformation.Fields;

public class ArrayField : RecordField
{
    [XmlAttribute]
    public bool Flat { get; set; }

    protected internal override Type InstanceType => typeof(ArrayFieldInst);
}

public class ArrayFieldInst : RecordFieldInst
{
    public override TransformResult Transform(ExtractedRecord record)
    {
        var result = new TransformResult();

        if (!record.ContainsKey(DataField))
            return result;

        if (record[DataField] is not ExtractedArray nestedRecords)
            throw new InvalidOperationException($"Expected {typeof(ExtractedArray).Name} instead of {record[DataField]?.GetType().Name}");

        foreach (var nextRecord in nestedRecords)
        {
            var oneResult = base.Transform(nextRecord);
            result.Batch.AddRange(oneResult.Batch);
            result.TotalErrors += oneResult.TotalErrors;
            result.AddErorrs(oneResult.Errors);
        }

        return result;
    }
}
