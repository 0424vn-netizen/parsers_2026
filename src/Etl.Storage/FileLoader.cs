using Etl.Core.Load;
using Etl.Core.Utils;
using System.Globalization;
using System.Xml.Serialization;

namespace Etl.Storage;

public abstract class FileLoader<TDef, TInst> : Loader<TInst>
    where TDef : FileLoader<TDef, TInst>
    where TInst : FileLoaderInst<TDef, TInst>
{
    [XmlAttribute]
    public virtual string OutPath { get; set; } = "$path/$name.result";
}

public abstract class FileLoaderInst<TDef, TInst> : LoaderInst<TDef>
    where TDef : FileLoader<TDef, TInst>
    where TInst : FileLoaderInst<TDef, TInst>

{
    protected StreamWriter? Writer;

    protected override void Initalize(TDef definition, LoaderArgs args)
    {
        var outPathLatestCode = string.Concat(definition.OutPath);
        var file = new FileInfo(FilePath.GetFullPath(args.InputFile));
        var path = outPathLatestCode.Replace("$path", file.DirectoryName)
            .Replace("$name", file.Name);

        Writer = new(path);
    }

    protected override void OnCompleted()
        => Writer?.Dispose();

    protected virtual string GetStringDateTimeValue(string value)
    {
        DateTime dateTime = new();
        if (!DateTime.TryParseExact(value, new string[] { "yyyy/MM/dd", "MM/dd/yyyy" }, CultureInfo.InvariantCulture, 0, out dateTime))
        {
            return "";
        }
        return dateTime.ToString("yyyy/MM/dd");
    }

    protected virtual string GetValueInDictionary(string? key, IDictionary<string, object>? batch)
    {
        if (!string.IsNullOrEmpty(key) && batch != null && batch.ContainsKey(key))
        {
            object item = batch[key];
            return item == null ? string.Empty : item.ToString();
        }
        return string.Empty;
    }
}
