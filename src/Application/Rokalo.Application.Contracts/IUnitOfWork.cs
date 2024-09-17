namespace Rokalo.Application.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken cancellationToken);

        IUserRepository Users { get; }

        IRefreshTokenRepository RefreshTokens { get; }

    }
}
