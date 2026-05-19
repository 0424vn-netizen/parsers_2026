using Aperia.UT;
using Etl.Core.Load;
using Etl.Core.Transformation.Fields;
using Etl.Storage.Test.InvalidFileLoaderTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Etl.Storage.Test.MongoDbLoaderTest
{
    [TestClass]
    public class MongoDbLoaderTestSuit
    {
        [TestMethod]
        public void MongoDbLoader_ProcessBatch_Test()
        {
            TestBuilder.CreateInstance((MongoDbConnection constructParam) =>
            {
                var builder = new MongoDbLoaderInstanceBuilder(constructParam);

                return builder.Build();
            }).ActionNoResult((MongoDbLoaderInstanceBuilder inst, (MongoDbLoader defintion, LoaderArgs args, BatchResult? batch) param) =>
            {
                if (!string.IsNullOrEmpty(param.defintion.DbName))
                {
                    inst.Initalize(param.defintion, param.args);
                }
                else
                {
                    inst.InitalizeForUT(param.defintion, param.args);
                }
                _ = inst.ProcessBatchAsync(param.batch);
            })
            .Validator<(string result, Exception? ex)>((inst, construc, args, result, expect) =>
            {
                var processBulkInsert = this.GetType()?.BaseType?.GetField("_processBulkInsert", BindingFlags.Instance | BindingFlags.NonPublic);
                if (processBulkInsert != null)
                {
                    var processBulkInsertValue = (ActionBlock<BatchResult>)processBulkInsert.GetValue(inst);
                    Assert.IsTrue(processBulkInsertValue?.Completion.IsCompletedSuccessfully);
                }
            })
            .TestCases(GetProcessBatch_MongoDbLoaderTestCases())
            .Run();
        }

        private MongoDbLoaderTestCase[] GetProcessBatch_MongoDbLoaderTestCases()
        {
            var loaderArg = new LoaderArgs(string.Empty, new List<TransformField>()
            {
                new RecordField()
            });

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
            var emptyBatchRecord = new BatchResult(new List<IDictionary<string, object?>>(), new List<string>(), DateTime.Now, 0, 0, true);

            return new MongoDbLoaderTestCase[]
            {
                new MongoDbLoaderTestCase("ProcessBatch_MongoDbLoader", new MongoDbConnection(), (new MongoDbLoader(), loaderArg, batchRecord),( string.Empty, null)),
                new MongoDbLoaderTestCase("ProcessBatch_DefineDbName_MongoDbLoader", new MongoDbConnection(){DbName = "DbName"}, (new MongoDbLoader(){DbName = "DbName"}, loaderArg, batchRecord),( string.Empty, null)),
                new MongoDbLoaderTestCase("ProcessBatch_EmptyBatchRecords_MongoDbLoader", new MongoDbConnection(), (new MongoDbLoader(), loaderArg, emptyBatchRecord),( string.Empty, null))
            };
        }
    }
}
