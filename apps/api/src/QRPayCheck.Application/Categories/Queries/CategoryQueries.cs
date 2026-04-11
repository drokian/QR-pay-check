using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.Categories.Queries;

public sealed record ListCategoriesQuery(Guid MenuId);

public static class ListCategoriesHandler
{
    public static async Task<IReadOnlyList<CategoryDto>> Handle(ListCategoriesQuery query, IApplicationDbContext db, CancellationToken ct)
        => await db.Categories
            .AsNoTracking()
            .Where(c => c.MenuId == query.MenuId)
            .OrderBy(c => c.SortOrder)
            .ProjectToType<CategoryDto>()
            .ToListAsync(ct);
}
