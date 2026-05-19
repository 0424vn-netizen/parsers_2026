using Aperia.UT;
using Etl.Core.Settings;
using Etl.Core.Transformation.Actions;
using Etl.Tranformation.Fields;
using Etl.Tranformation.Test.FieldsTest.EncryptFieldTest;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.FieldsTest.HashFieldTest
{
    [TestClass]
    public class HashFieldTestSuite
    {
        [TestMethod]
        public void EncryptField_Test()
        {
            TestBuilder.CreateInstance((CryptorInfo constructArgs) =>
            {
                var builder = new HashFieldTestBuilder();
                builder._cryptoInfo = constructArgs;
                return builder.Build();
            }).Action((IServiceCollection inst, string? arg) =>
            {
                var provider = inst.BuildServiceProvider();
                var field = new HashField();
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

        private HashFieldTestCase[] GetTestCases()
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

            return new HashFieldTestCase[]
            {
                new HashFieldTestCase("Should_Be_Null_Test_Case", new CryptorInfo(validEtlSetting), null, (null, null)),
                new HashFieldTestCase("Should_Be_Hashed_Without_Hash_String_Test_Case", new CryptorInfo(nullCryptoEtlSetting), "text", ("982D9E3EB996F559E633F4D194DEF3761D909F5A3B647D1A851FEAD67C32C9D1", null)),
                new HashFieldTestCase("Should_Be_Hashed_With_Hash_String_Test_Case", new CryptorInfo(validEtlSetting), "text", ("58FC46774E601229167247AC17115C9FD1A868B932C15B4220491FDC64A72ADA", null))
            };
        }
    }
}
