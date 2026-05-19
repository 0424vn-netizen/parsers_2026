using Etl.Core.Load;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Core.Test.LoadTest.BatchResultTest
{
    public class BatchResultTestBuilder
    {
        private BatchResult _instance;
        public BatchInput BatchInput;

        public BatchResultTestBuilder()
        {
        }

        public BatchResult Build()
        {
            return _instance;
        }
    }

    public class BatchInput
    {
        public Records Batch { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        public DateTime StartAt { get; set; }
        public int TotalTransformSuccess { get; set; }
        public int TotalTransformErrors { get; set; }
        public bool IsLast { get; set; }
    }
}
