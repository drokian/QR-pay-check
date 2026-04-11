using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.Menus.Queries;

public sealed record GetMenuQuery(Guid Id);
public sealed record ListMenusQuery(Guid BranchId);

public static class GetMenuHandler
{
    public static async Task<MenuDto?> Handle(GetMenuQuery query, IApplicationDbContext db, CancellationToken ct)
        => (await db.Menus.AsNoTracking().FirstOrDefaultAsync(m => m.Id == query.Id, ct))
            ?.Adapt<MenuDto>();
}

public static class ListMenusHandler
{
    public static async Task<IReadOnlyList<MenuDto>> Handle(ListMenusQuery query, IApplicationDbContext db, CancellationToken ct)
        => await db.Menus
            .AsNoTracking()
            .Where(m => m.BranchId == query.BranchId)
            .OrderBy(m => m.SortOrder)
            .ProjectToType<MenuDto>()
            .ToListAsync(ct);
}
