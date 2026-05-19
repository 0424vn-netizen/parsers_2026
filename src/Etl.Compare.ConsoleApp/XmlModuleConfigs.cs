using Etl.Core.Settings;
using Etl.Core.Utils;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Etl.Compare.ConsoleApp
{
    public class XmlModuleConfigs
    {
        public static async void ExportCsvByAllFileModuleConfigs(IConfiguration configuration)
        {
            var etlSetting = GetConfigFilesSetting(configuration);
            var lstConfigs = new List<ModuleConfiguretion>();
            string path = etlSetting.XmlModuleConfigs;
            string[] files = Directory.GetFiles(path);
            files = files.OrderBy(x => x).ToArray();
            int index = 1;
            foreach (string file in files)
            {
                var moduleName = Path.GetFileName(file);
                string clientName = "";
                string configName = "";
                bool hasMPParser = false;
                string note = "";
                string pathSource = "";
                var fileContent = File.ReadAllText(file);
                if (!string.IsNullOrEmpty(fileContent))
                {
                    try
                    {
                        XElement xmlElement = XElement.Parse(fileContent);
                        var xmlElementNew = xmlElement;
                        var elmModuleData = xmlElement.Element("ModuleData");
                        var elmProperties = elmModuleData?.Element("Properties");
                        var elmsProperty = elmProperties?.Elements("Property") ?? new List<XElement>();
                        foreach (var item in elmsProperty)
                        {
                            var attName = item?.Attribute("name")?.Value;
                            var attValue = item?.Attribute("value")?.Value;
                            if (string.IsNullOrEmpty(attName))
                                continue;
                            if (attName.Equals("ClientID", StringComparison.OrdinalIgnoreCase))
                            {
                                clientName = attValue ?? "";
                            }
                            if (attName.Equals("ParserConfig", StringComparison.OrdinalIgnoreCase))
                            {
                                configName = Path.GetFileName(attValue ?? "");
                            }
                        }
                        var parserValue = xmlElement?
                            .Element("ModuleTasks")?
                            .Elements("Task")?
                            .Where(x => x != null && x.Attribute != null && x.Attribute("Type").Value != null && x.Attribute("Type").Value.Equals("EXEC", StringComparison.OrdinalIgnoreCase))
                            .Elements("Command")?
                            .Attributes("Text")?
                            .FirstOrDefault(x => x.Value.Contains("\\Parsers\\", StringComparison.OrdinalIgnoreCase))?
                            .Value;
                        if (!string.IsNullOrEmpty(parserValue))
                        {
                            FileInfo fileSource = GetPatchSource(parserValue);
                            if (fileSource == null)
                            {
                                var arrPathSrc = parserValue.Split(";");
                                var arrPathParser = arrPathSrc?.FirstOrDefault(x => x.Contains("Parsers", StringComparison.OrdinalIgnoreCase));
                                fileSource = GetPatchSource(arrPathParser);
                            }
                            pathSource = fileSource?.DirectoryName;
                            hasMPParser = parserValue.Contains("MPParser", StringComparison.OrdinalIgnoreCase);
                            if (hasMPParser)
                            {
                                var arr = parserValue.Split("-config=");
                                if (string.IsNullOrEmpty(configName) && arr != null && arr.Length > 1)
                                {
                                    configName = Path.GetFileName(arr[1]);
                                }
                            }
                            else
                            {
                                configName = string.Concat(fileSource?.FullName, ".config");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        note = ex.Message;
                    }
                }
                else
                {
                    note = "Empty Data";
                }
                lstConfigs.Add(new ModuleConfiguretion
                {
                    Num = index++,
                    ClientName = clientName,
                    ModuleName = moduleName,
                    ConfigName = configName,
                    PathSource = pathSource,
                    HasParser = hasMPParser,
                    Note = note
                });
            }
            if (lstConfigs.Any())
            {
                await ExportCsvAsync(lstConfigs, etlSetting.CsvModuleConfigs, "MPParser");  
                // Export Csv by group
                int indexTotal = 1;
                var dictConfigGroupData = lstConfigs
                    .Where(x => !string.IsNullOrEmpty(x.ConfigName))
                    .GroupBy(x => x.PathSource)
                    .Select(x => new DictModuleConfigs
                    {
                        Num = indexTotal++,
                        PathSource = x.Key,
                        Total = x.Count()
                    }).ToList();
                await ExportCsvAsync(dictConfigGroupData, etlSetting.CsvModuleConfigs, "MPParserGrp");               
            }
        }

        public static void AddXmlModuleConfigsOfLatestCode(IConfiguration configuration)
        {
            var etlSetting = GetConfigFilesSetting(configuration);
            var lstConfigs = new List<ModuleConfiguretion>();
            string path = etlSetting.XmlModuleConfigs;
            string[] files = Directory.GetFiles(path);
            files = files.OrderBy(x => x).ToArray();
            int total = 1;
            foreach (string file in files)
            {
                var moduleName = Path.GetFileName(file);
                string configName = "";
                string note = "";
                var fileContent = File.ReadAllText(file);
                if (!string.IsNullOrEmpty(fileContent))
                {
                    try
                    {
                        XElement xmlElement = XElement.Parse(fileContent);
                        var xmlElementNew = xmlElement;
                        var elmModuleData = xmlElement.Element("ModuleData");
                        var elmProperties = elmModuleData?.Element("Properties");
                        var elmsProperty = elmProperties?.Elements("Property") ?? new List<XElement>();
                        bool isCreateXml = false;
                        bool isAddProperty = false;
                        string pathXmlConfig = "";
                        foreach (var item in elmsProperty)
                        {
                            var attName = item?.Attribute("name")?.Value;
                            var attValue = item?.Attribute("value")?.Value;
                            if (string.IsNullOrEmpty(attName))
                                continue;
                            if (attName.Equals("ParserConfig", StringComparison.OrdinalIgnoreCase))
                            {
                                configName = Path.GetFileName(attValue ?? "");
                                isCreateXml = true;
                                isAddProperty = true;
                                pathXmlConfig = attValue?.Replace("\\MPParser\\", "\\MPParserLatest\\");
                                break;
                            }
                        }
                        var commandTextAttr = xmlElement.XPathSelectElements("//Command[@Text]").Attributes("Text");
                        var parserTaskValue = commandTextAttr?.FirstOrDefault(x => x.Value.Contains("MPParser"))?.Value;
                        var parserLatestTaskValue = commandTextAttr?.FirstOrDefault(x => x.Value.Contains("MPParserLatest"))?.Value;
                        isCreateXml = isCreateXml || (!isCreateXml && !string.IsNullOrEmpty(parserTaskValue));
                        bool hasParserConfigLatest = isCreateXml
                            && (elmsProperty.Any(x => x.Attribute != null && x.Attribute("name").Value.Equals("ParserConfigLatest", StringComparison.OrdinalIgnoreCase)) || !string.IsNullOrEmpty(parserLatestTaskValue));
                        string pathFolder = etlSetting.XmlModuleConfigsLatest;
                        string pathXml = Path.Combine(pathFolder, moduleName);

                        if (hasParserConfigLatest)
                        {
                            if (File.Exists(pathXml))
                            {
                                File.Delete(pathXml);
                            }
                            File.WriteAllText(pathXml, xmlElement.ToString(), new UTF8Encoding());
                            continue;
                        }

                        if (!isCreateXml)
                        {
                            var parserValue = xmlElement?
                               .Element("ModuleTasks")?
                               .Elements("Task")?
                               .Where(x => x != null && x.Attribute != null && x.Attribute("Type").Value != null && x.Attribute("Type").Value.Equals("EXEC", StringComparison.OrdinalIgnoreCase))
                               .Elements("Command")?
                               .Attributes("Text")?
                               .FirstOrDefault(x => x.Value.Contains("MPParser"))?
                               .Value;
                            isCreateXml = !string.IsNullOrEmpty(parserValue);
                        }

                        if (isCreateXml && isAddProperty)
                        {
                            xmlElement.XPathSelectElement("ModuleData//Properties").Add(
                                         new XElement("Property",
                                         new XAttribute("name", "ParserConfigLatest"),
                                         new XAttribute("value", pathXmlConfig)
                                         ));
                        }

                        if (isCreateXml)
                        {
                            var taskMPParser = xmlElement.XPathSelectElements("//Command[@Text]")
                           .Attributes("Text")?
                           .FirstOrDefault(x => x.Value.Contains("MPParser"));

                            var taskLatestCodeStr = taskMPParser?.Parent?.Parent?.ToString()?.Replace("\\MPParser\\", "\\MPParserLatest\\")?.Replace(".ParserConfig", ".ParserConfigLatest");
                            if (taskLatestCodeStr == null)
                            {
                                continue;
                            }
                            xmlElement.XPathSelectElement("ModuleTasks").Add(XElement.Parse(taskLatestCodeStr));

                            if (File.Exists(pathXml))
                            {
                                File.Delete(pathXml);
                            }
                            File.WriteAllText(pathXml, xmlElement.ToString(), new UTF8Encoding());
                            total++;
                        }
                    }
                    catch (Exception ex)
                    {
                        note = ex.Message;
                    }
                }
            }
            Console.WriteLine($"EXPORT {total} XML FILES BY FOLDER: {etlSetting.XmlModuleConfigsLatest}");
        }
        public static bool IsCompare(IConfiguration configuration)
        {
            return configuration.GetSection("Etl").Get<EtlSetting>()?.ConfigFiles?.IsCompare ?? false;
        }
        private static ConfigFilesSetting GetConfigFilesSetting(IConfiguration configuration)
        {
            return configuration.GetSection("Etl").Get<EtlSetting>()?.ConfigFiles ?? new ConfigFilesSetting();
        }

        private static FileInfo GetPatchSource(string parserValue)
        {
            var arrPathSourc = parserValue.Split(".exe");
            if (arrPathSourc.Any() && arrPathSourc[0].Contains("Parsers", StringComparison.OrdinalIgnoreCase))
            {
                var pathName = arrPathSourc.Any() ? Regex.Replace(arrPathSourc[0], "\"", "") : string.Empty;
                return new FileInfo(FilePath.GetFullPath(string.Concat(pathName, ".exe")));
            }
            return default;
        }

        private static async Task ExportCsvAsync<T>(List<T> lstConfigs, string pathName, string fileName)
            where T : class
        {
            var builder = new StringBuilder();
            using var document = JsonSerializer.SerializeToDocument(lstConfigs);
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
            string pathCsv = $"{pathName}/{fileName}.csv";
            if (File.Exists(pathCsv))
            {
                File.Delete(pathCsv);
            }
            await File.WriteAllTextAsync(pathCsv, builder.ToString(), new UTF8Encoding());
            Console.WriteLine($"EXPORT {fileName} CSV: {pathCsv}");
        }
    }
    public class ModuleConfiguretion
    {
        public int Num { get; set; }
        public bool HasParser { get; set; }
        public string ClientName { get; set; }
        public string ModuleName { get; set; }
        public string ConfigName { get; set; }
        public string PathSource { get; set; }
        public string Note { get; set; }
    }

    public class DictModuleConfigs
    {
        public int Num { get; set; }
        public string ConfigName { get; set; }
        public string PathSource { get; set; }
        public int Total { get; set; }
    }
}
