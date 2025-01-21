using System.Net.Mail;

namespace Fiap.FileCut.Core.Objects;

/// <summary>
/// Object that represents a mail message to be sent when a file is cut.
/// 
/// Thats not contains property 'From' because the mail server is configured in system.
/// </summary>
/// <param name="to">Email address to send the message.</param>
public class FileCutMailMessage(string to)
{
    /// <summary>
    /// Email address to send the message.
    /// </summary>
    public MailAddress To { get; set; } = new MailAddress(to);
    /// <summary>
    /// Email address to send a copy of the message.
    /// </summary>
    public string Subject { get; set; } = string.Empty;
    /// <summary>
    /// Body of the message.
    /// </summary>
    public string Body { get; set; } = string.Empty;
    /// <summary>
    /// Indicates if the body of the message is in HTML format.
    /// </summary>
    public bool IsBodyHtml { get; set; } = false;
}
