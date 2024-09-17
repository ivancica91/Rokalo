namespace Rokalo.Infrastructure.Db.Users.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Rokalo.Domain;

    internal sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(key => key.Id);

            builder.Property(p => p.Email).IsRequired();

            builder.Property(p => p.Password).IsRequired();

            builder.Property(p => p.IsEmailVerified).IsRequired();

            builder.Property(p => p.EmailVerificationCode).IsRequired();

            builder.HasOne(x => x.Profile);

            builder.HasMany(x => x.Claims).WithOne().HasForeignKey(x => x.UserId);
        }
    }
}
