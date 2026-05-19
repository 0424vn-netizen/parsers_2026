using Aperia.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.FieldsTest.IntegerFieldTest
{
    public class IntegerFieldTestCase : ITestCaseParam<NA, string, (int?, Exception? ex)>
    {
        public IntegerFieldTestCase(string caseName, NA constructArgs, string? ActionArgs, (int?, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = ActionArgs!;
        }

        public string CaseName { get; set; }
        public NA ConstructArgs { get; set; }
        public string ActionArgs { get; set; }
        public (int?, Exception? ex) Expectation { get; set; }
    }
}
