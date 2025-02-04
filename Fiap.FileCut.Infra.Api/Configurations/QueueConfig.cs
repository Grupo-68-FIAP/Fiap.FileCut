using Fiap.FileCut.Core.Attributes;
using Fiap.FileCut.Core.Extensions;
using Fiap.FileCut.Core.Interfaces.Adapters;
using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Infra.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using System.Reflection;
using static Fiap.FileCut.Infra.Api.Configurations.NotificationConfig;

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

        services.AddSingleton<IMessagingConsumerService, RabbitMqConsumerService>();
        services.AddSingleton<IMessagingPublisherService, RabbitMqPublisherService>();
        conf(new QueueBuilder(services));
    }

    public class QueueBuilder(IServiceCollection services)
    {
        private readonly IServiceCollection _services = services;

        public QueueBuilder SubscribeQueue<T, TImplementation>()
            where T : class
            where TImplementation : class, IConsumerHandler<T>
        {
            var queueName = typeof(T).FullName;
            
            var props = Attribute.GetCustomAttributes(typeof(T));

            foreach (object attr in props)
            {
                if (attr is MessageQueueAttribute a)
                {
                    queueName = a.Queue.GetQueueNameAttribute();
                    break;
                }
            }

            ArgumentNullException.ThrowIfNull(queueName);

            return SubscribeQueue<T, TImplementation>(queueName);
        }

        public QueueBuilder AddPublisher<T>()
            where T : class, INotifyAdapter
        {
            _services.AddScoped<INotifyAdapter, T>();

            return this;
        }

        private QueueBuilder SubscribeQueue<T, TImplementation>(string queueName)
            where T : class
            where TImplementation : class, IConsumerHandler<T>
        {
            _services.AddScoped<IConsumerHandler<T>, TImplementation>();
            queueActions.Add(async (scope) => await SubscribeQueue<T>(scope, queueName));
            return this;
        }

        private static async Task SubscribeQueue<T>(IServiceScope scope, string queueName) where T : class
        {
            var consumer = scope.ServiceProvider.GetRequiredService<IConsumerHandler<T>>();
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
