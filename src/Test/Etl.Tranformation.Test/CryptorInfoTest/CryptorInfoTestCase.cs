namespace Etl.Tranformation.Test.CryptorInfoTest;

using Aperia.UT;
using Etl.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CryptorInfoTestCase : ITestCaseParam<EtlSetting, (EtlSetting param, CryptorInfo), (CryptorInfo result, Exception? ex)>
{
    public CryptorInfoTestCase(string caseName, EtlSetting constructArgs, (EtlSetting param, CryptorInfo) ActionArgs, (CryptorInfo result, Exception? ex) expectation)
    {
        this.CaseName = caseName;
        this.ConstructArgs = constructArgs;
        this.Expectation = expectation;
        this.ActionArgs = ActionArgs;
    }
    public string CaseName { get; set; }
    public EtlSetting ConstructArgs { get; set; }
    public (EtlSetting param, CryptorInfo) ActionArgs { get; set; }
    public (CryptorInfo result, Exception? ex) Expectation { get; set; }
}