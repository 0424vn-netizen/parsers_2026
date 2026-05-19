using Aperia.UT;
using Etl.Core.Test.LoadTest.BatchResultTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Core.Test.ScannerTest.TextLineTest
{
    public class TextLineTestCase : ITestCaseParam<TextLineInput, TextLineInput, (string result, Exception? ex)>
    {
        public TextLineTestCase(string caseName, TextLineInput constructArgs, TextLineInput ActionArgs, (string result, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = ActionArgs;
        }
        public string CaseName { get; set; }
        public TextLineInput ConstructArgs { get; set; }
        public TextLineInput ActionArgs { get; set; }
        public (string result, Exception? ex) Expectation { get; set; }
    }
}
