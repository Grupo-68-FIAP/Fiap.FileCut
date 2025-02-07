using Fiap.FileCut.Core.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fiap.FileCut.Core.UnitTests.Handlers;

public class VideoProcessorConsumerTests
{
    [Fact]
    public async Task HandleAsync_Should_Send_Correct_Notification()
    {
        // Arrange
        var mockNotifyService = new Mock<INotifyService>();
        var mockUserService = new Mock<IUserService>();
        var videoProcessingServiceMock = new Mock<IVideoProcessingService>();
        var packageServiceMock = new Mock<IPackageService>();
        var mockLogger = new Mock<ILogger<VideoProcessorConsumer>>();
        var consumer = new VideoProcessorConsumer(mockNotifyService.Object, mockLogger.Object, videoProcessingServiceMock.Object, packageServiceMock.Object);
        var userEvent = new VideoUploadedEvent("TesteVideo");
        var context = new NotifyContext<VideoUploadedEvent>(userEvent, Guid.NewGuid());
        var user = new User { Email = "test@example.com" };
        mockUserService.Setup(s => s.GetUserAsync(context.UserId, CancellationToken.None)).ReturnsAsync(user);

        // Act
        await consumer.HandleAsync(context);

        // Assert
        mockNotifyService.Verify(s => s.NotifyAsync(It.IsAny<NotifyContext<UserNotifyEvent>>()), Times.Once);
    }
}