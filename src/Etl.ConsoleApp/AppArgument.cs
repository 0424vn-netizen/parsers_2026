namespace Etl.ConsoleApp;

partial class Program
{
    public class AppArgument
    {
        public string DataFile = string.Empty;
        public string ConfigFile = string.Empty;
        public int? Take;
        public int? Skip;
        public bool OnScanned;
        public bool OnExtracting;
        public bool OnExtracted;
        public bool OnTransformed;
        public bool OnTransformedBatch;
        public int OnStatusIntervalSeconds;

        public Core.Etl Config { get; set; }
    }
}
