using Aperia.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.FieldsTest.DateFieldTest
{
    public class DateFieldTestCase : ITestCaseParam<NA, string, (DateTime?, Exception? ex)>
    {
        public DateFieldTestCase(string caseName, NA constructArgs, string? ActionArgs, (DateTime?, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = ActionArgs!;
        }

        public string CaseName { get; set; }
        public NA ConstructArgs { get; set; }
        public string ActionArgs { get; set; }
        public (DateTime?, Exception? ex) Expectation { get; set; }
    }
}