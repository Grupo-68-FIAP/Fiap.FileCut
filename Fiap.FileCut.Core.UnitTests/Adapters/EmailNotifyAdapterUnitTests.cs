using Fiap.FileCut.Core.Adapters;
using Fiap.FileCut.Core.Interfaces.Factories;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net.Mail;

namespace Fiap.FileCut.Core.UnitTests.Adapters;

public class EmailNotifyAdapterUnitTests
{
    [Fact]
    public void NotifyAsync_WhenCalledWithMailMessage_ShouldCallSendMailMessageAsync()
    {
        // Arrange
        var logger = new Mock<ILogger<EmailNotifyAdapter>>();
        var smtpClient = new Mock<ISmtpClient>();
        smtpClient.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>())).Returns(Task.CompletedTask);
        var adapter = new EmailNotifyAdapter(logger.Object, smtpClient.Object);
        var mailMessage = new MailMessage();
        var notifyContext = new NotifyContext<MailMessage>(mailMessage, Guid.NewGuid());
        // Act
        adapter.NotifyAsync(notifyContext);
        // Assert
        smtpClient.Verify(x => x.SendMailAsync(mailMessage), Times.Once);
    }

    [Fact]
    public void NotifyAsync_WhenCalledWithFileCutMailMessage_ShouldCallSendFileCutMailMessageAsync()
    {
        // Arrange
        var logger = new Mock<ILogger<EmailNotifyAdapter>>();
        logger.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Verifiable();
        var smtpClient = new Mock<ISmtpClient>();
        smtpClient.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>())).Returns(Task.CompletedTask);
        smtpClient.Setup(x => x.GetFrom()).Returns(new MailAddress("test@test.com"));
        var adapter = new EmailNotifyAdapter(logger.Object, smtpClient.Object);
        var fileCutMailMessage = new FileCutMailMessage("test@test.com");
        var notifyContext = new NotifyContext<FileCutMailMessage>(fileCutMailMessage, Guid.NewGuid());
        // Act
        adapter.NotifyAsync(notifyContext);
        // Assert
        smtpClient.Verify(x => x.SendMailAsync(It.IsAny<MailMessage>()), Times.Once);
        logger.Verify(x => x.Log(
            It.Is<LogLevel>(ll => ll == LogLevel.Debug),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public void NotifyAsync_WhenCalledWithInvalidContext_ShouldNotCallSendMailMessageAsync()
    {
        // Arrange
        var logger = new Mock<ILogger<EmailNotifyAdapter>>();
        var smtpClient = new Mock<ISmtpClient>();
        smtpClient.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>())).Returns(Task.CompletedTask);
        var adapter = new EmailNotifyAdapter(logger.Object, smtpClient.Object);
        var notifyContext = new NotifyContext<string>("test", Guid.NewGuid());
        // Act
        adapter.NotifyAsync(notifyContext);
        // Assert
        smtpClient.Verify(x => x.SendMailAsync(It.IsAny<MailMessage>()), Times.Never);
    }

    [Fact]
    public void SendEmailAsync_WhenCalledWithSmtpClientInvalid_ShouldLogError()
    {
        // Arrange
        var logger = new Mock<ILogger<EmailNotifyAdapter>>();
        logger.Setup(x => x.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Verifiable();
        var smtpClient = new Mock<ISmtpClient>();
        smtpClient.Setup(x => x.SendMailAsync(It.IsAny<MailMessage>())).Throws(new Exception());
        smtpClient.Setup(x => x.GetFrom()).Returns(new MailAddress("test@test.com"));
        var adapter = new EmailNotifyAdapter(logger.Object, smtpClient.Object);
        var userId = Guid.NewGuid();
        var fileCutMailMessage = new FileCutMailMessage("test@test.com");
        var notifyContext = new NotifyContext<FileCutMailMessage>(fileCutMailMessage, userId);
        // Act
        adapter.NotifyAsync(notifyContext);
        // Assert
        logger.Verify(x => x.Log(
            It.Is<LogLevel>(ll => ll == LogLevel.Error),
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }
}
