using Aperia.UT;
using Etl.Core.Load;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etl.Storage.Test.MongoDbLoaderTest
{
    public class MongoDbLoaderTestCase : ITestCaseParam<MongoDbConnection, (MongoDbLoader defintion, LoaderArgs args, BatchResult? batch), (string result, Exception? ex)>
    {
        public MongoDbLoaderTestCase(string caseName, MongoDbConnection constructArgs, (MongoDbLoader defintion, LoaderArgs args, BatchResult? batch) ActionArgs, (string result, Exception? ex) expectation)
        {
            this.CaseName = caseName;
            this.ConstructArgs = constructArgs;
            this.Expectation = expectation;
            this.ActionArgs = ActionArgs;
        }

        public string CaseName { get; set; }
        public MongoDbConnection ConstructArgs { get; set; }
        public (MongoDbLoader defintion, LoaderArgs args, BatchResult? batch) ActionArgs { get; set; }
        public (string result, Exception? ex) Expectation { get; set; }
    }
}
