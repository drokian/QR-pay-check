using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.Branches.Queries;

public sealed record GetBranchQuery(Guid Id);

public static class GetBranchHandler
{
    public static async Task<BranchDto?> Handle(
        GetBranchQuery query,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var branch = await db.Branches
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == query.Id, ct);

        return branch?.Adapt<BranchDto>();
    }
}
