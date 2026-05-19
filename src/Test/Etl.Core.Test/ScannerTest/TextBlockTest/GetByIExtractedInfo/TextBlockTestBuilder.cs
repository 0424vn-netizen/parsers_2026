using Etl.Core.Extraction;
using Etl.Core.Scanner;
using Etl.Core.Test.ScannerTest.TextLineTest;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Core.Test.ScannerTest.TextBlockTest.GetByIExtractedInfo
{
    public class TextBlockTestBuilder
    {
        private TextBlock _instance;

        public List<TextLine> Lines;

        public Mock<IExtractedInfo> MockIExtractedInfo;

        public TextBlockTestBuilder()
        {
            MockIExtractedInfo = new Mock<IExtractedInfo>();
        }

        public TextBlock Build()
        {
            return _instance;
        }

    }
}
