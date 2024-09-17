namespace Rokalo.Infrastructure.Db.Users.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Rokalo.Domain;

    internal sealed class ProfileEntityTypeConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.ToTable("Profiles");

            builder.HasKey(x => x.Id);

            builder.Property(p => p.FirstName);

            builder.Property(p => p.LastName);

            builder.Property(p => p.Number);

            builder.Property(p => p.Mobile);

            builder.Property(p => p.Oib).HasMaxLength(11);
        }
    }
}
