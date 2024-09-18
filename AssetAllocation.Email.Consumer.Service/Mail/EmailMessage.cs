using System;

namespace AssetAllocation.Email.Consumer.Service;

public class EmailMessage
{
    public EmailMessage()
    {
        To = string.Empty;
        Subject = string.Empty;
        Body = string.Empty;
        IsHtml = false;
    }
    public EmailMessage(string to, string subject, string body, bool isHtml)
    {
        To = to;
        Subject = subject;
        Body = body;
        IsHtml = isHtml;
    }

    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool IsHtml { get; set; }

}