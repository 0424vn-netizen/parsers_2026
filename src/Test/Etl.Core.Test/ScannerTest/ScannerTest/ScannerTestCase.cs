using Aperia.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Etl.Core.Test.ScannerTest.ScannerTest.ScannerTestSuite;

namespace Etl.Core.Test.ScannerTest.ScannerTest
{
    public class ScannerTestCase : ITestCaseParam<ScannerTestBuilder, (ScannerTestBuilder construct, int? take, int? skip), (NA, Exception? ex)>
    {

        public ScannerTestCase(string caseName, ScannerTestBuilder constructArgs, (ScannerTestBuilder construct, int? take, int? skip) actionArgs, (NA, Exception? ex) expectation) {
            CaseName = caseName;
            ConstructArgs = constructArgs;
            ActionArgs = actionArgs;
            Expectation = expectation;
        }

        public string CaseName { get; set; }
        public ScannerTestBuilder ConstructArgs { get; set; }
        public (ScannerTestBuilder construct, int? take, int? skip) ActionArgs { get; set; }
        public (NA, Exception? ex) Expectation { get; set; }
    }
}
