using Etl.Core.Events;
using Etl.Core.Load;
using Etl.Core.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Etl.Core;

public class EtlBuilder
{
    private readonly IServiceProvider _sp;
    private readonly EtlSetting _etlSetting;
    private readonly IEtlFactory _etlFactory;
    private readonly List<Loader> _extraLoaders = new();

    private bool _isResetLoaders;
    private IEtlEvent? _events;
    private Etl? _etl;

    public EtlBuilder(IServiceProvider sp)
    {
        _sp = sp;

        _etlSetting = sp.GetRequiredService<EtlSetting>();
        _etlFactory = sp.GetRequiredService<IEtlFactory>();
    }

    public EtlBuilder Subcribe(Action<IEtlEvent>? subscribe)
    {
        if (subscribe != null)
        {
            var e = new EtlEvent();
            subscribe(e);

            _events = e;
        }
        return this;
    }

    public EtlBuilder SetConfig(string configFilePath)
    {
        if (string.IsNullOrEmpty(configFilePath))
            return this;

        var etl = _etlFactory.GetByConfigFile(configFilePath);
        return SetConfig(etl);
    }

    public EtlBuilder SetStreamConfig(Stream xmlLayout)
    {
        if (xmlLayout == null)
            return this;

        var etl = _etlFactory.GetByConfigStream(xmlLayout);
        return SetConfig(etl);
    }

    public EtlBuilder SetConfig(Etl etl)
    {
        if (etl != null)
            _etl = etl;
        return this;
    }

    public EtlBuilder ResetLoaders(params Loader[] loaders)
    {
        _isResetLoaders = true;
        _extraLoaders.Clear();
        return AddLoaders(loaders);
    }

    public EtlBuilder AddLoaders(params Loader[] loaders)
    {
        if (loaders != null)
            _extraLoaders.AddRange(loaders);
        return this;
    }

    public EtlBuilder AddLoaders(IEnumerable<Loader>? loaders)
    {
        if (loaders != null)
            _extraLoaders.AddRange(loaders);
        return this;
    }

    public EtlInstance Build(string dataFilePath, StreamReader? dataStream = null)
    {
        if (dataStream == null && !File.Exists(dataFilePath))
            throw new FileNotFoundException($"Not existed data file '{dataFilePath}'.");

        var (loaders, configFilePath) = _etlFactory.GetLoadersAndConfigPath(dataFilePath);
        _etl ??= _etlFactory.GetByConfigFile(configFilePath);

        if (_isResetLoaders)
            loaders = _extraLoaders;
        else
            loaders.AddRange(_extraLoaders);

        dataStream ??= new StreamReader(dataFilePath);
        return new EtlInstance(_sp, _etlSetting, _etl, _events, dataFilePath, dataStream, loaders);
    }

    public Etl? GetEtl()
    {
        return _etl;
    }
    public EtlSetting GetEtlSetting()
    {
        return _etlSetting;
    }
}
