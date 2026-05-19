namespace Etl.Core.Transformation;

public class TransformResult
{
    public int TotalErrors { get; set; }

    private List<string>? _errorMessages;
    public IEnumerable<string>? Errors => _errorMessages;

    public Records Batch { get; }

    public int TotalRecords => TotalErrors + Batch.Count;

    public TransformResult(int? size = null)
    {
        Batch = size == null ? new() : new(size.Value);
    }

    public TransformResult AddErorr(string message)
    {
        if (_errorMessages == null)
            _errorMessages = new List<string>();

        _errorMessages.Add(message);
        return this;
    }

    public TransformResult AddErorrs(IEnumerable<string>? messages)
    {
        if (messages != null && messages.Any())
        {
            if (_errorMessages == null)
                _errorMessages = new List<string>();

            _errorMessages.AddRange(messages);
        }
        return this;
    }

    public void Append(TransformResult result)
    {
        if (result == null)
            return;

        if (result.Batch != null)
            Batch.AddRange(result.Batch);

        TotalErrors += result.TotalErrors;

        AddErorrs(result.Errors);
    }
}
