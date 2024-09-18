namespace AssetAllocation.Api;

public class LogDetailWithException : LogDetail
{
    public string? ExceptionMessage { get; set; }
    public string StackTrace { get; set; }

    public LogDetailWithException() : base()
    {
        ExceptionMessage = string.Empty;
        StackTrace = string.Empty;
    }

    public LogDetailWithException(string pathAndQuery, string user, List<LogParameter> parameters, string exceptionMessage, string stackTrace)
        : base(pathAndQuery, user, parameters)
    {
        ExceptionMessage = exceptionMessage;
        StackTrace = stackTrace;
    }
}
