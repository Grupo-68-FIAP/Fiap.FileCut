using System.Net.Mail;

namespace Fiap.FileCut.Core.Interfaces.Handler;

/// <summary>
/// SMTP client.
/// </summary>
public interface ISmtpHandler
{
    /// <summary>
    /// Get server email address.
    /// </summary>
    /// <returns>Mail address.</returns>
    MailAddress GetFrom();
    /// <summary>
    /// Send mail.
    /// </summary>
    /// <param name="message">Mail message.</param>
    /// <returns>Asynchronous task.</returns>
    Task SendMailAsync(MailMessage message);
}
