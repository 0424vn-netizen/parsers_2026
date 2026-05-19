using Etl.Tranformation.Actions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Tranformation.Test.FieldsTest.DateFieldTest
{
    public class DateFieldTestBuilder
    {
        private readonly ServiceCollection _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupTestBuilder"/> class.
        /// </summary>
        public DateFieldTestBuilder()
        {
            _instance = new ServiceCollection();
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public IServiceCollection Build()
        {
            _instance.AddTransient<DateActionInst>();
            return _instance;
        }
    }
}