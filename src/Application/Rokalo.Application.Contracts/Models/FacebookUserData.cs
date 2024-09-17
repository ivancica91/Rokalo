namespace Rokalo.Application.Contracts.Models
{
    using System;

    public class FacebookUserData
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = default!;

        public string Email { get; set; } = default!;
    }
}
