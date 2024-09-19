namespace Rokalo.Application.Contracts
{
    using Rokalo.Domain;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUserRepository
    {
        Task<User> GetByIdSafeAsync(Guid id, CancellationToken cancellationToken);

        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        void Add(User user);

        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

        Task<User> GetByEmailSafeAsync(string email, CancellationToken cancellationToken);

        void Update(User user);
    }
}
