using Fiap.FileCut.Core.Interfaces.Adapters;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Fiap.FileCut.Core.Adapters
{
    public class EmailNotifyAdapter(ILogger<EmailNotifyAdapter> logger, IOptions<SmtpProperties> optSmtpProperties) : INotifyAdapter
    {
        private readonly ILogger<EmailNotifyAdapter> _logger = logger;
        private readonly SmtpProperties _smtpProperties = optSmtpProperties.Value;

        public async Task NotifyAsync<T>(NotifyContext<T> notifyContext)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpProperties.Username),
                Subject = notifyContext.Context?.ToString(),
                Body = "body",
                IsBodyHtml = false,
            };
            mailMessage.To.Add(notifyContext.Mail);

            try
            {
                await SendEmailAsync(mailMessage);
                _logger.LogDebug("Sending email to {UserId} ", notifyContext.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {UserId}", notifyContext.UserId);
            }
        }

        public async Task SendEmailAsync(MailMessage message)
        {
            using var client = new SmtpClient(_smtpProperties.Server, _smtpProperties.Port) { EnableSsl = _smtpProperties.EnableSsl };
            if (_smtpProperties.Username == null && _smtpProperties.Password == null)
            {
                client.Credentials = new NetworkCredential(_smtpProperties.Username, _smtpProperties.Password);
            }
            await client.SendMailAsync(message);
        }
    }
}
