namespace Etl.Tranformation.Test.CryptorInfoTest;

using Aperia.UT;
using Etl.Core;
using Etl.Core.Settings;
using Etl.Core.Transformation.Actions;
using Etl.Core.Transformation.Fields;
using Etl.Tranformation.Actions;
using Etl.Tranformation.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

[TestClass]
public class CryptorInfoTestSuite
{
    [TestMethod]
    public void InitCryptorInfo_Test()
    {
        TestBuilder.CreateInstance((EtlSetting construc) =>
        {
            var builder = new CryptorInfoInstanceBuilder();

            builder.mockEltSetting = construc;

            return builder.Build();
        }).Action((CryptorInfo inst, (EtlSetting param, CryptorInfo) arg) =>
        {
            return new CryptorInfo(arg.param);
        })
        .Validator<(CryptorInfo result, Exception? ex)>((inst, construc, args, result, expect) =>
        {
            Assert.AreEqual(result?.Result?.SaltHashString, expect.result?.SaltHashString);
            CollectionAssert.AreEqual(result?.Result?.Config.key, expect.result?.Config.key);
            CollectionAssert.AreEqual(result?.Result?.Config.iv, expect.result?.Config.iv);
            Assert.AreEqual(result?.Exception?.GetType(), expect.ex?.GetType());
        })
        .TestCases(GetCryptInfoTestCases())
        .Run();
    }

    private CryptorInfoTestCase[] GetCryptInfoTestCases()
    {
        var emptyEtlSetting = new EtlSetting()
        {
            Transformation = null
        };
        var nullCryptoEtlSetting = new EtlSetting()
        {
            Transformation = new TransformationSetting
            {
                Cryptor = null
            }
        };

        var validEtlSetting = new EtlSetting()
        {
            Transformation = new TransformationSetting
            {
                Cryptor = new CryptorSetting
                {
                    CryptorFile = "./TestFiles/CryptorUnitTest.xml",
                    HashFile = "./TestFiles/CryptorKeyUnitTest.xml"
                }
            }
        };
        var notExsitCryptorFileEtlSetting = new EtlSetting()
        {
            Transformation = new TransformationSetting
            {
                Cryptor = new CryptorSetting
                {
                    CryptorFile = "./CryptorUnitTest.xml",
                    HashFile = "./TestFiles/CryptorKeyUnitTest.xml"
                }
            }
        };
        var notExitsHashFileEtlSetting = new EtlSetting()
        {
            Transformation = new TransformationSetting
            {
                Cryptor = new CryptorSetting
                {
                    CryptorFile = "./TestFiles/CryptorUnitTest.xml",
                    HashFile = "./CryptorKeyUnitTest.xml"
                }
            }
        };
        var invalidHashFileEtlSetting = new EtlSetting()
        {
            Transformation = new TransformationSetting
            {
                Cryptor = new CryptorSetting
                {
                    CryptorFile = "./TestFiles/CryptorUnitTest.xml",
                    HashFile = "./TestFiles/CryptorUnitTest.xml"
                }
            }
        };
        var invalidCryptorFileEtlSetting = new EtlSetting()
        {
            Transformation = new TransformationSetting
            {
                Cryptor = new CryptorSetting
                {
                    CryptorFile = "./TestFiles/CryptorKeyUnitTest.xml",
                    HashFile = "./TestFiles/CryptorKeyUnitTest.xml"
                }
            }
        };

        return new CryptorInfoTestCase[]
        {
                new CryptorInfoTestCase("EmptyEtlSetting", emptyEtlSetting,(emptyEtlSetting, new CryptorInfo(emptyEtlSetting)),(new CryptorInfo(emptyEtlSetting), null)),
                new CryptorInfoTestCase("NullCryptoEtlSetting", nullCryptoEtlSetting,(nullCryptoEtlSetting, new CryptorInfo(nullCryptoEtlSetting)),(new CryptorInfo(nullCryptoEtlSetting), null)),
                new CryptorInfoTestCase("InvalidHashFileEtlSetting", invalidHashFileEtlSetting,(invalidHashFileEtlSetting, new CryptorInfo(invalidHashFileEtlSetting)),(new CryptorInfo(invalidHashFileEtlSetting), null)),
                new CryptorInfoTestCase("InvalidCryptorFileEtlSetting", invalidCryptorFileEtlSetting,(invalidCryptorFileEtlSetting, new CryptorInfo(invalidCryptorFileEtlSetting)),(new CryptorInfo(invalidCryptorFileEtlSetting), null)),
                new CryptorInfoTestCase("ReadCryptorSettingFile", validEtlSetting, (validEtlSetting, new CryptorInfo(emptyEtlSetting)),(new CryptorInfo(validEtlSetting), null)),
                new CryptorInfoTestCase("NotFoundCryptorSettingFile", notExsitCryptorFileEtlSetting, (notExsitCryptorFileEtlSetting, new CryptorInfo(emptyEtlSetting)),(new CryptorInfo(emptyEtlSetting), new FileNotFoundException())),
                new CryptorInfoTestCase("NotFoundCryptorSettingFile", notExitsHashFileEtlSetting, (notExitsHashFileEtlSetting, new CryptorInfo(emptyEtlSetting)),(new CryptorInfo(emptyEtlSetting), new FileNotFoundException()))
        };
    }
}
