using Microsoft.AspNetCore.Http;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Infrastructure.Services;

/// <summary>
/// JWT'deki "tenant_id" claim'inden TenantId okur.
/// Keycloak'ta restoran sahiplerine özel olarak set edilen custom claim.
/// </summary>
public sealed class TenantContext : ITenantContext
{
    public Guid? TenantId { get; }

    public TenantContext(IHttpContextAccessor httpContextAccessor)
    {
        var claim = httpContextAccessor.HttpContext?.User
            .FindFirst("tenant_id")?.Value;

        if (Guid.TryParse(claim, out var tenantId))
            TenantId = tenantId;
    }
}
