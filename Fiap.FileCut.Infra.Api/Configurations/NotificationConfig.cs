using Fiap.FileCut.Core.Adapters;
using Fiap.FileCut.Core.Interfaces.Adapters;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Objects;
using Fiap.FileCut.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.FileCut.Infra.Api.Configurations
{
    public static class NotificationConfig
    {
        public static NotificationBuilder AddNotifications(this IServiceCollection services)
        {
            return new NotificationBuilder(services);
        }

        public class NotificationBuilder
        {
            private readonly IServiceCollection _services;
            internal NotificationBuilder(IServiceCollection services)
            {
                services.AddScoped<INotifyService, NotifyService>();
                _services = services;
            }
            public NotificationBuilder EmailNotify(IConfiguration configuration)
            {
                _services.AddScoped<INotifyAdapter, EmailNotifyAdapter>();
                SmtpConfigure(_services, configuration);
                return this;
            }

            private static void SmtpConfigure(IServiceCollection services, IConfiguration configuration)
            {
                var smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER");
                var smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME");

                if (!string.IsNullOrEmpty(smtpServer) || !string.IsNullOrEmpty(smtpUsername))
                {
                    if (smtpServer == null)
                        throw new MissingFieldException("To configure the SMTP server, you must provide the SMTP_SERVER environment variable.");

                    if (smtpUsername == null)
                        throw new MissingFieldException("To configure the SMTP server, you must provide the SMTP_USERNAME environment variable.");

                    var smtpPort = Environment.GetEnvironmentVariable("SMTP_PORT");
                    var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");
                    var smtpEnableSsl = Environment.GetEnvironmentVariable("SMTP_ENABLE_SSL");

                    services.Configure<SmtpProperties>(options =>
                    {
                        options.Server = smtpServer;
                        options.Port = int.Parse(smtpPort ?? "587");
                        options.Username = smtpUsername;
                        options.Password = smtpPassword;
                        options.EnableSsl = bool.Parse(smtpEnableSsl ?? "false");
                    });
                    return;
                }

                var smtpConfigurationSection = configuration.GetSection("SmtpConfig");
                var confg = smtpConfigurationSection.Get<SmtpProperties>();
                if (confg != null)
                {
                    services.Configure<SmtpProperties>(smtpConfigurationSection);
                }
            }
        }
    }
}
