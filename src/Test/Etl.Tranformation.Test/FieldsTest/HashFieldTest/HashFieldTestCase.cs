using Aperia.UT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.FieldsTest.HashFieldTest
{
    public class HashFieldTestCase : ITestCaseParam<CryptorInfo, string?, (string?, Exception? ex)>
    {
        public HashFieldTestCase(string caseName, CryptorInfo constructArgs, string? ActionArgs, (string?, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = ActionArgs!;
        }

        public string CaseName { get; set; }
        public CryptorInfo ConstructArgs { get; set; }
        public string? ActionArgs { get; set; }
        public (string?, Exception? ex) Expectation { get; set; }
    }
}
