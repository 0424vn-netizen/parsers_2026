using Etl.Core.Load;
using System.Data;
using System.Data.SqlClient;

namespace Etl.Storage;

internal class SqlBulkCopyExecutor
{
    private readonly DataTable _dataTable;
    private readonly SqlBulkCopy _sqlBulkCopy;

    public bool IsAvailable { get; private set; } = true;

    public SqlBulkCopyExecutor(SqlConnection sqlConnection, DataTable dataTable)
    {
        _dataTable = dataTable;
        _sqlBulkCopy = new SqlBulkCopy(sqlConnection)
        {
            DestinationTableName = dataTable.TableName
        };
    }

    public async Task Execute(BatchResult batch)
    {
        IsAvailable = false;
        _dataTable.Clear();

        var rows = new DataRow[batch.Batch.Count];

        for (var i = 0; i < batch.Batch.Count; i++)
        {
            var row = _dataTable.NewRow();
            foreach (var e in batch.Batch[i])
                row[e.Key] = e.Value;
            rows[i] = row;
        }

        await _sqlBulkCopy.WriteToServerAsync(rows);
        IsAvailable = true;
    }

    public void Completed()
    {
        _sqlBulkCopy.Close();
    }
}
