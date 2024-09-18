namespace AssetAllocation.Api;

public class LogDetail
{
    public string PathAndQuery { get; set; }
    public string User { get; set; }
    public List<LogParameter> Parameters { get; set; }

    public LogDetail()
    {
        PathAndQuery = string.Empty;
        User = string.Empty;
        Parameters = [];
    }

    public LogDetail(string pathAndQuery, string user, List<LogParameter> parameters)
    {
        PathAndQuery = pathAndQuery;
        User = user;
        Parameters = parameters;
    }
}
