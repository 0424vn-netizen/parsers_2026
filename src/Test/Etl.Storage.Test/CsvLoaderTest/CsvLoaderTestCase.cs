using Aperia.UT;
using Etl.Core.Load;
using Etl.Core.Settings;
using Etl.Tranformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Storage.Test.CsvLoaderTest
{
    public class CsvLoaderTestCase : ITestCaseParam<NA, (CsvLoader defintion, LoaderArgs args, BatchResult? batch), (string result, Exception? ex)>
    {
        public CsvLoaderTestCase(string caseName, NA constructArgs, (CsvLoader defintion, LoaderArgs args, BatchResult? batch) ActionArgs, (string result, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = ActionArgs;
        }
        public string CaseName { get; set; }
        public NA ConstructArgs { get; set; }
        public (CsvLoader defintion, LoaderArgs args, BatchResult? batch) ActionArgs { get; set; }
        public (string result, Exception? ex) Expectation { get; set; }
    }
}
