using Fiap.FileCut.Core.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.FileCut.Core.UnitTests.Handlers;

public class UserNotifyConsumerTests
{
    [Fact]
    public async Task HandleAsync_Should_Send_Correct_Notification()
    {
        // Arrange
        var mockNotifyService = new Mock<INotifyService>();
        var mockUserService = new Mock<IUserService>();
        var mockLogger = new Mock<ILogger<UserNotifyConsumer>>();
        var consumer = new UserNotifyConsumer(mockNotifyService.Object, mockUserService.Object, mockLogger.Object);
        var userEvent = new UserNotifyEvent("TesteVideo") { PackName = "TestPack", IsSuccess = true };
        var context = new NotifyContext<UserNotifyEvent>(userEvent, Guid.NewGuid());
        var user = new User { Email = "test@example.com" };
        mockUserService.Setup(s => s.GetUserAsync(context.UserId, CancellationToken.None)).ReturnsAsync(user);

        // Act
        await consumer.HandleAsync(context);

        // Assert
        mockNotifyService.Verify(s => s.NotifyAsync(It.IsAny<NotifyContext<FileCutMailMessage>>()), Times.Once);
    }
}
