using Etl.Core.Load;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Serialization;

namespace Etl.Storage;

public class SqlServerLoader : Loader<SqlServerLoaderInst>
{
    [XmlAttribute]
    public string TableName { get; set; } = string.Empty;

    [XmlAttribute]
    public int MaxConcurency { get; set; } = 2;
}

public class SqlServerLoaderInst : LoaderInst<SqlServerLoader>
{
    private readonly SqlConnection _sqlConnection;
    private readonly List<SqlBulkCopyExecutor> _executors = new();
    private readonly List<Task> _tasks = new();

    public SqlServerLoaderInst(SqlBulkCopyConnection sqlBulkCopyConnection)
    {
        _sqlConnection = new SqlConnection(sqlBulkCopyConnection.ConnectionString);
    }

    protected override void Initalize(SqlServerLoader definition, LoaderArgs args)
    {
        _sqlConnection.Open();

        var dbColumnNames = GetDbColumnNames(_sqlConnection, definition.TableName);
        var concurency = Math.Max(1, definition.MaxConcurency);

        for (var i = 0; i < concurency; i++)
        {
            var dataTable = new DataTable(definition.TableName);
            foreach (var col in dbColumnNames)
                dataTable.Columns.Add(col);
            _executors.Add(new SqlBulkCopyExecutor(_sqlConnection, dataTable));
        }
    }

    public override async Task ProcessBatchAsync(BatchResult batch)
    {
        var executor = _executors.FirstOrDefault(e => e.IsAvailable);
        while (executor == null)
        {
            _tasks.Remove(await Task.WhenAny(_tasks));
            executor = _executors.FirstOrDefault(e => e.IsAvailable);
        }

        _tasks.Add(executor.Execute(batch));

        if (batch.IsLast)
            await Task.WhenAll(_tasks);
    }

    private static string[] GetDbColumnNames(SqlConnection sqlConnection, string tableName)
    {
        var cmd = sqlConnection.CreateCommand();
        cmd.CommandText = $"Select * From [{tableName}] Where 1=2";

        using var reader = cmd.ExecuteReader();
        var schema = reader.GetSchemaTable();
        var columns = new string[schema.Rows.Count];

        for (var i = 0; i < schema.Rows.Count; i++)
        {
            var r = schema.Rows[i];
            columns[i] = r["ColumnName"] as string ?? string.Empty;
        }

        return columns;
    }

    protected override void OnCompleted()
    {
        foreach (var e in _executors)
            e.Completed();
        _sqlConnection.Dispose();
    }
}
