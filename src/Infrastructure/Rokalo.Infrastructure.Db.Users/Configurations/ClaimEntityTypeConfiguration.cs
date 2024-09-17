namespace Rokalo.Infrastructure.Db.Users.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Rokalo.Domain;

    internal sealed class ClaimEntityTypeConfiguration : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> builder)
        {
            builder.ToTable("Claims");

            builder.HasKey(key => key.Id);

            builder.Property(p => p.UserId).IsRequired();

            builder.Property(p => p.Value).IsRequired();
        }
    }
}
