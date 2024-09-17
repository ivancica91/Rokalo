namespace Rokalo.Infrastructure.Db.Users
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Rokalo.Application.Contracts;
    using Rokalo.Infrastructure.Db.Users.Repositories;
    using System;
    using System.Linq;

    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureUsersConfiguration(this IServiceCollection services, MssqlSettings settings)
        {
            services.AddDbContext<UsersDbContext>(options => options.UseSqlServer(settings.ConnectionString));

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            return services;
        }

        public static IApplicationBuilder MigrateMssqlDb(this IApplicationBuilder builder)
        {
            using var scope = builder.ApplicationServices.CreateScope();

            using var dbContext = scope.ServiceProvider.GetService<UsersDbContext>();

            if (dbContext is null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (dbContext.Database.GetPendingMigrations().Count() > 0)
            {
                dbContext.Database.Migrate();
            }

            return builder;
        }
    }

    public class MssqlSettings
    {
        public const string Key = nameof(MssqlSettings);
        public string ConnectionString { get; set; } = default;
    }
}
