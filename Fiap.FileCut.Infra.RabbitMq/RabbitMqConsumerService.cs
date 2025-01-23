using Fiap.FileCut.Core.Interfaces.Handler;
using Fiap.FileCut.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Fiap.FileCut.Infra.RabbitMq;

public class RabbitMqConsumerService : IMessagingConsumerService
{
    private readonly IChannel _channel;
    private readonly ILogger<RabbitMqConsumerService> _logger;

    public RabbitMqConsumerService(IChannel channel, ILogger<RabbitMqConsumerService> logger)
    {
        _channel = channel;
        _logger = logger;
    }

    public async Task SubscribeAsync<T>(string queueName, IMessageHandler<T> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                _logger.LogDebug("Mensagem recebida da fila {QueueName}", queueName);
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<T>(body);

                if (message != null)
                {
                    await handler.HandleAsync(message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem da fila {QueueName}", queueName);
            }
        };

        await _channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
    }
}