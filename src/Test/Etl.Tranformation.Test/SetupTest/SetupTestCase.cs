using Aperia.UT;
using Etl.Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.CryptorInfoTest
{
    public class SetupTestCase : ITestCaseParam<NA, IConfiguration, (string result, Exception? ex)>
    {
        public SetupTestCase(string caseName, NA constructArgs, IConfiguration ActionArgs , (string result, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = ActionArgs;
        }
        public string CaseName { get; set; }
        public NA ConstructArgs { get; set; }
        public IConfiguration ActionArgs { get; set; }
        public (string result, Exception? ex) Expectation { get; set; }
    }
}
