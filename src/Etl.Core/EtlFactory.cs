using Etl.Core.Load;
using Etl.Core.Settings;
using Etl.Core.Transformation;
using Etl.Core.Transformation.Fields;
using Etl.Core.Utils;
using Microsoft.Extensions.Configuration;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Etl.Core;

public interface IEtlFactory
{
    (List<Loader> loaders, string configFilePath) GetLoadersAndConfigPath(string dataFilePath);

    Etl GetByConfigFile(string configFilePath);

    Etl GetByConfigStream(Stream xmlLayout);
}

public class EtlFactory : IEtlFactory
{
    private readonly Dictionary<string, Etl> _caches = new();
    private readonly ConfigFilesSetting _setting;
    private readonly List<Type> _loaderDefs;
    private readonly List<(Regex matcher, ConfigFileItemSetting)> _matchers = new();
    private readonly XmlAttributeOverrides _attributeOverrides = new();

    public EtlFactory(EtlSetting setting, List<Type> fieldDefs, List<Type> actionDefs, List<Type> loaderDefs)
    {
        _loaderDefs = loaderDefs;
        _setting = setting?.ConfigFiles ?? new ConfigFilesSetting();

        foreach (var e in _setting.Categories.Values)
            _matchers.Add((new Regex(e.Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled), e));

        AddFieldDefsOverrides(_attributeOverrides, fieldDefs);
        AddActionDefsOverrides(_attributeOverrides, actionDefs);
        AddLoaderDefsOverrides(_attributeOverrides, loaderDefs);
    }

    public void Save(Etl config, string filePath)
    {
        var serializer = new XmlSerializer(typeof(Etl), _attributeOverrides);
        filePath = FilePath.GetFullPath(filePath);
        using var stream = new StreamWriter(filePath);

        serializer.Serialize(stream, config);
    }

    public (List<Loader> loaders, string configFilePath) GetLoadersAndConfigPath(string dataFilePath)
    {
        List<Loader> loaderDefs = new();

        foreach (var (matcher, item) in _matchers)
        {
            if (matcher.IsMatch(dataFilePath))
            {
                if (item.Loaders != null)
                    foreach (var section in item.Loaders)
                    {
                        var instanceOf = section["_InstanceOf"];
                        var type = _loaderDefs.FirstOrDefault(x =>
                            x.Name.Equals(instanceOf, StringComparison.OrdinalIgnoreCase)
                            || (x.FullName != null && x.FullName.Equals(instanceOf, StringComparison.OrdinalIgnoreCase)));

                        if (type != null)
                        {
                            var loader = Activator.CreateInstance(type) as Loader
                                ?? throw new InvalidOperationException($"Cannot create instance of type '{type.FullName}'");
                            section.Bind(loader);
                            loaderDefs.Add(loader);
                        }
                    }

                return (loaderDefs, Path.Combine(_setting.Location, item.File));
            }
        }

        dataFilePath = FilePath.GetFullPath(dataFilePath);
        var fileInfo = new FileInfo(dataFilePath);
        var configPath = $"{fileInfo.Directory}/{Path.GetFileNameWithoutExtension(fileInfo.Name)}.xml";

        return (loaderDefs, configPath);
    }

    public Etl GetByConfigFile(string configFilePath)
    {
        if (!_caches.TryGetValue(configFilePath, out var cache))
            lock (_caches)
            {
                if (!_caches.TryGetValue(configFilePath, out cache))
                {
                    var serializer = new XmlSerializer(typeof(Etl), _attributeOverrides);
                    ////serializer.UnknownNode += (sender, e) => Console.WriteLine("Unknown Node:" + e.Name + "\t" + e.Text);
                    ////serializer.UnknownAttribute += (sender, e) => Console.WriteLine("Unknown Attribute " + e.Attr.Name + "='" + e.Attr.Value + "'");
                    ////serializer.UnknownElement += (sender, e) => Console.WriteLine("Unknown Element:" + e.Element, e);
                    ////serializer.UnreferencedObject += (sender, e) => Console.WriteLine("Unknown UnreferencedObject " + e.ToString());

                    if (!File.Exists(configFilePath))
                        throw new FileNotFoundException($"Not found config file '{configFilePath}'");

                    using var stream = new FileStream(configFilePath, FileMode.Open);
                    var config = serializer.Deserialize(stream) as Etl
                        ?? throw new InvalidOperationException($"cannot deserialize Etl from file '{configFilePath}'");
                    _caches[configFilePath] = config;

                    return config;
                }
            }

        return cache;
    }

    public Etl GetByConfigStream(Stream xmlLayout)
    {
        var serializer = new XmlSerializer(typeof(Etl), _attributeOverrides);

        if (xmlLayout == null)
            throw new ArgumentNullException(nameof(xmlLayout), $"Not found config from stream '{nameof(xmlLayout)}'");


        var config = serializer.Deserialize(xmlLayout) as Etl
            ?? throw new InvalidOperationException($"cannot deserialize Etl from stream '{nameof(xmlLayout)}'");

        return config;
    }

    private static void AddLoaderDefsOverrides(XmlAttributeOverrides attrOverrides, List<Type> loaderDefs)
    {
        var attrs = new XmlAttributes();
        foreach (var e in loaderDefs)
            attrs.XmlArrayItems.Add(new XmlArrayItemAttribute
            {
                ElementName = e.Name.Replace("Loader", ""),
                Type = e
            });
        attrOverrides.Add(typeof(Etl), nameof(Etl.Loaders), attrs);
    }

    private static void AddFieldDefsOverrides(XmlAttributeOverrides attrOverrides, List<Type> fieldDefs)
    {
        var attrs = new XmlAttributes();
        foreach (var e in fieldDefs)
            attrs.XmlArrayItems.Add(new XmlArrayItemAttribute
            {
                ElementName = e.Name.Replace("Field", ""),
                Type = e
            });
        attrOverrides.Add(typeof(Transformer), nameof(Transformer.Fields), attrs);
        attrOverrides.Add(typeof(RecordField), nameof(RecordField.Fields), attrs);
    }

    private static void AddActionDefsOverrides(XmlAttributeOverrides attrOverrides, List<Type> actionDefs)
    {
        var attrs = new XmlAttributes();
        foreach (var e in actionDefs)
            attrs.XmlArrayItems.Add(new XmlArrayItemAttribute
            {
                ElementName = e.Name.Replace("Action", ""),
                Type = e
            });
        attrOverrides.Add(typeof(PipeLineField), nameof(PipeLineField.Actions), attrs);
    }
}
