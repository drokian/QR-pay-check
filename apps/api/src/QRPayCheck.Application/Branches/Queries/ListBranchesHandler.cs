using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.Branches.Queries;

public sealed record ListBranchesQuery;

public static class ListBranchesHandler
{
    public static async Task<IReadOnlyList<BranchDto>> Handle(
        ListBranchesQuery query,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        return await db.Branches
            .AsNoTracking()
            .OrderBy(b => b.Name)
            .ProjectToType<BranchDto>()
            .ToListAsync(ct);
    }
}
