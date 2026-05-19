using Aperia.UT;
using System;

namespace Etl.Tranformation.Test.FieldsTest.BooleanFieldTest
{
    public class BooleanFieldTestCase : ITestCaseParam<NA, string, (bool?, Exception? ex)>
    {
        public BooleanFieldTestCase(string caseName, NA constructArgs, string? ActionArgs, (bool?, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = ActionArgs!;
        }

        public string CaseName { get; set; }
        public NA ConstructArgs { get; set; }
        public string ActionArgs { get; set; }
        public (bool?, Exception? ex) Expectation { get; set; }
    }
}