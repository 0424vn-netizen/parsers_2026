using Etl.Core.Load;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Storage.Test.InvalidFileLoaderTest
{
    public class InvalidFileLoaderInstanceBuilder : InvalidLogLoaderInst
    {
        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns></returns>
        public InvalidFileLoaderInstanceBuilder Build()
        {
            return this;
        }

        public new void Initalize(InvalidLogLoader defintion, LoaderArgs args)
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
