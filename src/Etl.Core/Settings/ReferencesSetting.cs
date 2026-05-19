namespace Etl.Core.Settings;

public class ReferencesSetting
{
    public List<string> TransformFields { get; set; } = new();

    public List<string> Loaders { get; set; } = new();
}
