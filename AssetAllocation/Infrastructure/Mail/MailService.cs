using System.Net;
using System.Net.Mail;
using AssetAllocation.Api.Infrastructure.Mail;

namespace AssetAllocation.Api;

public interface IMailService
{
    Task SendMailAsync(EmailMessage emailMessage, CancellationToken cancellationToken);
}
public class MailService : IMailService
{
    private readonly SmtpConfiguration _smtpConfiguration;

    public MailService(IConfiguration configuration)
    {
        _smtpConfiguration = configuration.GetSection("SmtpConfiguration").Get<SmtpConfiguration>() 
            ?? throw new ArgumentException("Smtp configuration is not valid");
    }
    public async Task SendMailAsync(EmailMessage emailMessage, CancellationToken cancellationToken)
    {
        using var smtpClient = new SmtpClient(_smtpConfiguration.Host, _smtpConfiguration.Port)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_smtpConfiguration.Username, _smtpConfiguration.Password)
        };

        MailAddress from = _smtpConfiguration.FromName != null ? new(_smtpConfiguration.FromAddress, _smtpConfiguration.FromName) : new(_smtpConfiguration.FromAddress);
        MailAddress to = new(emailMessage.To);
        MailMessage mailMessage = new(from, to)
        {
            Subject = emailMessage.Subject,
            Body = emailMessage.Body,
            IsBodyHtml = emailMessage.IsHtml,
        };

        await smtpClient.SendMailAsync(mailMessage, cancellationToken);
    }
}
