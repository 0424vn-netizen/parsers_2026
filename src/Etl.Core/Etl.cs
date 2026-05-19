using Etl.Core.Extraction;
using Etl.Core.Load;
using Etl.Core.Transformation;
using System.Xml.Serialization;

namespace Etl.Core;

public class Etl
{
    [XmlAttribute]
    public int BatchSize { get; set; } = 10 * 1000;

    [XmlAttribute]
    public int BufferSize { get; set; } = 2;

    public Extractor Extraction { get; set; } = new();

    public Transformer Transformation { get; set; } = new();

    public List<Loader> Loaders { get; set; } = new();


    internal readonly Lazy<EtlCache> LazyCreator;
    public Etl()
    {
        LazyCreator = new Lazy<EtlCache>(() => new(this));
    }
}
