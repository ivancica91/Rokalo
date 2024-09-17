namespace Rokalo.Infrastructure.Db.Users
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using System;

    internal sealed class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
    {
        public UsersDbContext CreateDbContext(string[] args)
        {
            IConfiguration configuration = BuildConfiguration(args);

            var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
            var connectionString = configuration.GetSection(args[0]).Value;

            optionsBuilder
                .UseSqlServer(connectionString);

            var instance = new UsersDbContext(optionsBuilder.Options);

            if (instance is null )
            {
                throw new InvalidOperationException($"Unable to initialize {nameof(UsersDbContext)} instance.");
            }

            return instance;
        }


        private static IConfigurationRoot BuildConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(args[2])
                .AddJsonFile(
                    path: "appsettings.json",
                    optional: false,
                    reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets(args[1])
                .AddCommandLine(args)
                .Build();
        }
    }
}
