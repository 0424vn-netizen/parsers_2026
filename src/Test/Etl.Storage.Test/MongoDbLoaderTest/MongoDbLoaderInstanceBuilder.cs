using Etl.Core.Load;
using Etl.Storage.Test.InvalidFileLoaderTest;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Etl.Storage.Test.MongoDbLoaderTest
{
    public class MongoDbLoaderInstanceBuilder : MongoDbLoaderInst
    {
        public MongoDbLoaderInstanceBuilder(MongoDbConnection dbConnection) : base(dbConnection)
        {
        }

        public MongoDbLoaderInstanceBuilder Build()
        {
            return this;
        }

        public new void Initalize(MongoDbLoader defintion, LoaderArgs args)
        {
            base.Initalize(defintion, args);
        }

        public void InitalizeForUT(MongoDbLoader defintion, LoaderArgs args)
        {
            base.Initalize(defintion, args);
            var _lazyCollection = this.GetType()?.BaseType?.GetField("_lazyCollection", BindingFlags.Instance | BindingFlags.NonPublic);

            if(_lazyCollection != null)
            {
                var mongoDbMock = new Mock<IMongoCollection<BsonDocument>>();
                mongoDbMock.Setup(m=>m.InsertManyAsync(It.IsAny<IEnumerable<BsonDocument>>(), It.IsAny<InsertManyOptions>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(0));
                _lazyCollection.SetValue(this, new Lazy<IMongoCollection<BsonDocument>>(mongoDbMock.Object));
            }
        }
    }
}
