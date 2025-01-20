using System.Net.Mail;

namespace Fiap.FileCut.Core.Interfaces.Factories
{
    public interface ISmtpClient
    {
        Task SendMailAsync(MailMessage message);
    }
}
