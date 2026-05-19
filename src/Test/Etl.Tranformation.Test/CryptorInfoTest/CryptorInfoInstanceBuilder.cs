namespace Etl.Tranformation.Test.CryptorInfoTest;

using Etl.Core.Settings;
using Etl.Tranformation.Actions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CryptorInfoInstanceBuilder
{
    private CryptorInfo _instance;
    public EtlSetting mockEltSetting;

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptorInfoInstanceBuilder"/> class.
    /// </summary>
    public CryptorInfoInstanceBuilder()
    {
    }

    /// <summary>
    /// Builds this instance.
    /// </summary>
    /// <returns></returns>
    public CryptorInfo Build()
    {
        return _instance;
    }
}