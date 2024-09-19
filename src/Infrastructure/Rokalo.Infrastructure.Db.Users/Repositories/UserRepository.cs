namespace Rokalo.Infrastructure.Db.Users.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Rokalo.Application.Contracts;
    using Rokalo.Blocks.Common.Exceptions;
    using Rokalo.Domain;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class UserRepository : IUserRepository
    {
        private readonly DbSet<User> users;

        public UserRepository(UsersDbContext context)
        {
            this.users = context.Set<User>();
        }

        public void Add(User user)
        {
            this.users.Add(user);
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await this.users.Where(user => user.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User> GetByIdSafeAsync(Guid id, CancellationToken cancellationToken)
        {
            return await this.users.FindAsync(new object[] { id }, cancellationToken) ?? throw new ServiceValidationException("Unable to find that user.");
        }

        public void Update(User user)
        {
            this.users.Update(user);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await this.users.FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
        }

        public async Task<User> GetByEmailSafeAsync(string email, CancellationToken cancellationToken)
        {
            return await this.GetByEmailAsync(email, cancellationToken) ?? throw new ServiceValidationException("User not found");
        }
    }
}
