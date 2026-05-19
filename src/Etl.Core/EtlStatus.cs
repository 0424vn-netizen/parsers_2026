namespace Etl.Core;

public interface IEtlStatus
{
    string FilePath { get; }
    DateTime StartAt { get; }
    float ScannerProgress { get; }

    int TotalRecords { get; }
    int TotalTransformSuccess { get; }
    int TotalTransformErrors { get; }

    int BatchBuffer { get; }

    int TotalLoadSuccess { get; }
    int TotalLoadErrors { get; }

    bool IsCompleted { get; }
}

class EtlStatus : IEtlStatus
{
    internal bool HasChanged;

    public string FilePath { get; }

    private DateTime _startAt;
    public DateTime StartAt
    {
        get => _startAt;
        set { _startAt = value; HasChanged = true; }
    }

    private float _scannerProgress;
    public float ScannerProgress
    {
        get => _scannerProgress;
        set { _scannerProgress = value; HasChanged = true; }
    }

    private int _totalTransformSuccess;
    public int TotalTransformSuccess
    {
        get => _totalTransformSuccess;
        set { _totalTransformSuccess = value; HasChanged = true; }
    }

    private int _totalTransformErrors;
    public int TotalTransformErrors
    {
        get => _totalTransformErrors;
        set { _totalTransformErrors = value; HasChanged = true; }
    }
    public int TotalRecords { get; set; }

    private Func<int> _batchBuffer;
    public int BatchBuffer
        => _batchBuffer();

    public int TotalLoadSuccess { get; set; }

    private int _totalLoadErrors;
    public int TotalLoadErrors
    {
        get => _totalLoadErrors;
        set { _totalLoadErrors = value; HasChanged = true; }
    }

    private bool _isCompleted;
    public bool IsCompleted
    {
        get => _isCompleted;
        set { _isCompleted = value; HasChanged = true; }
    }
    public EtlStatus(string filePath, Func<int> batchBuffer)
    {
        FilePath = filePath;
        StartAt = DateTime.MinValue;
        _batchBuffer = batchBuffer;
    }

    public override string ToString()
        => $"{DateTime.Now.Subtract(StartAt)}" +
            $", Scan: {ScannerProgress}%" +
            $", Extract: {TotalRecords}" +
            $", Transform: ({TotalTransformSuccess} ok, {TotalTransformErrors} fail)" +
            $", Buffer: {BatchBuffer}" +
            $", Load: ({TotalLoadSuccess} ok, {TotalLoadErrors} fail)";
}
