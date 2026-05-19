using Etl.Core.Load;
using Etl.Core.Scanner;
using Etl.Core.Test.LoadTest.BatchResultTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Core.Test.ScannerTest.TextLineTest
{
    public class TextLineTestBuilder
    {
        private TextLine _instance;
        public TextLineInput TextLineInput;

        public TextLineTestBuilder()
        {
        }

        public TextLine Build()
        {
            return _instance;
        }
    }

    public class TextLineInput {
        public string Text { get; set; }
        public int Row { get; set; }
    }
}
