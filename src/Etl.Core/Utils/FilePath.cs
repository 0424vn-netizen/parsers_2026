namespace Etl.Core.Utils;

public static class FilePath
{
    public static string GetFullPath(string path)
    {
        path = path.StartsWith(".") ? $"{AppContext.BaseDirectory}{path}" : path;
        return Path.GetFullPath(path);
    }
}
