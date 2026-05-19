using System.Data.SqlClient;

namespace Etl.Storage;

public class SqlBulkCopyConnection
{
    public string ConnectionString { get; set; } = string.Empty;

    public SqlBulkCopyConnection(string connectionString)
    {
        ConnectionString = connectionString;
    }   
}
