using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRPayCheck.Domain.Entities;

namespace QRPayCheck.Infrastructure.Persistence.Configurations;

public sealed class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).HasMaxLength(200).IsRequired();
        builder.Property(m => m.Description).HasColumnType("text");
        builder.Property(m => m.Price).HasPrecision(10, 2);
        builder.Property(m => m.ImageUrl).HasMaxLength(500);
        builder.Property(m => m.Allergens).HasColumnType("text");
        builder.HasIndex(m => m.TenantId);

        builder.HasMany(m => m.Options)
            .WithOne(o => o.MenuItem)
            .HasForeignKey(o => o.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
