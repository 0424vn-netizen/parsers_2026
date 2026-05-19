using Aperia.UT;
using Etl.Core.Load;
using Etl.Core.Transformation.Fields;
using Etl.Tranformation;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Storage.Test.CsvLoaderTest
{
    [TestClass]
    public class CsvLoaderTestSuit
    {
        [TestMethod]
        public void CsvLoader_Initalize_Test()
        {
            TestBuilder.CreateInstance((NA _) =>
            {
                var builder = new CsvLoaderInstanceBuilder();

                return builder.Build();
            }).ActionNoResult((CsvLoaderInstanceBuilder inst, (CsvLoader defintion, LoaderArgs args, BatchResult? batch) param) =>
            {
                inst.Initalize(param.defintion, param.args);
                inst.DisposeWriter();
            })
            .Validator<(string result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                var fieldOrders = inst.GetType()?.BaseType?.GetField("_fieldOrders", BindingFlags.Instance | BindingFlags.NonPublic);
                var fieldOrdersValue = (Dictionary<string, int>)fieldOrders.GetValue(inst);
                var delimiter = inst.GetType()?.BaseType?.GetField("_delimiter", BindingFlags.Instance | BindingFlags.NonPublic);
                var delimiterValue = (string)delimiter.GetValue(inst);

                var expectedFieldCount = args.defintion?.Fields.Count == 0 ? args.args.Fields.Count : args.defintion?.Fields.Count;

                Assert.AreEqual(args.defintion?.Delimiter, delimiterValue);
                Assert.AreEqual(expectedFieldCount, fieldOrdersValue?.Count);
            })
            .TestCases(GetInitialize_CsvLoaderTestCases())
            .Run();
        }

        private CsvLoaderTestCase[] GetInitialize_CsvLoaderTestCases()
        {
            var loaderArg = new LoaderArgs("./CsvLoaderTest/CsvUTFile.csv", new List<TransformField>()
            {
                new RecordField()
            });
            var csvLoader = new CsvLoader
            {
                Delimiter = ",",
                Fields = new[] { "field1", "field2", "field3" }.ToList()
            };
            var emptyCsvLoader = new CsvLoader();

            return new CsvLoaderTestCase[]
            {
                new CsvLoaderTestCase("Initialize_EmptyCsvLoader", NA.Value, (emptyCsvLoader, loaderArg, null),( string.Empty, null)),
                new CsvLoaderTestCase("Initialize_CsvLoader", NA.Value, (csvLoader, loaderArg, null),( string.Empty, null))
            };
        }

        [TestMethod]
        public void CsvLoader_ProcessBatch_Test()
        {
            TestBuilder.CreateInstance((NA _) =>
            {
                var builder = new CsvLoaderInstanceBuilder();

                return builder.Build();
            }).ActionNoResult((CsvLoaderInstanceBuilder inst, (CsvLoader defintion, LoaderArgs args, BatchResult? batch) param) =>
            {
                inst.Initalize(param.defintion, param.args);
                _ = inst.ProcessBatchAsync(param.batch);
                inst.DisposeWriter();
            })
            .Validator<(string result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                var writer = inst.GetWriter();
                var outputFilePath = ((FileStream)writer.BaseStream).Name;
                var fileContent = File.ReadAllText(outputFilePath);

                Assert.IsNotNull(writer);
                Assert.IsTrue(!string.IsNullOrEmpty(fileContent));
            })
            .TestCases(GetProcessBatch_CsvLoaderTestCases())
            .Run();
        }

        private CsvLoaderTestCase[] GetProcessBatch_CsvLoaderTestCases()
        {
            var loaderArg = new LoaderArgs("./CsvLoaderTest/CsvUTFile.csv", new List<TransformField>()
            {
                new RecordField()
            });
            var csvLoader = new CsvLoader
            {
                Delimiter = ",",
                Fields = new[] { "field1", "field2", "field3" }.ToList()
            };
            var batchRecord = new BatchResult(new List<IDictionary<string, object?>>()
            {
                new Dictionary<string, object?>()
                {
                    { "field1", 123 }
                },
                new Dictionary<string, object?>()
                {
                    { "field2", "abc" }
                },
                new Dictionary<string, object?>()
                {
                    { "field3", false }
                }
            }, new List<string>(), DateTime.Now, 0, 0, true);

            return new CsvLoaderTestCase[]
            {
                new CsvLoaderTestCase("ProcessBatch_CsvLoader", NA.Value, (csvLoader, loaderArg, batchRecord),( string.Empty, null))
            };
        }

    }
}
