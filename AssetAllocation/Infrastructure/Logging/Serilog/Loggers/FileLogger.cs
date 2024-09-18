using Serilog;

namespace AssetAllocation.Api;

public class FileLogger : LoggerServiceBase
{
    public FileLogger(IConfiguration configuration)
    {
        FileLogConfiguration fileLogConfiguration = configuration.GetSection("SerilogConfigurations:FileLogConfiguration").Get<FileLogConfiguration>() 
            ?? throw new Exception(SerilogMessages.NullOptionMessage);

            string logFilePath = string.Format("{0}{1}", Directory.GetCurrentDirectory() + fileLogConfiguration.FolderPath, "log.txt");

            Logger = new LoggerConfiguration().WriteTo.File(
                logFilePath, 
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                fileSizeLimitBytes: 50000000,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}")
                .CreateLogger();
    }   
}
