using Etl.Core.Scanner;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EtlScanner = Etl.Core.Scanner.Scanner;


namespace Etl.Core.Test.ScannerTest.ScannerTest
{
    public class ScannerTestBuilder
    {
        public (Regex? regex, int offset) StartLayout;
        public (Regex? regex, int offset) StartRecord;
        public (Regex? regex, int offset) EndRecord;
        public StreamReader StreamReader;
        public Mock<Func<ScannedRecord, Task>?> MockOnFlushAsync;

        public ScannerTestBuilder()
        {
            MockOnFlushAsync = new Mock<Func<ScannedRecord, Task>?>();            
            StreamReader = new StreamReader("./TestData/Delimiter-demo");
        }

        public EtlScanner Build()
        {
            return new EtlScanner(StreamReader,StartLayout, StartRecord, EndRecord, MockOnFlushAsync.Object);
        }

    }
}
