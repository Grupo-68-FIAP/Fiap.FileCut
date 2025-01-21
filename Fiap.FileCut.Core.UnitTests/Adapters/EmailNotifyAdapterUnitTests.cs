using Fiap.FileCut.Core.Adapters;
using Fiap.FileCut.Core.Interfaces.Factories;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net.Mail;

namespace Fiap.FileCut.Core.UnitTests.Adapters
{
    public class EmailNotifyAdapterUnitTests
    {
        [Fact]
        public void NotifyAsync_WhenCalledWithMailMessage_ShouldCallSendMailMessageAsync()
        {
            // Arrange
            var logger = new Mock<ILogger<EmailNotifyAdapter>>();
            var smtpClient= new Mock<ISmtpClient>();
            smtpClient.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>())).Returns(Task.CompletedTask);
            var adapter = new EmailNotifyAdapter(logger.Object, smtpClient.Object);
            var mailMessage = new MailMessage();
            var notifyContext = new NotifyContext<MailMessage>(mailMessage, Guid.NewGuid());
            // Act
            adapter.NotifyAsync(notifyContext);
            // Assert
            smtpClient.Verify(x => x.SendMailAsync(mailMessage), Times.Once);
        }
    }
}
