using Etl.Tranformation.Actions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.FieldsTest.HashFieldTest
{
    public class HashFieldTestBuilder
    {
        private readonly ServiceCollection _instance;
        public CryptorInfo _cryptoInfo;
        /// <summary>
        /// Initializes a new instance of the <see cref="SetupTestBuilder"/> class.
        /// </summary>
        public HashFieldTestBuilder()
        {
            _instance = new ServiceCollection();
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public IServiceCollection Build()
        {
            _instance.AddSingleton(_cryptoInfo);
            _instance.AddTransient<ICryptorInfo>(sp => _cryptoInfo);
            _instance.AddTransient<HashActionInst>();
            return _instance;
        }
    }
}
