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
    public async Task HandleAsync_Should_Send_Correct_Notification_On_Success()
    {
        // Arrange
        var mockNotifyService = new Mock<INotifyService>();
        var mockLogger = new Mock<ILogger<VideoProcessorConsumer>>();
        var videoProcessingServiceMock = new Mock<IVideoProcessingService>();
        var packageServiceMock = new Mock<IPackageService>();

        var consumer = new VideoProcessorConsumer(
            mockNotifyService.Object,
            mockLogger.Object,
            videoProcessingServiceMock.Object,
            packageServiceMock.Object);

        var userEvent = new VideoUploadedEvent("TesteVideo");
        var context = new NotifyContext<VideoUploadedEvent>(userEvent, Guid.NewGuid());

        videoProcessingServiceMock
            .Setup(v => v.ProcessVideoAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("path/to/zipfile.zip");

        // Act
        await consumer.HandleAsync(context);

        // Assert
        mockNotifyService.Verify(s => s.NotifyAsync(It.Is<NotifyContext<UserNotifyEvent>>(c => c.Value.IsSuccess)), Times.Once);
        mockLogger.Verify(l => l.LogInformation("Video em processamento"), Times.Once);
        mockLogger.Verify(l => l.LogInformation("Video processado com sucesso"), Times.Once);
        mockLogger.Verify(l => l.LogInformation("Imagens empacotadas"), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_Send_Correct_Notification_On_Failure()
    {
        // Arrange
        var mockNotifyService = new Mock<INotifyService>();
        var mockLogger = new Mock<ILogger<VideoProcessorConsumer>>();
        var videoProcessingServiceMock = new Mock<IVideoProcessingService>();
        var packageServiceMock = new Mock<IPackageService>();

        var consumer = new VideoProcessorConsumer(
            mockNotifyService.Object,
            mockLogger.Object,
            videoProcessingServiceMock.Object,
            packageServiceMock.Object);

        var userEvent = new VideoUploadedEvent("TesteVideo");
        var context = new NotifyContext<VideoUploadedEvent>(userEvent, Guid.NewGuid());

        videoProcessingServiceMock
            .Setup(v => v.ProcessVideoAsync(context.UserId, context.Value.VideoName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Processing error"));

        // Act
        await consumer.HandleAsync(context);

        // Assert
        mockNotifyService.Verify(s => s.NotifyAsync(It.Is<NotifyContext<UserNotifyEvent>>(c => !c.Value.IsSuccess && c.Value.ErrorMessage == "Processing error")), Times.Once);
        mockLogger.Verify(l => l.LogInformation("Video em processamento"), Times.Once);
        mockLogger.Verify(l => l.LogError(It.IsAny<Exception>(), "Erro ao processar vídeo"), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_Package_Images()
    {
        // Arrange
        var mockNotifyService = new Mock<INotifyService>();
        var mockLogger = new Mock<ILogger<VideoProcessorConsumer>>();
        var videoProcessingServiceMock = new Mock<IVideoProcessingService>();
        var packageServiceMock = new Mock<IPackageService>();

        var consumer = new VideoProcessorConsumer(
            mockNotifyService.Object,
            mockLogger.Object,
            videoProcessingServiceMock.Object,
            packageServiceMock.Object);

        var userEvent = new VideoUploadedEvent("TesteVideo");
        var context = new NotifyContext<VideoUploadedEvent>(userEvent, Guid.NewGuid());

        videoProcessingServiceMock
            .Setup(v => v.ProcessVideoAsync(context.UserId, context.Value.VideoName, It.IsAny<CancellationToken>()))
            .ReturnsAsync("path/to/zipfile.zip");

        // Act
        await consumer.HandleAsync(context);

        // Assert
        packageServiceMock.Verify(p => p.PackageImagesAsync("path/to/zipfile.zip"), Times.Once);
    }
}
