using Etl.Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.CryptorInfoTest
{
    public class SetupTestBuilder
    {
        private ServiceCollection _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupTestBuilder"/> class.
        /// </summary>
        public SetupTestBuilder()
        {
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public ServiceCollection Build()
        {
            _instance = new ServiceCollection();
            return _instance;
        }
    }
}
