namespace Rokalo.Infrastructure.Db.Users.Repositories
{
    using Microsoft.EntityFrameworkCore;
    using Rokalo.Application.Contracts;
    using Rokalo.Domain;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly DbSet<RefreshToken> refreshTokens;

        public RefreshTokenRepository(UsersDbContext usersDbContext)
        {
            this.refreshTokens = usersDbContext.Set<RefreshToken>();
        }

        public void Add(RefreshToken refreshToken)
        {
            this.refreshTokens.Add(refreshToken);
        }

        public void Delete(List<RefreshToken> refreshTokens)
        {
            this.refreshTokens.RemoveRange(refreshTokens);
        }

        public async Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await this.refreshTokens.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var tokens = await this.refreshTokens.Where(x => x.UserId == userId).ToListAsync(cancellationToken);
            return tokens;
        }
    }
}
