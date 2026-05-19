using Aperia.UT;
using Etl.Core.Extraction;
using Etl.Core.Scanner;
using Etl.Core.Test.ScannerTest.TextLineTest;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Core.Test.ScannerTest.TextBlockTest.GetIndexder
{
    [TestClass]
    public class TextBlockTestSuite
    {
        [TestMethod]
        public void GetValueByIExtractedInfo_Test()
        {
            TestBuilder.CreateInstance((TextBlockTestBuilder construct) =>
            {
                var builder = new TextBlockTestBuilder();

                return builder.Build();
            }).Action((TextBlock inst, (TextBlockTestBuilder construct, List<TextLine> lines) arg) =>
            {
                var obj = new TextBlock(arg.lines);

                return obj[0];
            }).Validator<(string result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result?.Result.Text, expect.result);
            }).TestCases(GetTestCases())
            .Run();
        }

        private TextBlockTestCase[] GetTestCases()
        {
            return new TextBlockTestCase[]
            {
                 new TextBlockTestCase("Case_GetIndexer", new TextBlockTestBuilder(),
                (GetConstructByCaseName("Case_GetIndexer"), new List<TextLine>()
                {
                    new TextLine("text v",1)
                })
                , ("text v",null))
            };
        }

        private TextBlockTestBuilder GetConstructByCaseName(string caseName)
        {
            var construct = new TextBlockTestBuilder();

            switch (caseName)
            {
                case "Case_GetIndexer":
                    {
                        construct.Row = 0;
                        construct.StartColumn = 1;
                        construct.Length = 10;
                        break;
                    }
            }

            return construct;
        }
    }
}
