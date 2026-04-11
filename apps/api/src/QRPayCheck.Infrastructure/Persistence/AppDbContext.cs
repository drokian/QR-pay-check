using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;
using QRPayCheck.Domain.Entities;
#pragma warning disable CS8618

namespace QRPayCheck.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    private readonly ITenantContext _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuItemOption> MenuItemOptions => Set<MenuItemOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Multi-tenancy Global Query Filter
        // TenantId null ise (platform-admin veya Tenant oluşturma aşaması) filtre devre dışı
        modelBuilder.Entity<Branch>()
            .HasQueryFilter(b => _tenantContext.TenantId == null || b.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<Menu>()
            .HasQueryFilter(m => _tenantContext.TenantId == null || m.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<Category>()
            .HasQueryFilter(c => _tenantContext.TenantId == null || c.TenantId == _tenantContext.TenantId);

        modelBuilder.Entity<MenuItem>()
            .HasQueryFilter(m => _tenantContext.TenantId == null || m.TenantId == _tenantContext.TenantId);

        // MenuItemOption'ın MenuItem ile ilişkisi zorunlu (required FK) ve MenuItem filtrelidir.
        // EF Core uyarısını gidermek için MenuItemOption da MenuItem'ın tenant filtresiyle hizalanır.
        modelBuilder.Entity<MenuItemOption>()
            .HasQueryFilter(o => _tenantContext.TenantId == null || o.MenuItem.TenantId == _tenantContext.TenantId);
    }
}
