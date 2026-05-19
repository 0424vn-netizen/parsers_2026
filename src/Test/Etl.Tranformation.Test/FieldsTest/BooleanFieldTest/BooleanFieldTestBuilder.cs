using Etl.Tranformation.Actions;
using Microsoft.Extensions.DependencyInjection;

namespace Etl.Tranformation.Test.FieldsTest.BooleanFieldTest
{
    public class BooleanFieldTestBuilder
    {
        private readonly ServiceCollection _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetupTestBuilder"/> class.
        /// </summary>
        public BooleanFieldTestBuilder()
        {
            _instance = new ServiceCollection();
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public IServiceCollection Build()
        {
            _instance.AddTransient<BooleanActionInst>();
            return _instance;
        }
    }
}