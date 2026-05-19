using Aperia.UT;
using Etl.Core.Load;
using Etl.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Etl.Core.Test.LoadTest.BatchResultTest
{
    [TestClass]
    public class BatchResultTestSuite
    {
        [TestMethod]
        public void BatchResult_Test()
        {
            TestBuilder.CreateInstance((BatchInput construct) =>
            {
                var builder = new BatchResultTestBuilder();
                builder.BatchInput = construct;
                return builder.Build();
            }).Action((BatchResult inst, BatchInput input) =>
            {
                var obj = new BatchResult(input.Batch, input.Errors, input.StartAt, input.TotalTransformSuccess, input.TotalTransformErrors, input.IsLast);
                return obj.ToString();
            }).Validator<(string result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                Assert.AreEqual(result?.Result.Contains(expect.result),true);
            }).TestCases(GetTestCases())
            .Run();
        }

        private BatchResultTestCase[] GetTestCases()
        {
            return new BatchResultTestCase[]
            {
                new BatchResultTestCase("HasError", null, new BatchInput
                { 
                 Batch = new List<IDictionary<string, object?>>(),
                 Errors = new List<string>{ "Error1", "Error2" },
                 IsLast = true,
                 StartAt = DateTime.Now,
                 TotalTransformSuccess = 0,
                 TotalTransformErrors = 2                 
                },("errors: 2",null)),
                new BatchResultTestCase("NoError", null, new BatchInput
                {
                 Batch = new List<IDictionary<string, object?>>(),
                 Errors = new List<string>(),
                 IsLast = true,
                 StartAt = DateTime.Now,
                 TotalTransformSuccess = 0,
                 TotalTransformErrors = 0
                },("errors: 0",null))
            };
        }
    }
}
