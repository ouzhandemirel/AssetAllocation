namespace AssetAllocation.Api;

public class SmtpConfiguration
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public bool EnableSsl { get; set; }
    public string FromAddress { get; set; }
    public string FromName { get; set; }

    public SmtpConfiguration()
    {
        Host = string.Empty;
        Username = string.Empty;
        Password = string.Empty;
        FromAddress = string.Empty;
        FromName = string.Empty;
    }

    public SmtpConfiguration(string host, int port, string username, string password, bool enableSsl, string fromAddress, string fromName)
    {
        Host = host;
        Port = port;
        Username = username;
        Password = password;
        EnableSsl = enableSsl;
        FromAddress = fromAddress;
        FromName = fromName;
    }
}
