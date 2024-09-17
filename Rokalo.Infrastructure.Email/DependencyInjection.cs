namespace Rokalo.Infrastructure.Email
{
    using Microsoft.Extensions.DependencyInjection;
    using Rokalo.Application.Contracts.Email;
    using Rokalo.Infrastructure.Email.Configurations;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureEmailConfiguration(this IServiceCollection services, SmtpConfiguration configuration)
        {
            services.Configure<SmtpConfiguration>(options =>
            {
                options.DisplayName = configuration.DisplayName;
                options.From = configuration.From;
                options.Host = configuration.Host;
                options.Password = configuration.Password;
                options.Port = configuration.Port;
                options.UserName = configuration.UserName;
                options.UseSSL = configuration.UseSSL;
                options.UseStartTls = configuration.UseStartTls;
            });
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }

    }
}
