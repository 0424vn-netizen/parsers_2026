using Aperia.UT;
using Etl.Core.Settings;
using Etl.Core.Transformation.Actions;
using Etl.Tranformation.Actions;
using Etl.Tranformation.Fields;
using Etl.Tranformation.Test.FieldsTest.DateFieldTest;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.FieldsTest.EncryptFieldTest
{
    [TestClass]
    public class EncryptFieldTestSuite
    {
        [TestMethod]
        public void EncryptField_Test()
        {
            TestBuilder.CreateInstance((CryptorInfo constructArgs) =>
            {
                var builder = new EncryptFieldTestBuilder();
                builder._cryptoInfo = constructArgs;
                return builder.Build();
            }).Action((IServiceCollection inst, string? arg) =>
            {
                var provider = inst.BuildServiceProvider();
                var field = new EncryptField();
                var actionType = field.DefaultAction.GetType().GetProperty("ActionType", System.Reflection.BindingFlags.NonPublic
                                                                            | System.Reflection.BindingFlags.Instance).GetValue(field.DefaultAction) as Type;

                var transformAction = (ITransformActionInst)provider.GetRequiredService(actionType);
                return transformAction.Execute(arg, null);
            })
            .Validator<(string? result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result?.Result, expect.result);
            })
            .TestCases(GetTestCases())
            .Run();
        }

        private EncryptFieldTestCase[] GetTestCases()
        {
            var validEtlSetting = new EtlSetting()
            {
                Transformation = new TransformationSetting
                {
                    Cryptor = new CryptorSetting
                    {
                        CryptorFile = "./TestFiles/CryptorKeyUnitTest.xml",
                        HashFile = "./TestFiles/CryptorUnitTest.xml"
                    }
                }
            };

            var nullCryptoEtlSetting = new EtlSetting()
            {
                Transformation = new TransformationSetting
                {
                    Cryptor = null
                }
            };

            return new EncryptFieldTestCase[]
            {
                new EncryptFieldTestCase("Should_Be_Null_Test_Case_1", new CryptorInfo(validEtlSetting), null, (null, null)),
                new EncryptFieldTestCase("Should_Be_Null_Test_Case_2", new CryptorInfo(nullCryptoEtlSetting), "text", (null, null)),
                new EncryptFieldTestCase("Should_Be_Encrypted_Test_Case", new CryptorInfo(validEtlSetting), "text", ("3qy/Mb30O+rQAjvLuIGJeQ==", null))
            };
        }
    }
}