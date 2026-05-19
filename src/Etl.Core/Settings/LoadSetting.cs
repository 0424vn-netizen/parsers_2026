using Microsoft.Extensions.Configuration;

namespace Etl.Core.Settings;

public class LoadSetting
{
    public int BatchSize { get; set; } = 10 * 1000;

    public List<string> References { get; set; } = new();

    public IConfigurationSection? Loaders { get; set; }
}
