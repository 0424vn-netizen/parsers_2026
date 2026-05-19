using Etl.Core.Events;
using Etl.Core.Extraction;
using Etl.Core.Load;
using Etl.Core.Scanner;
using Etl.Core.Settings;
using Etl.Core.Transformation;
using Etl.Core.Utils;
using System.Threading.Tasks.Dataflow;

namespace Etl.Core;

public class EtlInstance
{
    private readonly object _lock = new();
    private readonly IEtlEvent? _events;
    private readonly int _batchSize;
    private readonly EtlStatus _status;

    private readonly ActionBlock<ScannedRecord> _extractAndTranformActionBlock;
    private readonly ActionBlock<(TransformResult result, bool isLast)> _loadActionBlock;

    private readonly Scanner.Scanner _scannerExec;
    private readonly Func<ScannedRecord, ExtractedRecord> _extractExec;
    private readonly Func<ExtractedRecord, TransformResult> _transformExec;
    private readonly Func<Records, Records?>? _applyMassageExec;
    private readonly List<ILoaderInst> _loaderInstances;

    private TransformResult _transformResultBuffer = new();

    public EtlInstance(
        IServiceProvider sp,
        EtlSetting etlSetting, Etl etl, IEtlEvent? events,
        string dataFile, StreamReader dataStream,
        IEnumerable<Loader> extraLoaders)
    {
        _events = events;
        _batchSize = etl.BatchSize;
        _status = new(dataFile, () => _loadActionBlock!.InputCount);

        var maxExtractorThread = etlSetting?.Extraction?.MaxThread ?? 2;

        _extractAndTranformActionBlock = new ActionBlock<ScannedRecord>(
            ExtractAndTransform,
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = maxExtractorThread,
                BoundedCapacity = maxExtractorThread
            });

        _loadActionBlock = new ActionBlock<(TransformResult result, bool isLast)>(
            args => Load(args.result, args.isLast),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1,     //Sequence
                BoundedCapacity = etl.BufferSize
            });

        var creator = etl.LazyCreator.Value;
        _scannerExec = creator.CreateScannerExec(dataStream, _extractAndTranformActionBlock.SendAsync);
        _extractExec = creator.CreateExtractExec(events);
        _transformExec = creator.CreateTransformExec(sp);
        _applyMassageExec = creator.CreateMassageFunc();
        _loaderInstances = creator.CreateLoaderInstances(sp, dataFile, extraLoaders);
    }

    public async Task StartAsync(int? take = null, int? skip = null)
    {
        if (_status.StartAt != DateTime.MinValue)
            throw new InvalidOperationException($"{nameof(EtlInstance)} already started!");
        _status.StartAt = DateTime.Now;

        System.Timers.Timer? timer = CreateTimer();

        try
        {
            timer?.Start();
            _transformResultBuffer = new(_batchSize);

            await _scannerExec.StartAsync(take, skip);
            await _extractAndTranformActionBlock.Completion;
            
            await _loadActionBlock.SendAsync((_transformResultBuffer, true));
            await _loadActionBlock.Completion;
        }
        finally
        {
            timer?.Dispose();
            _scannerExec.Dispose();
            _loaderInstances.ForEach(e => e.OnCompleted());

            _status.IsCompleted = true;
            _events?.OnStatusChanged.onStatus?.Invoke(_status);
        }
    }

    private System.Timers.Timer? CreateTimer()
    {
        System.Timers.Timer? timer = null;
        if (_events != null && _events.OnStatusChanged != default)
        {
            timer = new(_events.OnStatusChanged.seconds * 1000);
            timer.Elapsed += (sender, e) =>
            {
                if (_status.HasChanged)
                {
                    _status.HasChanged = false;
                    _events.OnStatusChanged.onStatus(_status);
                }
            };
        }

        return timer;
    }

    private async Task ExtractAndTransform(ScannedRecord scannedRecord)
    {
        _status.ScannerProgress = scannedRecord.Progress;
        if (scannedRecord == Scanner.Scanner.END)
        {
            _extractAndTranformActionBlock.Complete();
            return;
        }

        try
        {
            _events?.OnScanned?.Invoke(scannedRecord.Lines);

            var record = _extractExec(scannedRecord);
            _events?.OnExtracted?.Invoke(record);

            var result = _transformExec(record);
            _events?.OnTransformed?.Invoke(result);

            TransformResult? batch = null;
            lock (_lock)
            {
                _status.TotalRecords += result.TotalRecords;
                _transformResultBuffer.Append(result);
                if (_transformResultBuffer.Batch.Count >= _batchSize)
                {
                    batch = _transformResultBuffer;
                    _transformResultBuffer = new TransformResult(_batchSize);
                }
            }

            if (batch != null)
                await _loadActionBlock.SendAsync((batch, false));
        }
        catch (Exception ex)
        {
            _events?.OnError?.Invoke((ex.InnerException ?? ex).Message, ex);
        }
    }

    private async Task Load(TransformResult result, bool isLast)
    {
        var batch = _applyMassageExec?.Invoke(result.Batch) ?? result.Batch;
        var newResult = new BatchResult
        (
            startAt: _status.StartAt,
            totalTransformSuccess: _status.TotalTransformSuccess += batch.Count,
            totalTransformErrors: _status.TotalTransformErrors += result.TotalErrors,

            batch: batch,
            errors: result.Errors,
            isLast: isLast
        );

        _events?.OnTransformedBatch?.Invoke(newResult);

        try
        {
            var tasks = _loaderInstances.Select(e => e.ProcessBatchAsync(newResult));
            await Task.WhenAll(tasks);
            _status.TotalLoadSuccess += batch.Count;
        }
        catch (Exception ex)
        {
            _status.TotalLoadErrors += batch.Count;
            _events?.OnError?.Invoke("Load Fail", ex);
        }

        if (isLast)
            _loadActionBlock.Complete();
    }
}
