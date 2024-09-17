namespace Rokalo.Infrastructure.Db.Users
{
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;

    internal sealed class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) :base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
