using Fiap.FileCut.Core.Adapters;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Fiap.FileCut.Core.Objects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fiap.FileCut.Core.UnitTests.Adapters;

public class UserNotifyQueuePublishTests
{
    [Fact]
    public async Task NotifyAsync_Should_Call_PublishAsync_When_Correct_Type()
    {
        // Arrange
        var mockService = new Mock<IMessagingPublisherService>();
        var queuePublish = new UserNotifyQueuePublish(mockService.Object);
        var userId = Guid.NewGuid();
        var notifyContext = new NotifyContext<UserNotifyEvent>(new UserNotifyEvent("VideoTest"), userId);

        // Act
        await queuePublish.NotifyAsync(notifyContext);

        // Assert
        mockService.Verify(s => s.PublishMessage(It.IsAny<NotifyContext<UserNotifyEvent>>()), Times.Once);
    }

    [Fact]
    public async Task PublishAsync_Should_Call_PublishMessage()
    {
        // Arrange
        var mockService = new Mock<IMessagingPublisherService>();
        var queuePublish = new UserNotifyQueuePublish(mockService.Object);
        var userId = Guid.NewGuid();
        var notifyContext = new NotifyContext<UserNotifyEvent>(new UserNotifyEvent("VideoTest"), userId);

        // Act
        await queuePublish.PublishAsync(notifyContext);

        // Assert
        mockService.Verify(s => s.PublishMessage(notifyContext), Times.Once);
    }
}