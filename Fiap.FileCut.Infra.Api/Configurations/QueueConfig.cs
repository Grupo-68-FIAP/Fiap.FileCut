using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Infra.RabbitMq;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Fiap.FileCut.Infra.Api.Configurations;

/// <summary>
/// Cors configuration class.
/// </summary>
public static class QueueConfig
{

    private static readonly List<Func<IServiceScope, Task>> queueActions = [];

    public static async Task AddQueue(this IServiceCollection services, Action<QueueBuilder> conf)
    {
        var connectionString = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING");
        ArgumentNullException.ThrowIfNull(connectionString);

        var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        services.AddSingleton<IConnection>(sp => connection);
        services.AddSingleton<IChannel>(sp => channel);
        services.AddScoped<IMessagingConsumerService, RabbitMqConsumerService>();
        conf(new QueueBuilder(services));
    }

    public class QueueBuilder(IServiceCollection services)
    {
        private readonly IServiceCollection _services = services;

        public QueueBuilder SubscribeQueue<T, TImplementation>()
            where T : class
            where TImplementation : class, IMessageHandler<T>
        {
            // TODO NOSONAR: Talvez criar um Attribute para definir o nome da fila no objeto colocando esse fullname coalesce
            var queueName = typeof(T).FullName;
            ArgumentNullException.ThrowIfNull(queueName);
            return SubscribeQueue<T, TImplementation>(queueName);
        }

        public QueueBuilder SubscribeQueue<T, TImplementation>(string queueName)
            where T : class
            where TImplementation : class, IMessageHandler<T>
        {
            _services.AddScoped<IMessageHandler<T>, TImplementation>();
            queueActions.Add(async (scope) => await SubscribeQueue<T>(scope, queueName));
            return this;
        }

        private static async Task SubscribeQueue<T>(IServiceScope scope, string queueName) where T : class
        {
            var consumer = scope.ServiceProvider.GetRequiredService<IMessageHandler<T>>();
            var consumerService = scope.ServiceProvider.GetRequiredService<IMessagingConsumerService>();
            await consumerService.SubscribeAsync(queueName, consumer);
        }
    }

    public async static Task UseQueue(this IServiceScope scope)
    {
        foreach (var action in queueActions)
        {
            await action(scope);
        }
    }
}
