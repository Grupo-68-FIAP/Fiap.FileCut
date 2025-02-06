using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Objects.QueueEvents;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;

namespace Fiap.FileCut.Infra.RabbitMq.UnitTests;
public class RabbitMqPublisherServiceTests
{
    [Fact]
    public async Task PublishMessage_Should_DeclareQueue_And_PublishMessage()
    {
        // Arrange
        var mockChannel = new Mock<IChannel>();
        var mockLogger = new Mock<ILogger<RabbitMqPublisherService>>();
        var service = new RabbitMqPublisherService(mockChannel.Object, mockLogger.Object);
        var userId = Guid.NewGuid();
        var notifyContext = new NotifyContext<UserNotifyEvent>(new UserNotifyEvent("video"), userId);

        // Act
        await service.PublishMessage(notifyContext);

        // Assert
        mockChannel.Verify(c =>
            c.QueueDeclareAsync(
                It.Is<string>(x => x.SequenceEqual("FIAP-FILECUT-INFORMATION-QUEUE")),
                true,
                false,
                false,
                (IDictionary<string, object?>?)null,
                false,
                false,
                default
            ), Times.Once
        );

        mockChannel.Verify(c =>
            c.BasicPublishAsync(
                "",
                It.IsAny<string>(),
                true,
                It.IsAny<BasicProperties>(),
                It.IsAny<ReadOnlyMemory<byte>>(),
                CancellationToken.None
            ), Times.Once
        );
    }
}
