using Aperia.UT;
using Etl.Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.CryptorInfoTest
{
    [TestClass]
    public class SetupTestSuite
    {
        [TestMethod]
        public void InitCryptorInfo_Test()
        {
            TestBuilder.CreateInstance((NA _) =>
            {
                var builder = new SetupTestBuilder();

                return builder.Build();
            }).Action((ServiceCollection inst, IConfiguration arg) =>
            {
                return inst.AddEtlTransformation(arg);
            })
            .Validator<(string result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.IsTrue(inst.Any(s => s.ServiceType?.Name == expect.result));
            })
            .TestCases(GetSetupTestCases())
            .Run();
        }

        private SetupTestCase[] GetSetupTestCases()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("./TestFiles/appsettingsUT.json")
                .Build();

            return new SetupTestCase[]
            {
                new SetupTestCase("RegisterEtlSetting", NA.Value, configuration,(nameof(CryptorInfo), null))
            };
        }
    }
}
