using Fiap.FileCut.Core.Attributes;
using Fiap.FileCut.Core.Extensions;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Fiap.FileCut.Infra.RabbitMq
{
    public class RabbitMqPublisherService : IMessagingPublisherService
    {
        private readonly IChannel _channel;
        private readonly ILogger<RabbitMqPublisherService> _logger;

        public RabbitMqPublisherService(IChannel channel, ILogger<RabbitMqPublisherService> logger)
        {
            _channel = channel;
            _logger = logger;
        }

        public async Task PublishMessage<T>(NotifyContext<T> context)
        {
            ArgumentNullException.ThrowIfNull(context.Value);

            var queue = context.Value.GetType().FullName;

            var props = Attribute.GetCustomAttributes(typeof(T));

            foreach (object attr in props)
            {
                if (attr is MessageQueueAttribute a)
                {
                    queue = a.Queue.GetQueueNameAttribute();
                    break;
                }
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(queue);

            _logger.LogInformation("[{Queue}] Publishing message", queue);

            await _channel.QueueDeclareAsync(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(context.Value));

            _logger.LogInformation("[{Queue}] VideoName body prepared: {Body}", queue, body);

            var properties = new BasicProperties();
            properties.Headers = new Dictionary<string, object?>
            { 
                { nameof(context.UserId), Encoding.UTF8.GetBytes(context.UserId.ToString()) }
            };

            await _channel.BasicPublishAsync(
                exchange: "",
                routingKey: queue,
                body: body,
                mandatory: true,
                basicProperties: properties
            );

            _logger.LogInformation("[{Queue}] VideoName sent: {Body}", queue, body);
        }
    }
}
