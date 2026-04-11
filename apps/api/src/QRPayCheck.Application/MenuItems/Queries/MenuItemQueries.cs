using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.MenuItems.Queries;

public sealed record ListMenuItemsQuery(Guid CategoryId);

public static class ListMenuItemsHandler
{
    public static async Task<IReadOnlyList<MenuItemDto>> Handle(ListMenuItemsQuery query, IApplicationDbContext db, CancellationToken ct)
        => await db.MenuItems
            .AsNoTracking()
            .Where(m => m.CategoryId == query.CategoryId)
            .OrderBy(m => m.SortOrder)
            .ProjectToType<MenuItemDto>()
            .ToListAsync(ct);
}
