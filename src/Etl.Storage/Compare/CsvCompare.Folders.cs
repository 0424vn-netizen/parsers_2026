using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace Etl.Storage
{
    public partial class CsvCompare
    {

        public static async Task ExportAllFileByDirectories(IConfiguration configuration)
        {
            try
            {
                var etlSetting = GetConfigFilesSetting(configuration);
                var directory = Directory.GetDirectories(etlSetting.Location).ToList();
                string specificFile = etlSetting.SpecificFile;
                var outputCompares = await GetAllFileAsync(etlSetting.Location, specificFile, 1);
                if (directory != null)
                {
                    foreach (var file in directory)
                    {
                        int index = outputCompares?.Count ?? 1;
                        var outputResults = await GetAllFileAsync(file, specificFile, index);
                        outputCompares?.AddRange(outputResults);
                    }
                }
                if (outputCompares.Any())
                {
                    var builder = new StringBuilder();
                    using var document = JsonSerializer.SerializeToDocument(outputCompares);
                    var root = document.RootElement.EnumerateArray();
                    var headers = root.First().EnumerateObject().Select(o => o.Name);
                    builder.AppendJoin(',', headers);
                    builder.AppendLine();
                    // new
                    foreach (var element in root)
                    {
                        var row = element.EnumerateObject().Select(o => o.Value.ToString());
                        builder.AppendJoin(',', row);
                        builder.AppendLine();
                    }
                    string pathResult = string.Concat(etlSetting.Location, "\\MPParserCompare", ".csv");
                    if (File.Exists(pathResult))
                    {
                        File.Delete(pathResult);
                    }
                    File.WriteAllText(pathResult, builder.ToString(), new UTF8Encoding());

                    Console.WriteLine($"EXPORT CSV: {pathResult}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exception: {ex.Message}");
            }
        }
        private static async Task<List<OutputCompare>> GetAllFileAsync(string pathFileName, string specificFile, int index)
        {
            var arrSpecific = specificFile?.Split('|')?.ToList() ?? new List<string> { fileNameValid };
            List<string> files = Directory.GetFiles(pathFileName)
                .Where(x => arrSpecific.Any(y => x.Contains(y, StringComparison.OrdinalIgnoreCase))
                && !x.Contains(fileNameCompare, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x)
                .ToList();
            var dictFiles = files.Select(x => x.Replace(fileNameLatest, ""))
                .Distinct()
                .ToDictionary(x => Path.GetFileName(x), x => new FileNameCompare
                {
                    PathFolder = Path.GetDirectoryName(x),
                    Name = Path.GetFileName(x),
                    PathProd = x,
                    PathLatestCode = string.Concat(x, fileNameLatest)
                });

            var outputCompares = new List<OutputCompare>();
            foreach (var item in dictFiles)
            {

                try
                {
                    string note = string.Empty;
                    var fileCompare = item.Value;
                    //Prod
                    var fileContentProd = File.Exists(fileCompare.PathProd) ? File.ReadAllText(fileCompare.PathProd) : string.Empty;
                    var contentProd = await ConvertToListAsync(fileContentProd, fileCompare.PathProd);
                    var hashProd = ComputeSha256Hash(contentProd);
                    note = string.IsNullOrEmpty(fileContentProd) ? noteCanNotFindProd : string.Empty;
                    //Latest code
                    var fileContentLatestCode = File.Exists(fileCompare.PathLatestCode) ? File.ReadAllText(fileCompare.PathLatestCode) : string.Empty;
                    var contentLatestCode = await ConvertToListAsync(fileContentLatestCode, fileCompare.PathLatestCode);
                    var hashLatestCode = ComputeSha256Hash(contentLatestCode);
                    note += string.IsNullOrEmpty(fileContentLatestCode) ? noteCanNotFindLatestCode : string.Empty;
                    //Compare
                    var isCompare = hashProd == hashLatestCode;
                    outputCompares.Add(new OutputCompare
                    {
                        Num = index++,
                        PathFolder = fileCompare.PathFolder,
                        FileName = fileCompare.Name,
                        HashProd = hashProd,
                        HashLatestCode = hashLatestCode,
                        IsCompare = isCompare,
                        Note = note
                    });
                }
                catch (Exception ex)
                {
                    outputCompares.Add(new OutputCompare
                    {
                        Num = index++,
                        PathFolder = item.Value != null ? item.Value.PathFolder : string.Empty,
                        FileName = item.Value != null ? item.Value.Name : string.Empty,
                        IsCompare = false,
                        Note = string.Concat("Exception:", ex.Message)
                    });
                }
            }
            return outputCompares;
        }
    }
}

