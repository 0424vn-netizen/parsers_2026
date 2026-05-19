using Aperia.UT;
using Etl.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Core.Test.LoadTest.BatchResultTest
{   
    public class BatchResultTestCase : ITestCaseParam<BatchInput, BatchInput, (string result, Exception? ex)>
    {
        public BatchResultTestCase(string caseName, BatchInput constructArgs, BatchInput ActionArgs, (string result, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = ActionArgs;
        }
        public string CaseName { get; set; }
        public BatchInput ConstructArgs { get; set; }
        public BatchInput ActionArgs { get; set; }
        public (string result, Exception? ex) Expectation { get; set; }
    }
}
