using System.Net.Mail;

namespace Fiap.FileCut.Core.Objects
{
    public class FileCutMailMessage(string to)
    {
        public MailAddress To { get; set; } = new MailAddress(to);
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsBodyHtml { get; set; } = false;
    }
}
