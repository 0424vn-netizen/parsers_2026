using Etl.Core.Extraction;
using Etl.Core.Scanner;
using Etl.Core.Test.ScannerTest.TextLineTest;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Core.Test.ScannerTest.TextBlockTest.GetByParameters
{
    public class TextBlockTestBuilder
    {
        private TextBlock _instance;

        public int Row { get; set; }
        public int StartColumn { get; set; }
        public int Length { get; set; }

        public TextBlockTestBuilder()
        {
           
        }

        public TextBlock Build()
        {
            return _instance;
        }

    }
}
