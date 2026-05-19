using Aperia.UT;
using Etl.Core.Load;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Storage.Test.InvalidFileLoaderTest
{
    public class InvalidFileLoaderTestCase : ITestCaseParam<NA, (InvalidLogLoader defintion, LoaderArgs args, BatchResult? batch), (string result, Exception? ex)>
    {
        public InvalidFileLoaderTestCase(string caseName, NA constructArgs, (InvalidLogLoader defintion, LoaderArgs args, BatchResult? batch) ActionArgs, (string result, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = ActionArgs;
        }

        public string CaseName { get; set; }
        public NA ConstructArgs { get; set; }
        public (InvalidLogLoader defintion, LoaderArgs args, BatchResult? batch) ActionArgs { get; set; }
        public (string result, Exception? ex) Expectation { get; set; }
    }
}
