using Etl.Core.Load;
using System.Xml.Serialization;

namespace Etl.Storage;

public class CsvLoader : FileLoader<CsvLoader, CsvLoaderInst>
{
    [XmlAttribute]
    public string Delimiter { get; set; } = "|";

    public List<string> Fields { get; set; } = new();
}

public class CsvLoaderInst : FileLoaderInst<CsvLoader, CsvLoaderInst>
{
    private Dictionary<string, int>? _fieldOrders;
    private string? _delimiter;

    protected override void Initalize(CsvLoader defintion, LoaderArgs args)
    {
        base.Initalize(defintion, args);

        HashSet<string> selectedFields = defintion.Fields.Count == 0
            ? new(args.Fields.Select(e => e.Alias ?? e.DataField))
            : new(defintion.Fields);

        _delimiter = defintion.Delimiter;
        Writer?.WriteLine(string.Join('|', selectedFields));

        _fieldOrders ??= new Dictionary<string, int>();

        foreach (var field in selectedFields)
            _fieldOrders.Add(field, _fieldOrders.Count);
    }

    public override async Task ProcessBatchAsync(BatchResult batch)
    {
        foreach(var e in batch.Batch)
            await OnSaveRecord(e);
    }

    protected virtual Task OnSaveRecord(IDictionary<string, object?> record)
    {
        var eles = new object?[_fieldOrders!.Count];
        foreach (var e in record)
        {
            if (_fieldOrders.TryGetValue(e.Key, out int order))
                eles[order] = e.Value;
        }

        var text = string.Join(_delimiter, eles);
        return Writer?.WriteLineAsync(text) ?? Task.CompletedTask;
    }
}
