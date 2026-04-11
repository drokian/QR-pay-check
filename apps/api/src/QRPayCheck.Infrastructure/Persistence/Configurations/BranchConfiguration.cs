using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRPayCheck.Domain.Entities;

namespace QRPayCheck.Infrastructure.Persistence.Configurations;

public sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Name).HasMaxLength(200).IsRequired();
        builder.Property(b => b.Address).HasColumnType("text");
        builder.Property(b => b.City).HasMaxLength(100);
        builder.Property(b => b.District).HasMaxLength(100);
        builder.Property(b => b.Latitude).HasPrecision(9, 6);
        builder.Property(b => b.Longitude).HasPrecision(9, 6);
        builder.Property(b => b.Phone).HasMaxLength(20);

        builder.HasIndex(b => b.TenantId);

        builder.HasMany(b => b.Menus)
            .WithOne(m => m.Branch)
            .HasForeignKey(m => m.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
