using Fiap.FileCut.Core.Objects;
using System.Net.Mail;

namespace Fiap.FileCut.Core.Interfaces.Factories
{
    public interface ISmtpClient
    {
        SmtpProperties GetProperties();
        Task SendMailAsync(MailMessage message);
    }
}
