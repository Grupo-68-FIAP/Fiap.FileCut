using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
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

    public async Task SubscribeAsync<T>(string queueName, IMessageHandler<T> handler) where T : class
    {
        await _channel.QueueDeclareAsync(queueName, true, false, false, null);
        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                _logger.LogDebug("Mensagem recebida da fila {QueueName}", queueName);
                var body = ea.Body.ToArray();
                var userId = GetUserId(ea);
                var message = JsonSerializer.Deserialize<T>(body);

                if (message != null)
                {
                    var context = new NotifyContext<T>(message, userId);
                    await handler.HandleAsync(context);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem da fila {QueueName}", queueName);
            }
        };

        await _channel.BasicConsumeAsync(queueName, true, consumer);
        _logger.LogInformation("Consumidor da fila {QueueName} iniciado", queueName);
    }

    private static Guid GetUserId(BasicDeliverEventArgs ea)
    {
        var byteId = ea.BasicProperties.Headers?["UserID"] as byte[]
            ?? throw new MissingMemberException("Cabeçalho 'UserID' não encontrado na mensagem");
        var userId = Encoding.UTF8.GetString(byteId);
        ArgumentNullException.ThrowIfNull(userId);
        return new Guid(userId);
    }
}