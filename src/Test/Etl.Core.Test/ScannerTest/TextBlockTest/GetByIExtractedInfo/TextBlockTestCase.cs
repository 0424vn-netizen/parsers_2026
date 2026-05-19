using Aperia.UT;
using Etl.Core.Extraction;
using Etl.Core.Scanner;
using Etl.Core.Test.LoadTest.BatchResultTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Core.Test.ScannerTest.TextBlockTest.GetByIExtractedInfo
{
    public class TextBlockTestCase : ITestCaseParam<TextBlockTestBuilder, (TextBlockTestBuilder construct, List<TextLine> lines), (string result, Exception? ex)>
    {
        public TextBlockTestCase(string caseName, TextBlockTestBuilder constructArgs, (TextBlockTestBuilder construct, List<TextLine> lines) actionArgs, (string result, Exception? ex) expectation)
        {
            CaseName = caseName;
            ConstructArgs = constructArgs;
            Expectation = expectation;
            ActionArgs = actionArgs;
        }
        public string CaseName { get; set; }
        public TextBlockTestBuilder ConstructArgs { get; set; }
        public (TextBlockTestBuilder construct, List<TextLine> lines) ActionArgs { get; set; }
        public (string result, Exception? ex) Expectation { get; set; }
    }
}
