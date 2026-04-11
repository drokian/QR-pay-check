using Microsoft.EntityFrameworkCore;
using QRPayCheck.Domain.Entities;

namespace QRPayCheck.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Tenant> Tenants { get; }
    DbSet<Branch> Branches { get; }
    DbSet<User> Users { get; }
    DbSet<Menu> Menus { get; }
    DbSet<Category> Categories { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<MenuItemOption> MenuItemOptions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
