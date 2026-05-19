using Etl.Core.Events;
using Etl.Core.Extraction;
using Etl.Core.Load;
using Etl.Core.Scanner;
using Etl.Core.Transformation;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;

namespace Etl.Core;

internal class EtlCache
{
    private readonly Etl _etl;
    private readonly ExtractorCache _extractor;
    private readonly TransformerCache _transformer;
    public EtlCache(Etl etl)
    {
        _etl = etl;
        _extractor = new ExtractorCache(etl.Extraction);
        _transformer = new TransformerCache(etl.Transformation, etl.Extraction.Layout);
    }

    public Scanner.Scanner CreateScannerExec(StreamReader dataStream, Func<ScannedRecord, Task> onFlush)
    {
        var definition = _etl.Extraction;
        var startLayout = (string.IsNullOrWhiteSpace(definition.LayoutStart) ? null : new Regex(definition.LayoutStart, RegexOptions.Compiled), definition.LayoutStartOffset);
        var startRecord = (string.IsNullOrWhiteSpace(definition.Layout.Start) ? null : new Regex(definition.Layout.Start, RegexOptions.Compiled), definition.Layout.StartOffset);
        var endRecord = (string.IsNullOrWhiteSpace(definition.Layout.End) ? null : new Regex(definition.Layout.End, RegexOptions.Compiled), definition.Layout.EndOffset);
        return new(dataStream, startLayout, startRecord, endRecord, onFlush);
    }

    public Func<ScannedRecord, ExtractedRecord> CreateExtractExec(IEtlEvent? events)
        => rawTextRecord => _extractor.Execute(rawTextRecord.Lines, events);

    public Func<ExtractedRecord, TransformResult> CreateTransformExec(IServiceProvider sp)
        => _transformer.CreateTransformFunc(sp);

    public Func<Records, Records?>? CreateMassageFunc()
       => _transformer.CreateMassageFunc();

    public List<ILoaderInst> CreateLoaderInstances(
        IServiceProvider sp,
        string dataFile,
        IEnumerable<Loader> extraLoaders)
    {
        var mergedLoaders = new List<Loader>(_etl.Loaders);
        if (extraLoaders != null)
            mergedLoaders.AddRange(extraLoaders);

        var loaderArgs = new LoaderArgs(dataFile, _transformer.AllFields);
        return mergedLoaders.Select(e =>
        {
            var inst = (ILoaderInst)sp.GetRequiredService(e.InstanceType);
            inst.Initalize(e, loaderArgs);
            return inst;
        }).ToList();
    }
}
