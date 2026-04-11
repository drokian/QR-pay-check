using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.Tenants.Commands.UpdateTenant;

public static class UpdateTenantHandler
{
    public static async Task<TenantDto> Handle(
        UpdateTenantCommand command,
        IApplicationDbContext db,
        ITenantContext tenantContext,
        CancellationToken ct)
    {
        var tenantId = tenantContext.TenantId
            ?? throw new UnauthorizedAccessException("Tenant bağlamı bulunamadı.");

        var tenant = await db.Tenants.FirstOrDefaultAsync(t => t.Id == tenantId, ct)
            ?? throw new KeyNotFoundException($"Tenant bulunamadı: {tenantId}");

        tenant.Name = command.Name;
        tenant.Phone = command.Phone;
        tenant.Email = command.Email;
        tenant.TaxNumber = command.TaxNumber;
        tenant.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return tenant.Adapt<TenantDto>();
    }
}
