namespace AssetAllocation.Api;

public class PostgreSqlLogConfiguration
{
    public string ConnectionString { get; set; }
    public string SchemaName { get; set; }
    public string TableName { get; set; }
    public bool AutoCreateTable { get; set; }

    public PostgreSqlLogConfiguration()
    {
        ConnectionString = string.Empty;
        TableName = string.Empty;
        SchemaName = string.Empty;
    }
    public PostgreSqlLogConfiguration(string connectionString, string schemaName, string tableName, bool autoCreateTable)
    {
        ConnectionString = connectionString;
        SchemaName = schemaName;
        TableName = tableName;
        AutoCreateTable = autoCreateTable;
    }

}
