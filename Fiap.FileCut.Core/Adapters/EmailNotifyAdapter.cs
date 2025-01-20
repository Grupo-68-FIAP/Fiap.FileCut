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

        public Task NotifyAsync<T>(NotifyContext<T> notifyContext)
        {
            if (notifyContext.Context is MailMessage mailMessage)
            {
                return SendMailMessageAsync(new NotifyContext<MailMessage>(mailMessage, notifyContext.UserId));
            }
            else if (notifyContext.Context is FileCutMailMessage fileCutMailMessage)
            {
                return SendFileCutMailMessageAsync(new NotifyContext<FileCutMailMessage>(fileCutMailMessage, notifyContext.UserId));
            }
            return Task.CompletedTask;
        }

        public async Task SendFileCutMailMessageAsync(NotifyContext<FileCutMailMessage> notifyContext)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpProperties.Username),
                Subject = notifyContext.Context.Subject,
                Body = notifyContext.Context.Body,
                IsBodyHtml = notifyContext.Context.IsBodyHtml
            };
            mailMessage.To.Add(notifyContext.Context.To);

            await SendEmailAsync(mailMessage, notifyContext.UserId);
        }

        public async Task SendMailMessageAsync(NotifyContext<MailMessage> notifyContext)
        {
            await SendEmailAsync(notifyContext.Context, notifyContext.UserId);
        }

        private async Task SendEmailAsync(MailMessage message, Guid userId)
        {
            try
            {
                using var client = new SmtpClient(_smtpProperties.Server, _smtpProperties.Port) { EnableSsl = _smtpProperties.EnableSsl };
                if (_smtpProperties.Username == null && _smtpProperties.Password == null)
                {
                    client.Credentials = new NetworkCredential(_smtpProperties.Username, _smtpProperties.Password);
                }
                await client.SendMailAsync(message);
                _logger.LogDebug("Email sent to {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {UserId}", userId);
            }
        }
    }
}
