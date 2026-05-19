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

namespace Etl.Core.Test.ScannerTest.TextBlockTest.GetByIExtractedInfo
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
                builder.Lines = construct.Lines;

                return builder.Build();
            }).Action((TextBlock inst, (TextBlockTestBuilder construct, List<TextLine> lines) arg) =>
            {
                var obj = new TextBlock(arg.lines);
                return obj.GetValue(arg.construct.MockIExtractedInfo.Object);
            }).Validator<(string result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result?.Result, expect.result);
            }).TestCases(GetTestCases())
            .Run();
        }

        private TextBlockTestCase[] GetTestCases()
        {
            return new TextBlockTestCase[]
            {
                new TextBlockTestCase("Case_StartRowEqualEndRow_TextLen0_ReturnEmpty", new TextBlockTestBuilder(),
                (new TextBlockTestBuilder(), new List<TextLine>()
                {
                    new TextLine(string.Empty,1)
                })
                , (string.Empty,null)),

                 new TextBlockTestCase("Case_StartRowEqualEndRow_TextLenMoreThan1_GreaterTextLen_ReturnValue", new TextBlockTestBuilder(),
                (GetConstructByCaseName("Case_StartRowEqualEndRow_TextLenMoreThan1_GreaterTextLen_ReturnValue"), new List<TextLine>()
                {
                    new TextLine("text v",1)
                })
                , ("v",null)),

                 new TextBlockTestCase("Case_StartRowEqualEndRow_TextLenMoreThan1_EndGreaterThanStartIndex_ReturnValue", new TextBlockTestBuilder(),
                (GetConstructByCaseName("Case_StartRowEqualEndRow_TextLenMoreThan1_EndGreaterThanStartIndex_ReturnValue"), new List<TextLine>()
                {
                    new TextLine("test valu ",1)
                })
                , ("valu",null)),

                new TextBlockTestCase("Case_StartRowEqualEndRow_TextLenMoreThan1_StartIndexLessThanIndex_ReturnValue", new TextBlockTestBuilder(),
                (GetConstructByCaseName("Case_StartRowEqualEndRow_TextLenMoreThan1_StartIndexLessThanIndex_ReturnValue"), new List<TextLine>()
                {
                    new TextLine("t  ext valu  text value text value",1)
                })
                , ("ext val",null)),

                new TextBlockTestCase("Case_StartRowEqualEndRow_TextLenMoreThan1_ReturnEmpty", new TextBlockTestBuilder(),
                (GetConstructByCaseName("Case_StartRowEqualEndRow_TextLenMoreThan1_ReturnEmpty"), new List<TextLine>()
                {
                    new TextLine("t  ext",1)
                })
                , (string.Empty,null)),

                 new TextBlockTestCase("Case_StartRowNotEqualEndRow_TextLenMoreThan1_ReturnEmpty", new TextBlockTestBuilder(),
                (GetConstructByCaseName("Case_StartRowNotEqualEndRow_TextLenMoreThan1_ReturnEmpty"), new List<TextLine>()
                {
                    new TextLine("text value",1)
                })
                , (string.Empty,null))

            };
        }

        private TextBlockTestBuilder GetConstructByCaseName(string caseName)
        {
            var construct = new TextBlockTestBuilder();

            switch (caseName)
            {
                case "Case_StartRowEqualEndRow_TextLen0_ReturnEmpty":
                    {
                        return construct;
                    }

                case "Case_StartRowEqualEndRow_TextLenMoreThan1_GreaterTextLen_ReturnValue":
                    {
                        construct.MockIExtractedInfo.Setup(x => x.From).Returns((0, 5));
                        construct.MockIExtractedInfo.Setup(x => x.To).Returns((0, 10));
                        break;
                    }

                case "Case_StartRowEqualEndRow_TextLenMoreThan1_EndGreaterThanStartIndex_ReturnValue":
                    {
                        construct.MockIExtractedInfo.Setup(x => x.From).Returns((0, 5));
                        construct.MockIExtractedInfo.Setup(x => x.To).Returns((0, 10));
                        break;
                    }

                case "Case_StartRowEqualEndRow_TextLenMoreThan1_StartIndexLessThanIndex_ReturnValue":
                    {
                        construct.MockIExtractedInfo.Setup(x => x.From).Returns((0, 1));
                        construct.MockIExtractedInfo.Setup(x => x.To).Returns((0, 10));
                        break;
                    }

                case "Case_StartRowEqualEndRow_TextLenMoreThan1_ReturnEmpty":
                    {
                        construct.MockIExtractedInfo.Setup(x => x.From).Returns((0, 10));
                        construct.MockIExtractedInfo.Setup(x => x.To).Returns((0, 15));
                        break;
                    }

                case "Case_StartRowNotEqualEndRow_TextLenMoreThan1_ReturnEmpty":
                    {
                        construct.MockIExtractedInfo.Setup(x => x.From).Returns((0, 10));
                        construct.MockIExtractedInfo.Setup(x => x.To).Returns((1, 15));
                        break;
                    }
            }

            return construct;
        }
    }
}
