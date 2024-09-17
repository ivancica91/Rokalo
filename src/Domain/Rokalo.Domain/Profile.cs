namespace Rokalo.Domain
{
    using System;

    public class Profile
    {
        public Profile(
            Guid id,
            Guid userId,
            string? firstName,
            string? lastName,
            string? number,
            string? mobile,
            string? oib
            )
        {
            this.Id = id;
            this.UserId = userId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Number = number;
            this.Mobile = mobile;
            this.Oib = oib;
        }

        public Guid Id { get; protected set; }
        public Guid UserId { get; protected set; }
        public string? FirstName { get; protected set; }
        public string? LastName { get; protected set; }
        public string? Number { get; protected set; }
        public string? Mobile { get; protected set; }
        public string? Oib { get; protected set; }

        public void Update(string? firstName, string? lastName, string? number, string? mobile, string? oib)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Number = number;
            this.Mobile = mobile;
            this.Oib = oib;
        }
    }
}
