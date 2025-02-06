using Fiap.FileCut.Core.Adapters;
using Fiap.FileCut.Core.Interfaces.Adapters;
using Fiap.FileCut.Core.Interfaces.Handlers;
using Fiap.FileCut.Core.Interfaces.Services;
using Fiap.FileCut.Core.Services;
using Fiap.FileCut.Infra.RabbitMq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Fiap.FileCut.Infra.Api.Configurations;

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
            if (SmtpConfigure(_services, configuration))
            {
                _services.AddScoped<INotifyAdapter, EmailNotifyAdapter>();
                _services.AddScoped<ISmtpHandler, SmtpClientWrapper>();
            }
            return this;
        }

        private static bool SmtpConfigure(IServiceCollection services, IConfiguration configuration)
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
                return true;
            }

            var smtpConfigurationSection = configuration.GetSection("SmtpConfig");
            if (smtpConfigurationSection.Exists())
            {
                services.Configure<SmtpProperties>(smtpConfigurationSection);
                return true;
            }

            return false;
        }
    }

    private sealed class SmtpClientWrapper(IOptions<SmtpProperties> smtpProperties) : ISmtpHandler
    {
        private readonly SmtpProperties _smtpProperties = smtpProperties.Value;

        public MailAddress GetFrom()
        {
            return new MailAddress(_smtpProperties.Username);
        }

        public async Task SendMailAsync(MailMessage message)
        {
            using var client = new SmtpClient(_smtpProperties.Server, _smtpProperties.Port) // NOSONAR
            {
                Credentials = new NetworkCredential(_smtpProperties.Username, _smtpProperties.Password),
                EnableSsl = _smtpProperties.EnableSsl
            };
            await client.SendMailAsync(message);
        }
    }

    private sealed class SmtpProperties
    {
        public string? Server { get; set; }
        public int Port { get; set; }
        public string Username { get; set; } = "";
        public string? Password { get; set; }
        public bool EnableSsl { get; set; }
    }
}
