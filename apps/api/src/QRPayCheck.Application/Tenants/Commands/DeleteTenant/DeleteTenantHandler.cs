using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.Tenants.Commands.DeleteTenant;

public static class DeleteTenantHandler
{
    public static async Task Handle(
        DeleteTenantCommand command,
        IApplicationDbContext db,
        ITenantContext tenantContext,
        CancellationToken ct)
    {
        var tenantId = tenantContext.TenantId
            ?? throw new UnauthorizedAccessException("Tenant bağlamı bulunamadı.");

        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct)
            ?? throw new KeyNotFoundException($"Tenant bulunamadı: {tenantId}");

        tenant.IsActive = false;
        tenant.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }
}
