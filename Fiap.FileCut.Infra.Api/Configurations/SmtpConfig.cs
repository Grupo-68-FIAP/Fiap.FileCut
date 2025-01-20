using Fiap.FileCut.Core.Objects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.FileCut.Infra.Api.Configurations
{
    public static class SmtpConfig
    {
        public static void AddSmtpConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var config = GetSmatpConfig(configuration);
            if (config != null)
                services.ConfigureOptions(config);
        }

        internal static SmtpProperties? GetSmatpConfig(IConfiguration configuration)
        {
            var smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER");
            var smtpPort = Environment.GetEnvironmentVariable("SMTP_PORT");
            var smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME");
            var smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

            if (!string.IsNullOrEmpty(smtpServer) || !string.IsNullOrEmpty(smtpPort) || !string.IsNullOrEmpty(smtpUsername) || !string.IsNullOrEmpty(smtpPassword))
            {
                if (smtpServer == null)
                    throw new MissingFieldException("To configure the SMTP server, you must provide the SMTP_SERVER environment variable.");
                if (smtpPort == null)
                    throw new MissingFieldException("To configure the SMTP port, you must provide the SMTP_PORT environment variable.");
                if (smtpUsername == null)
                    throw new MissingFieldException("To configure the SMTP username, you must provide the SMTP_USERNAME environment variable.");
                if (smtpPassword == null)
                    throw new MissingFieldException("To configure the SMTP password, you must provide the SMTP_PASSWORD environment variable.");

                return new SmtpProperties(smtpServer, int.Parse(smtpPort), smtpUsername, smtpPassword);
            }

            var smtpConfigurationSection = configuration.GetSection("SmtpConfig");
            var confg = smtpConfigurationSection.Get<SmtpProperties>();
            if (confg != null)
                return confg;

            return null;
        }
    }
}
