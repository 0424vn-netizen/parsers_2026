using Etl.Core.Settings;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Etl.Storage
{
    public partial class CsvCompare
    {
        private readonly static string fileNameLatest = "_LATEST";
        private readonly static string fileNameValid = "_VALID";
        private readonly static string fileNameCompare = "_COMPARE";
        private readonly static string noteCanNotFindProd = "Can't find PROD;";
        private readonly static string noteCanNotFindLatestCode = "Can't find Latest Code;";
        private static async Task<string?> ConvertToListAsync(string fileContent, string pathName)
        {
            if (string.IsNullOrEmpty(fileContent))
                return default;
            var builder = new StringBuilder();
            var arrContenttProd = fileContent
                       .Split("\r\n")
                       .Where(x => !string.IsNullOrEmpty(x))
                       .ToList();
            var header = arrContenttProd.FirstOrDefault();
            builder.AppendLine(header);
            var results = arrContenttProd.Where(x => x != header)
                .OrderBy(x => x)
                .ToList();
            builder.AppendJoin("\r\n", results);
            var dataResult = builder.ToString();
            string pathResult = string.Concat(pathName, "_COMPARE");
            if (File.Exists(pathResult))
            {
                File.Delete(pathResult);
            }
            await File.WriteAllTextAsync(pathResult, dataResult, new UTF8Encoding());
            return dataResult;
        }

        private static string ComputeSha256Hash(string rawData)
        {
            if (string.IsNullOrEmpty(rawData))
                return string.Empty;
            // Create a SHA256
            using SHA256 sha256Hash = SHA256.Create();
            // ComputeHash - returns byte array
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            // Convert byte array to a string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        private static ConfigFilesSetting GetConfigFilesSetting(CompareArgs args)
        {
            return args?.EtlSetting?.ConfigFiles ?? new ConfigFilesSetting();
        }
        private static ConfigFilesSetting GetConfigFilesSetting(IConfiguration configuration)
        {
            return configuration.GetSection("Etl").Get<EtlSetting>()?.ConfigFiles ?? new ConfigFilesSetting();
        }
    }
    public class FileNameCompare
    {
        public string PathFolder { get; set; }
        public string FolderName
        {
            get
            {
                if (string.IsNullOrEmpty(PathFolder))
                    return string.Empty;
                return PathFolder.Split('\\').Last();
            }
        }
        public string ClientID
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return string.Empty;
                return Name.Split("_")[0];
            }
        }
        public string Name { get; set; }
        public string PathProd { get; set; }
        public string PathLatestCode { get; set; }
    }

    public class OutputCompare
    {
        public int Num { get; set; }
        public string PathFolder { get; set; }
        public string FolderName
        {
            get
            {
                if (string.IsNullOrEmpty(PathFolder))
                    return string.Empty;
                return PathFolder.Split('\\').Last();
            }
        }
        public string ConfigFile { get; set; }
        public string InputDataFile { get; set; }
        public string Client
        {
            get
            {
                if (string.IsNullOrEmpty(FileName))
                    return string.Empty;
                return FileName.Split("_")[0];
            }
        }
        public string FileName { get; set; }
        public string HashProd { get; set; }
        public string HashLatestCode { get; set; }
        public bool IsCompare { get; set; }

        public string Note { get; set; }
    }

    public class CompareArgs
    {
        public string DataFile = string.Empty;
        public string ConfigFile = string.Empty;
        public Core.Etl Config { get; set; }
        public Core.Settings.EtlSetting EtlSetting { get; set; }
    }
}
