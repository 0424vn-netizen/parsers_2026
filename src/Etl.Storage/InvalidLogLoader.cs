using Etl.Core.Load;
using System.Xml.Serialization;

namespace Etl.Storage;

public class InvalidLogLoader : FileLoader<InvalidLogLoader, InvalidLogLoaderInst>
{
    [XmlAttribute]
    public override string OutPath { get; set; } = "$path/$name.invalid";
}

public class InvalidLogLoaderInst : FileLoaderInst<InvalidLogLoader, InvalidLogLoaderInst>
{
    public override async Task ProcessBatchAsync(BatchResult batch)
    {
        if (Writer != null && batch.Errors != null)
            foreach (var error in batch.Errors)
                await Writer.WriteLineAsync(error);
    }
}
