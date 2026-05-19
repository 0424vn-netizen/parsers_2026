using Etl.Core.Load;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks.Dataflow;
using System.Xml.Serialization;

namespace Etl.Storage;

public class MongoDbLoader : Loader<MongoDbLoaderInst>
{
    [XmlAttribute]
    public string DbName { get; set; } = String.Empty;

    [XmlAttribute]
    public string CollectionName { get; set; } = String.Empty;

    [XmlAttribute]
    public int MaxConcurency { get; set; } = 4;
}

public class MongoDbLoaderInst : LoaderInst<MongoDbLoader>
{
    private readonly MongoDbConnection _dbConnection;
    private Lazy<IMongoCollection<BsonDocument>>? _lazyCollection;  
    private ActionBlock<BatchResult> _processBulkInsert = default!;  

    public MongoDbLoaderInst(MongoDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    protected override void Initalize(MongoDbLoader definition, LoaderArgs args)
    {
        _lazyCollection = new(() =>
            new MongoClient(_dbConnection.GetConnectionString())
                .GetDatabase(definition.DbName)
                .GetCollection<BsonDocument>(definition.CollectionName));

        _processBulkInsert = new (
            OnBulkInsert,
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = definition.MaxConcurency,
                BoundedCapacity = 2
            });
    }

    public override async Task ProcessBatchAsync(BatchResult batch)
    {
        await _processBulkInsert.SendAsync(batch);

        if (batch.IsLast)
         await _processBulkInsert.Completion;
    }

    private async Task OnBulkInsert(BatchResult result)
    {
        if (result?.Batch == null || result.Batch.Count == 0)
            return;

        await _lazyCollection!.Value.InsertManyAsync(result.Batch.Select(e => new BsonDocument(e)));

        if (result?.IsLast ?? true)
            _processBulkInsert!.Complete();
    }
}
