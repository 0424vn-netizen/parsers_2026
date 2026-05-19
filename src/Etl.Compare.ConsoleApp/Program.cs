using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Etl.Storage;
using Etl.Compare.ConsoleApp;

partial class Program
{
    static async Task Main(string[] arguments)
    {
        var start = DateTime.Now;
        Console.WriteLine($"\n============================ START {start} ================================");
        var configuration = BuildConfiguration();
        var services = new ServiceCollection();
        services.AddSingleton(configuration); 
        if (XmlModuleConfigs.IsCompare(configuration))
        {
            Console.WriteLine($"\nInitializing... Add module configs  the latest to MPParsers");
            //Compare output file and export file between version PROD - LatestSource
            await CsvCompare.ExportAllFileByDirectories(configuration);
        }
        else
        {
            Console.WriteLine($"\nInitializing... Export CSV of ModuleConfigs XML exists in MPParser");
            //Export all file config Parser to Csv
            XmlModuleConfigs.ExportCsvByAllFileModuleConfigs(configuration);
            //Additional xml config MPParser for latestCode
            XmlModuleConfigs.AddXmlModuleConfigsOfLatestCode(configuration);
        }

        Console.WriteLine($"Processed: {DateTime.Now.Subtract(start)}");
        Console.WriteLine($"\n============================ END {DateTime.Now.Subtract(start)} ================================");
        Console.ReadLine();
    }

    static IConfiguration BuildConfiguration()
    {
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddJsonFile("appsettings.json");

        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (!string.IsNullOrWhiteSpace(env))
            configBuilder.AddJsonFile("appsettings.{env}.json");

        return configBuilder.Build();
    }
}
