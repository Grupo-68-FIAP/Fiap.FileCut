using Fiap.FileCut.Core.Interfaces.Adapters;
using Fiap.FileCut.Core.Interfaces.Factories;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;
using System.Net.Mail;

namespace Fiap.FileCut.Core.Adapters;

/// <summary>
/// Email Notify Adapter.
/// 
/// Adapter to send email notifications.
/// </summary>
/// <param name="logger"></param>
/// <param name="smtpClient">Smtp client to send email.</param>
public class EmailNotifyAdapter(ILogger<EmailNotifyAdapter> logger, ISmtpClient smtpClient) : INotifyAdapter
{
    private readonly ILogger<EmailNotifyAdapter> _logger = logger;
    private readonly ISmtpClient _smtpClient = smtpClient;

    /// <summary>
    /// Notify async. 
    /// 
    /// Send email notification if the context is a instance of <see cref="MailMessage"/> or <see cref="FileCutMailMessage"/>
    /// else do nothing.
    /// </summary>
    /// <typeparam name="T"> Any object. </typeparam>
    /// <param name="notifyContext"> Notify context.</param>
    /// <returns></returns>
    public Task NotifyAsync<T>(NotifyContext<T> notifyContext)
    {
        if (notifyContext is NotifyContext<MailMessage> mailMessageContext)
        {
            return SendMailMessageAsync(mailMessageContext);
        }
        else if (notifyContext is NotifyContext<FileCutMailMessage> fileCutMailMessageContext)
        {
            return SendFileCutMailMessageAsync(fileCutMailMessageContext);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Send mail message async.
    /// 
    /// Send email notification with <see cref="FileCutMailMessage"/>.
    /// </summary>
    /// <param name="notifyContext"></param>
    /// <returns></returns>
    public async Task SendFileCutMailMessageAsync(NotifyContext<FileCutMailMessage> notifyContext)
    {
        var mailMessage = new MailMessage
        {
            From = _smtpClient.GetFrom(),
            Subject = notifyContext.Context.Subject,
            Body = notifyContext.Context.Body,
            IsBodyHtml = notifyContext.Context.IsBodyHtml
        };
        mailMessage.To.Add(notifyContext.Context.To);

        await SendEmailAsync(mailMessage, notifyContext.UserId);
    }

    /// <summary>
    /// Send mail message async.
    /// 
    /// Send email notification with <see cref="MailMessage"/>.
    /// </summary>
    /// <param name="notifyContext"></param>
    /// <returns></returns>
    public async Task SendMailMessageAsync(NotifyContext<MailMessage> notifyContext)
    {
        await SendEmailAsync(notifyContext.Context, notifyContext.UserId);
    }

    private async Task SendEmailAsync(MailMessage message, Guid userId)
    {
        try
        {
            await _smtpClient.SendMailAsync(message);
            _logger.LogDebug("Email sent to {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {UserId}", userId);
        }
    }

}
