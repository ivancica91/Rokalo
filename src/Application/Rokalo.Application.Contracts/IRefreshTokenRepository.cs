namespace Rokalo.Application.Contracts
{
    using Rokalo.Domain;
    using System.Threading.Tasks;
    using System;
    using System.Threading;
    using System.Collections.Generic;

    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<List<RefreshToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

        void Add(RefreshToken refreshToken);

        void Delete(List<RefreshToken> refreshTokens);

    }
}
