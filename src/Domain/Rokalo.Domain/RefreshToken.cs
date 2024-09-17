namespace Rokalo.Domain
{
    using System;

    public class RefreshToken
    {
        public RefreshToken(
            Guid id,
            Guid userId,
            DateTime createdAt,
            DateTime expiresAt)
        {
            this.Id = id;
            this.UserId = userId;
            this.CreatedAt = createdAt;
            this.ExpiresAt = expiresAt;
        }

        public Guid Id { get; protected set; }

        public Guid UserId { get; protected set; }

        public DateTime CreatedAt { get; protected set; }

        public DateTime ExpiresAt { get; protected set; }
    }
}
