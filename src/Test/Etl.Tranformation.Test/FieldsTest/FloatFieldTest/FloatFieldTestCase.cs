using Aperia.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.FieldsTest.FloatFieldTest
{
    public class FloatFieldTestCase : ITestCaseParam<NA, string, (double?, Exception? ex)>
    {
        public FloatFieldTestCase(string caseName, NA constructArgs, string? ActionArgs, (double?, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = ActionArgs!;
        }

        public string CaseName { get; set; }
        public NA ConstructArgs { get; set; }
        public string ActionArgs { get; set; }
        public (double?, Exception? ex) Expectation { get; set; }
    }
}
