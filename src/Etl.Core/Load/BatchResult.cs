namespace Etl.Core.Load;

public class BatchResult
{
    public readonly ReadOnlyRecords Batch;
    public readonly IEnumerable<string>? Errors;
    public readonly DateTime StartAt;
    public readonly int TotalTransformSuccess;
    public readonly int TotalTransformErrors;
    public readonly bool IsLast;

    public BatchResult(Records batch, IEnumerable<string>? errors, DateTime startAt, int totalTransformSuccess, int totalTransformErrors, bool isLast)
    {
        Batch = batch;
        Errors = errors;
        StartAt = startAt;
        TotalTransformSuccess = totalTransformSuccess;
        TotalTransformErrors = totalTransformErrors;
        IsLast = isLast;
    }

    public int TotalRecords => TotalTransformSuccess + TotalTransformErrors;

    public override string ToString()
    {
        var t = DateTime.Now.Subtract(StartAt);

        var sb = new StringBuilder($"total: {TotalRecords}, valid: {TotalTransformSuccess}, errors: {TotalTransformErrors}, spend: {t}, batch {Batch.Count}");

        if (Errors != null)
        {
            sb.AppendLine();
            foreach (var e in Errors)
                sb.AppendLine(e);
        }

        return sb.ToString();
    }
}
