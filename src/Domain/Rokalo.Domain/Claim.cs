namespace Rokalo.Domain
{
    using System;

    public class Claim
    {
        public Claim(
            Guid id,
            Guid userId,
            string value )
        {
            this.Id = id;
            this.UserId = userId;
            this.Value = value;
        }

        public Guid Id { get; protected set; }
        public Guid UserId { get; protected set; }
        public string Value { get; protected set; }
    }
}
