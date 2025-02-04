using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using System.Text;

namespace Fiap.FileCut.Infra.RabbitMq.UnitTests
{
    public class RabbitMqConsumerServiceUnitTests
    {
        [Fact]
        public async Task SubscribeAsync_WhenValidHandler_ShouldSubscribe()
        {
            // Arrange
            var channel = new Mock<IChannel>();
            var logger = new Mock<ILogger<RabbitMqConsumerService>>();
            var consumerService = new RabbitMqConsumerService(channel.Object, logger.Object);
            var handler = new Mock<IConsumerHandler<string>>();
            var queueName = "test-queue";
            // Act
            await consumerService.SubscribeAsync(queueName, handler.Object);
            // Assert
            handler.Verify(c => c.HandleAsync(It.IsAny<NotifyContext<string>>()), Times.Never);
            channel.Verify(c => c.BasicConsumeAsync(queueName, true, It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<IAsyncBasicConsumer>(), It.IsAny<CancellationToken>()), Times.Once);
            logger.Verify(x => x.Log(It.Is<LogLevel>(ll => ll == LogLevel.Information), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task SubscribeAsync_WhenReceivedAValidMessage_ShouldCallHandler()
        {
            // Arrange
            var channel = new Mock<IChannel>();
            var logger = new Mock<ILogger<RabbitMqConsumerService>>();
            var consumerService = new RabbitMqConsumerService(channel.Object, logger.Object);
            var handler = new Mock<IConsumerHandler<string>>();
            var queueName = "test-queue";
            var message = "test-message";
            var userId = Guid.NewGuid().ToString();

            channel.Setup(c => c.BasicConsumeAsync(queueName, true, It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<IAsyncBasicConsumer>(), It.IsAny<CancellationToken>()))
                .Callback<string, bool, string, bool, bool, IDictionary<string, object?>, IAsyncBasicConsumer, CancellationToken>((queue, autoAck, consumerTag, noLocal, exclusive, arguments, consumer, cancellationToken) => // NOSONAR
                {
                    var encodeId = Encoding.UTF8.GetBytes(userId);
                    var headers = new Dictionary<string, object?> { { "UserId", encodeId } };
                    var prop = new Mock<IReadOnlyBasicProperties>();
                    prop.Setup(p => p.Headers).Returns(headers);
                    var messageBinary = Encoding.UTF8.GetBytes($"\"{message}\"");// Valid json string
                    _ = consumer.HandleBasicDeliverAsync(consumerTag, 1, false, "exchange", "routing-key", prop.Object, messageBinary, CancellationToken.None);
                });

            handler.Setup(c => c.HandleAsync(It.IsAny<NotifyContext<string>>())).Returns(Task.CompletedTask);
            // Act
            await consumerService.SubscribeAsync(queueName, handler.Object);
            // Assert
            handler.Verify(c => c.HandleAsync(It.Is<NotifyContext<string>>(r => r.Value == message)), Times.Once);
            logger.Verify(x => x.Log(It.Is<LogLevel>(ll => ll == LogLevel.Debug), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task SubscribeAsync_WhenReceivedMissingUserId_ShouldNotCallHandler()
        {
            // Arrange
            var channel = new Mock<IChannel>();
            var logger = new Mock<ILogger<RabbitMqConsumerService>>();
            var consumerService = new RabbitMqConsumerService(channel.Object, logger.Object);
            var handler = new Mock<IConsumerHandler<string>>();
            var queueName = "test-queue";
            channel.Setup(c => c.BasicConsumeAsync(queueName, true, It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<IAsyncBasicConsumer>(), It.IsAny<CancellationToken>()))
                .Callback<string, bool, string, bool, bool, IDictionary<string, object?>, IAsyncBasicConsumer, CancellationToken>((queue, autoAck, consumerTag, noLocal, exclusive, arguments, consumer, cancellationToken) => // NOSONAR
                {
                    var prop = new Mock<IReadOnlyBasicProperties>();
                    var messageBinary = Encoding.UTF8.GetBytes($"\"test-message\"");// Valid json string
                    _ = consumer.HandleBasicDeliverAsync(consumerTag, 1, false, "exchange", "routing-key", prop.Object, messageBinary, CancellationToken.None);
                });
            handler.Setup(c => c.HandleAsync(It.IsAny<NotifyContext<string>>())).Returns(Task.CompletedTask);
            // Act
            await consumerService.SubscribeAsync(queueName, handler.Object);
            // Assert
            handler.Verify(c => c.HandleAsync(It.IsAny<NotifyContext<string>>()), Times.Never);
            logger.Verify(x => x.Log(It.Is<LogLevel>(ll => ll == LogLevel.Error), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact]
        public async Task SubscribeAsync_WhenReceivedAInvalidMessage_ShouldNotCallHandler()
        {
            // Arrange
            var channel = new Mock<IChannel>();
            var logger = new Mock<ILogger<RabbitMqConsumerService>>();
            var consumerService = new RabbitMqConsumerService(channel.Object, logger.Object);
            var handler = new Mock<IConsumerHandler<string>>();
            var queueName = "test-queue";
            channel.Setup(c => c.BasicConsumeAsync(queueName, true, It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<IDictionary<string, object?>>(),
                It.IsAny<IAsyncBasicConsumer>(), It.IsAny<CancellationToken>()))
                .Callback<string, bool, string, bool, bool, IDictionary<string, object?>, IAsyncBasicConsumer, CancellationToken>((queue, autoAck, consumerTag, noLocal, exclusive, arguments, consumer, cancellationToken) => // NOSONAR
                {
                    var prop = new Mock<IReadOnlyBasicProperties>();
                    var messageBinary = Encoding.UTF8.GetBytes("json invalido");// Invalid json string
                    _ = consumer.HandleBasicDeliverAsync(consumerTag, 1, false, "exchange", "routing-key", prop.Object, messageBinary, CancellationToken.None);
                });
            handler.Setup(c => c.HandleAsync(It.IsAny<NotifyContext<string>>())).Returns(Task.CompletedTask);
            // Act
            await consumerService.SubscribeAsync(queueName, handler.Object);
            // Assert
            handler.Verify(c => c.HandleAsync(It.IsAny<NotifyContext<string>>()), Times.Never);
            logger.Verify(x => x.Log(It.Is<LogLevel>(ll => ll == LogLevel.Error), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }
    }
}
