using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.Tenants.Queries;

public sealed record GetMyTenantQuery;

public static class GetMyTenantHandler
{
    public static async Task<TenantDto?> Handle(
        GetMyTenantQuery query,
        IApplicationDbContext db,
        ITenantContext tenantContext,
        CancellationToken ct)
    {
        if (tenantContext.TenantId is null)
            return null;

        var tenant = await db.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == tenantContext.TenantId, ct);

        return tenant?.Adapt<TenantDto>();
    }
}
