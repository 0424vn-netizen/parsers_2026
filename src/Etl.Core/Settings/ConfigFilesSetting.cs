using Microsoft.Extensions.Configuration;

namespace Etl.Core.Settings;

public class ConfigFileItemSetting
{
    public string Category { get; set; } = String.Empty;
    public string Pattern { get; set; } = String.Empty;
    public string File { get; set; } = String.Empty;

    public IConfigurationSection[] Loaders { get; set; } = Array.Empty<IConfigurationSection>();
}

public class ConfigFilesSetting
{
    public string Location { get; set; } = "./EtlConfigs";
    public string SpecificFile { get; set; } = "_VALID";
    public bool IsCompare { get; set; } = false;
    public int ThreadSleep { get; set; }
    public int Retry { get; set; }
    public string XmlModuleConfigs { get; set; } 
    public string XmlModuleConfigsLatest { get; set; }
    public string CsvModuleConfigs { get; set; }

    public Dictionary<string, ConfigFileItemSetting> Categories { get; set; } = new();
}
