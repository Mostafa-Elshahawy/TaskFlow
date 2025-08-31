using System.Net;
using System.Net.Mail;
using TaskFlow.Domain.Constants;
using TaskFlow.Domain.Repositories;

namespace TaskFlow.Infrastructure.Repositories;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpSettings _settings;

    public SmtpEmailSender(SmtpSettings settings)
    {
        _settings = settings;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(_settings.Username, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        var mailMessage = new MailMessage(_settings.From, to, subject, body)
        {
            IsBodyHtml = true
        };

        await client.SendMailAsync(mailMessage);
    }
}