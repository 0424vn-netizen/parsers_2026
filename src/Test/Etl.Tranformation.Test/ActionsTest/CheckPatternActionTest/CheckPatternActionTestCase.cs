using Aperia.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.ActionsTest.CheckPatternActionTest
{
    public class CheckPatternActionTestCase : ITestCaseParam<NA, (string?, string?), (bool, Exception? ex)>
    {
        public CheckPatternActionTestCase(string caseName, NA constructArgs, (string? input, string? pattern) ActionArgs, (bool, Exception? ex) expectation)
        {
            CaseName = caseName;
            ConstructArgs = constructArgs;
            Expectation = expectation;
            this.ActionArgs = ActionArgs!;
        }

        public string CaseName { get; set; }
        public NA ConstructArgs { get; set; }
        public (string?, string?) ActionArgs { get; set; }
        public (bool, Exception? ex) Expectation { get; set; }
    }
}
