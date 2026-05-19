using Etl.Tranformation.Actions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.FieldsTest.FloatFieldTest
{
    public class FloatFieldTestBuilder
    {
        private readonly ServiceCollection _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupTestBuilder"/> class.
        /// </summary>
        public FloatFieldTestBuilder()
        {
            _instance = new ServiceCollection();
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public IServiceCollection Build()
        {
            _instance.AddTransient<FloatActionInst>();
            return _instance;
        }
    }
}
