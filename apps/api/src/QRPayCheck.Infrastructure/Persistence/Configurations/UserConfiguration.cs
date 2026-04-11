using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRPayCheck.Domain.Entities;

namespace QRPayCheck.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.KeycloakId).HasMaxLength(100).IsRequired();
        builder.HasIndex(u => u.KeycloakId).IsUnique();
        builder.Property(u => u.Phone).HasMaxLength(20);
        builder.HasIndex(u => u.Phone).IsUnique().HasFilter("\"Phone\" IS NOT NULL");
        builder.Property(u => u.FullName).HasMaxLength(200);
        builder.Property(u => u.Role).HasMaxLength(50).IsRequired();
    }
}
