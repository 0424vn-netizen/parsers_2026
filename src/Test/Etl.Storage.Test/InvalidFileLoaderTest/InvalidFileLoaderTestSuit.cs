using Aperia.UT;
using Etl.Core.Load;
using Etl.Core.Transformation.Fields;
using Etl.Storage.Test.CsvLoaderTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Storage.Test.InvalidFileLoaderTest
{
    [TestClass]
    public class InvalidFileLoaderTestSuit
    {
        [TestMethod]
        public void InvalidFileLoader_ProcessBatch_Test()
        {
            TestBuilder.CreateInstance((NA constructParam) =>
            {
                var builder = new InvalidFileLoaderInstanceBuilder();

                return builder.Build();
            }).ActionNoResult((InvalidFileLoaderInstanceBuilder inst, (InvalidLogLoader defintion, LoaderArgs args, BatchResult? batch) param) =>
            {
                if (param.batch?.Errors?.Count() > 0)
                {
                    inst.Initalize(param.defintion, param.args);
                }
                _ = inst.ProcessBatchAsync(param.batch);
                inst.DisposeWriter();
            })
            .Validator<(string result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                if (args.batch?.Errors?.Count() > 0)
                {
                    var writer = inst.GetWriter();
                    var outputFilePath = ((FileStream)writer.BaseStream).Name;
                    var fileContent = File.ReadAllText(outputFilePath);

                    Assert.IsNotNull(writer);
                    Assert.IsTrue(!string.IsNullOrEmpty(fileContent));
                }
            })
            .TestCases(GetProcessBatch_InvalidFileLoaderTestCases())
            .Run();
        }

        private InvalidFileLoaderTestCase[] GetProcessBatch_InvalidFileLoaderTestCases()
        {
            var loaderArg = new LoaderArgs("./InvalidFileLoaderTest/InvalidUTFile.csv", new List<TransformField>()
            {
                new RecordField()
            });

            var batchRecord = new BatchResult(new List<IDictionary<string, object?>>(), new[] { "error1", "error2", "error3" }.ToList(), DateTime.Now, 0, 0, true);
            var emptyBatchRecord = new BatchResult(new List<IDictionary<string, object?>>(), new List<string>(), DateTime.Now, 0, 0, true);

            return new InvalidFileLoaderTestCase[]
            {
                new InvalidFileLoaderTestCase("ProcessBatch_CsvLoader", NA.Value,(new InvalidLogLoader(), loaderArg, batchRecord),( string.Empty, null)),
                new InvalidFileLoaderTestCase("ProcessEmptyBatch_CsvLoader", NA.Value, (new InvalidLogLoader(), loaderArg, emptyBatchRecord),( string.Empty, null))
            };
        }
    }
}
