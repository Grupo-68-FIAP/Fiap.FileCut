using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Fiap.FileCut.Infra.RabbitMq;

public static class RabbitMqConfiguration
{
    public async static Task AddRabbitMQ(this IServiceCollection services, string connectionString)
    {
        if (services.Any(x => x.ServiceType == typeof(IConnection)))
            return;
        
        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        services.AddSingleton<IConnection>(sp => connection);
        services.AddSingleton<IChannel>(sp => channel);
    }
}
