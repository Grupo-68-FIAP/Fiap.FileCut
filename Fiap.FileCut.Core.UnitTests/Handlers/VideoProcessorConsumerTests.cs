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
        var fileServiceMock = new Mock<IFileService>();
        var mockLogger = new Mock<ILogger<VideoProcessorConsumer>>();
        var consumer = new VideoProcessorConsumer(fileServiceMock.Object, mockNotifyService.Object, videoProcessingServiceMock.Object, mockLogger.Object);
        var userEvent = new VideoUploadedEvent("TesteVideo");
        var context = new NotifyContext<VideoUploadedEvent>(userEvent, Guid.NewGuid());
        var user = new User { Email = "test@example.com" };
        var fileStream = new Fiap.FileCut.Infra.Storage.Shared.Models.FileStreamResult("video.mp4", new MemoryStream());

        mockUserService.Setup(s => s.GetUserAsync(context.UserId, CancellationToken.None)).ReturnsAsync(user);
        fileServiceMock.Setup(s => s.GetFileAsync(context.UserId, context.Value.VideoName, CancellationToken.None)).ReturnsAsync(fileStream);
        videoProcessingServiceMock.Setup(s => s.ProcessVideoAsync(fileStream.FileStream, CancellationToken.None)).ReturnsAsync(new MemoryStream());
        fileServiceMock.Setup(s => s.SaveFileAsync(context.UserId, It.IsAny<string>(), It.IsAny<Stream>(), CancellationToken.None)).ReturnsAsync(true);

        // Act
        await consumer.HandleAsync(context);

        // Assert
        mockNotifyService.Verify(s => s.NotifyAsync(It.IsAny<NotifyContext<UserNotifyEvent>>()), Times.Once);
        fileServiceMock.Verify(s => s.GetFileAsync(context.UserId, context.Value.VideoName, It.IsAny<CancellationToken>()), Times.Once);
        fileServiceMock.Verify(s => s.SaveFileAsync(context.UserId, 
            It.Is<string>(fileName => fileName.EndsWith(".zip")),
            It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}