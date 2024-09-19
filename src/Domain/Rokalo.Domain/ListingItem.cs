namespace Rokalo.Domain
{
    using Rokalo.Domain.Enums;
    using System;
    using System.Collections.Generic;

    public class ListingItem
    {
        public ListingItem() { }

        public Guid Id { get; protected set; }
        public Guid ProfileId { get; protected set; }
        public Guid CategoryId { get; protected set; }
        public Guid CityId { get; protected set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ListingStatus Status { get; set; } = ListingStatus.Available;
        public ItemCondition Condition { get; set; }

        // add this and enable file upload
        //public List<string> ImageUrls { get; set; } = new List<string>();
        public List<string> Tags { get; set; } = new List<string>();
        public string ContactPhone { get; set; } = default!;
        public string ContactEmail { get; set; } = default!;
    }
}
