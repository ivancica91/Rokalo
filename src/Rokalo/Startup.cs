namespace Rokalo
{
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Rokalo.Application;
    using Rokalo.Infrastructure.Db.Users;
    using Rokalo.Infrastructure.Email;
    using Rokalo.Infrastructure.Email.Configurations;
    using Rokalo.Infrastructure.Security;
    using Rokalo.Presentation.Api;
    using System;

    internal sealed class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public MssqlSettings MssqlSettings =>
            this.Configuration
            .GetSection(MssqlSettings.Key)
            .Get<MssqlSettings>() ?? throw new ArgumentNullException(nameof(this.MssqlSettings));

        public SmtpConfiguration SmtpConfiguration =>
            this.Configuration
            .GetSection(SmtpConfiguration.Key)
            .Get<SmtpConfiguration>() ?? throw new ArgumentNullException(nameof(this.SmtpConfiguration));

        public SecurityAdapterConfigurations SecurityAdapterConfigurations =>
            this.Configuration
            .GetSection(SecurityAdapterConfigurations.Key)
           .Get<SecurityAdapterConfigurations>() ?? throw new ArgumentNullException(nameof(this.SecurityAdapterConfigurations));

        public JwtTokenConfiguration JwtTokenConfiguration =>
            this.Configuration
            .GetSection(JwtTokenConfiguration.Key)
            .Get<JwtTokenConfiguration>() ?? throw new ArgumentNullException(nameof(this.JwtTokenConfiguration));

        public RefreshTokenConfiguration RefreshTokenConfiguration =>
            this.Configuration
            .GetSection(RefreshTokenConfiguration.Key)
            .Get<RefreshTokenConfiguration>() ?? throw new ArgumentNullException(nameof(this.RefreshTokenConfiguration));

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddInfrastructureUsersConfiguration(this.MssqlSettings);
            services.AddInfrastructureEmailConfiguration(this.SmtpConfiguration);
            services.Configure<SmtpConfiguration>(Configuration.GetSection(SmtpConfiguration.Key));
            services.AddSecurityAdapter(this.SecurityAdapterConfigurations);
            services.AddApplicationLayer();
            services.AddPresentationConfiguration(this.Environment);
            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseProblemDetails();

            if (!Environment.IsDevelopment())
            {
                app.UseHsts();
            }

            app.MigrateMssqlDb();

            app.UseHttpsRedirection();

            app.UseCors(options => options
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
