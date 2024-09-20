namespace Rokalo.Infrastructure.Security
{
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Rokalo.Application.Contracts.Security;
    using Rokalo.Infrastructure.Security.Services;
    using System.Text;

    public class SecurityAdapterSettingsFactory : IConfigureOptions<SecurityAdapterConfigurations>
    {
        private readonly IConfiguration _configuration;

        public SecurityAdapterSettingsFactory(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public void Configure(SecurityAdapterConfigurations options)
        {
            this._configuration.GetSection(SecurityAdapterConfigurations.Key).Bind(options);
        }
    }

    public static class DependencyInjection
    {
        public static IServiceCollection AddSecurityAdapter(
            this IServiceCollection services,
            SecurityAdapterConfigurations configurations)
        {
            services.ConfigureOptions<SecurityAdapterSettingsFactory>();
            services.AddScoped<IPasswordHashingService, PasswordHashingService>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddHttpClient<IFacebookOAuthService, FacebookOAuthService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = configurations.JwtTokenConfiguration.Issuer,
                        ValidAudience = configurations.JwtTokenConfiguration.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurations.JwtTokenConfiguration.Secret)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true
                    };
                })
                .AddFacebook( o =>
                {
                    o.AppId = configurations.FacebookOAuthConfiguration.AppId;
                    o.AppSecret = configurations.FacebookOAuthConfiguration.AppSecret;
                });

            return services;
        }
    }

    public class SecurityAdapterConfigurations
    {
        public const string Key = nameof(SecurityAdapterConfigurations);

        public JwtTokenConfiguration JwtTokenConfiguration { get; set; } = default!;

        public RefreshTokenConfiguration RefreshTokenConfiguration { get; set; } = default!;

        public FacebookOAuthConfiguration FacebookOAuthConfiguration { get; set; } = default!;
    }

    public class JwtTokenConfiguration
    {
        public const string Key = nameof(JwtTokenConfiguration);
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public int ExpiresInMinutes { get; set; }
        public string Secret { get; set; } = default!;
    }
    public class RefreshTokenConfiguration
    {
        public const string Key = nameof(RefreshTokenConfiguration);

        public int ValidForDays { get; set; }
    }

    public class FacebookOAuthConfiguration
    {
        public string AppId { get; set; } = default!;
        public string AppSecret { get; set; } = default!;
    }
}
