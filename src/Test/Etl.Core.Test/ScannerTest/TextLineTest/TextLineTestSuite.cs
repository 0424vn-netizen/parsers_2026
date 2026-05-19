using Aperia.UT;
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
   [TestClass]
    public class TextLineTestSuite
    {
        [TestMethod]
        public void TextLine_Test()
        {
            TestBuilder.CreateInstance((TextLineInput construct) =>
            {
                var builder = new TextLineTestBuilder();
                builder.TextLineInput = construct;
                return builder.Build();
            }).Action((TextLine inst, TextLineInput input) =>
            {
                var obj = new TextLine(input.Text, input.Row);
                return obj.ToString();
            }).Validator<(string result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result?.Result, expect.result);
            }).TestCases(GetTestCases())
            .Run();
        }

        private TextLineTestCase[] GetTestCases()
        {
            return new TextLineTestCase[]
            {
                new TextLineTestCase("RowText", null, new TextLineInput
                    { 
                        Row = 1,
                        Text = "Test"
                    },("R.1:Test",null))
            };
        }


        [TestMethod]
        public void TextLineStaticMethod_Test()
        {
            var testCase = new TextLineTestCase[]
            {
                new TextLineTestCase("RowText", null, new TextLineInput
                    {
                        Row = 1,
                        Text = "Test"
                    },(string.Empty,null))
            };

            TestBuilder.CreateInstance((TextLineInput construct) =>
            {
                var builder = new TextLineTestBuilder();
                builder.TextLineInput = construct;
                return builder.Build();
            }).Action((TextLine inst, TextLineInput input) =>
            {
                return TextLine.End;
            }).Validator<(string result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result?.Result.Text, expect.result);
            }).TestCases(testCase)
            .Run();
        }
    }
}
