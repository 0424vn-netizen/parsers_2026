using Etl.Core.Utils;
using System.Text;
using System.Text.Json;

namespace Etl.Storage
{
    public partial class CsvCompare
    {
        public static async Task ConvertToOrderOutputFileName(CompareArgs args)
        {
            try
            {
                var setting = GetConfigFilesSetting(args);
                if (!(setting?.IsCompare ?? false))
                {
                    return;
                }
                if ((setting?.ThreadSleep ?? 0) > 0)
                {
                    await Task.Delay(setting?.ThreadSleep ?? 0);
                }
                var outputCompares = new List<OutputCompare>();
                foreach (var item in args.Config.Loaders)
                {
                    CsvLoader? itemCsvLoader = item as CsvLoader;
                    if (itemCsvLoader == null)
                    {
                        continue;
                    }

                    //get path file
                    var outPathLatestCode = string.Concat(itemCsvLoader.OutPath, fileNameLatest);
                    var file = new FileInfo(FilePath.GetFullPath(args.DataFile));
                    var path = outPathLatestCode.Replace("$path", file.DirectoryName)
                        .Replace("$name", file.Name);

                    //get content output
                    var outputResult = await GetOutputByFileNameAsync(path, setting?.Retry ?? 0);
                    if (outputResult != null)
                    {
                        outputResult.ConfigFile = args.ConfigFile;
                        outputResult.InputDataFile = args.DataFile;
                    }

                    //build content csv
                    var builder = new StringBuilder();
                    using var document = JsonSerializer.SerializeToDocument(outputResult);
                    var root = document.RootElement.EnumerateObject();
                    var headers = root.Select(o => o.Name);
                    builder.AppendJoin(',', headers);
                    builder.AppendLine();
                    var row = root.Select(o => o.Value.ToString());
                    builder.AppendJoin(',', row);
                    builder.AppendLine();
                    //export csv
                    string pathResult = string.Concat(path, "_COMPARE_", outputResult.IsCompare.ToString().ToUpper(), ".csv");
                    if (File.Exists(pathResult))
                    {
                        File.Delete(pathResult);
                    }
                    await File.WriteAllTextAsync(pathResult, builder.ToString(), new UTF8Encoding());                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exception CsvCompare FileName: {ex.Message}");
            }
        }

        private static async Task<OutputCompare> GetOutputByFileNameAsync(string pathFileName, int retry = 0)
        {
            try
            {

                var fileCompare = new FileNameCompare
                {
                    PathFolder = Path.GetDirectoryName(pathFileName),
                    Name = Path.GetFileName(pathFileName),
                    PathProd = pathFileName.Replace(fileNameLatest, ""),
                    PathLatestCode = pathFileName
                };

                string note = string.Empty;
                //Prod
                if (!File.Exists(fileCompare.PathProd) && retry > 0)
                {
                    int retryProd = retry;
                    bool isFileProd = false;
                    do
                    {
                        isFileProd = File.Exists(fileCompare.PathProd);
                        retryProd--;
                        if (isFileProd) retryProd = 0;
                    } while (retryProd > 0);

                }
                var fileContentProd = File.Exists(fileCompare.PathProd) ? File.ReadAllText(fileCompare.PathProd) : string.Empty;
                var contentProd = await ConvertToListAsync(fileContentProd, fileCompare.PathProd);
                var hashProd = ComputeSha256Hash(contentProd);
                note = string.IsNullOrEmpty(fileContentProd) ? noteCanNotFindProd : string.Empty;
                //Latest code
                if (!File.Exists(fileCompare.PathLatestCode) && retry > 0)
                {
                    int retryProd = retry;
                    bool isFileProd = false;
                    do
                    {
                        isFileProd = File.Exists(fileCompare.PathLatestCode);
                        retryProd--;
                        if (isFileProd) retryProd = 0;
                    } while (retryProd > 0);

                }
                var fileContentLatestCode = File.Exists(fileCompare.PathLatestCode) ? File.ReadAllText(fileCompare.PathLatestCode) : string.Empty;
                var contentLatestCode = await ConvertToListAsync(fileContentLatestCode, fileCompare.PathLatestCode);
                var hashLatestCode = ComputeSha256Hash(contentLatestCode);
                note += string.IsNullOrEmpty(fileContentLatestCode) ? noteCanNotFindLatestCode : string.Empty;
                //Compare
                var isCompare = !string.IsNullOrEmpty(hashProd) && hashProd == hashLatestCode;
                return new OutputCompare
                {
                    Num = 1,
                    PathFolder = fileCompare.PathFolder,
                    FileName = fileCompare.Name,
                    HashProd = hashProd,
                    HashLatestCode = hashLatestCode,
                    IsCompare = isCompare,
                    Note = note
                };
            }
            catch (Exception ex)
            {
                return new OutputCompare
                {
                    Num = 1,
                    PathFolder = Path.GetDirectoryName(pathFileName),
                    FileName = Path.GetFileName(pathFileName),
                    IsCompare = false,
                    Note = ex.Message
                };
            }
        }
    }
}
