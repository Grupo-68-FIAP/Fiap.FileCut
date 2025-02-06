using Fiap.FileCut.Core.Adapters;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Moq;

namespace Fiap.FileCut.Core.UnitTests.Adapters;

public class VideoUploadedQueuePublishTests
{
    [Fact]
    public async Task NotifyAsync_Should_Call_PublishAsync_When_Correct_Type()
    {
        // Arrange
        var mockService = new Mock<IMessagingPublisherService>();
        var queuePublish = new VideoUploadedQueuePublish(mockService.Object);
        var userId = Guid.NewGuid();
        var notifyContext = new NotifyContext<VideoUploadedEvent>(new VideoUploadedEvent("VideoTest"), userId);

        // Act
        await queuePublish.NotifyAsync(notifyContext);

        // Assert
        mockService.Verify(s => s.PublishMessage(It.IsAny<NotifyContext<VideoUploadedEvent>>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_Should_Call_PublishMessage()
    {
        // Arrange
        var mockService = new Mock<IMessagingPublisherService>();
        var queuePublish = new VideoUploadedQueuePublish(mockService.Object);
        var userId = Guid.NewGuid();
        var notifyContext = new NotifyContext<VideoUploadedEvent>(new VideoUploadedEvent("VideoTest"), userId);

        // Act
        await queuePublish.PublishAsync(notifyContext);

        // Assert
        mockService.Verify(s => s.PublishMessage(notifyContext), Times.Once);
    }
}