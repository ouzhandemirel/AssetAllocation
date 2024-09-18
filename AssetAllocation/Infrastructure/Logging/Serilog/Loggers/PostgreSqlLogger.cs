using NpgsqlTypes;
using Serilog;
using Serilog.Configuration;
using Serilog.Sinks.PostgreSQL;

namespace AssetAllocation.Api;

public class PostgreSqlLogger : LoggerServiceBase
{
    public PostgreSqlLogger(IConfiguration configuration)
    {
        PostgreSqlLogConfiguration postgreSqlLogConfiguration = configuration.GetSection("SerilogConfigurations:PostgreSqlLogConfiguration").Get<PostgreSqlLogConfiguration>()
             ?? throw new Exception(SerilogMessages.NullOptionMessage);

        Logger = new LoggerConfiguration().WriteTo.PostgreSQL(
            connectionString: postgreSqlLogConfiguration.ConnectionString,
            tableName: postgreSqlLogConfiguration.TableName,
            schemaName: postgreSqlLogConfiguration.SchemaName,
            needAutoCreateTable: postgreSqlLogConfiguration.AutoCreateTable,
            columnOptions: new Dictionary<string, ColumnWriterBase>
            {
                {"message", new RenderedMessageColumnWriter(NpgsqlDbType.Text)},
                {"message_template", new MessageTemplateColumnWriter(NpgsqlDbType.Text)},
                {"level", new LevelColumnWriter(true , NpgsqlDbType.Varchar)},
                {"time_stamp", new TimestampColumnWriter(NpgsqlDbType.Timestamp)},
                {"exception", new ExceptionColumnWriter(NpgsqlDbType.Text)},
                {"log_event", new LogEventSerializedColumnWriter(NpgsqlDbType.Json)},
            })
            .Enrich.FromLogContext()
            .MinimumLevel.Information()
            .CreateLogger();
    }
}
