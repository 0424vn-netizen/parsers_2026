using Etl.Core.Load;
using Etl.Tranformation.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Storage.Test.CsvLoaderTest
{
    public class CsvLoaderInstanceBuilder : CsvLoaderInst
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvLoaderInstanceBuilder"/> class.
        /// </summary>
        public CsvLoaderInstanceBuilder()
        {
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public CsvLoaderInstanceBuilder Build()
        {
            return this;
        }

        public new void Initalize(CsvLoader defintion, LoaderArgs args)
        {
            base.Initalize(defintion, args);
        }

        public void DisposeWriter()
        {
            base.OnCompleted();
        }

        public StreamWriter? GetWriter()
        {
            return Writer;
        }
    }
}
