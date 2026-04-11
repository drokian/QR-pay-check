namespace QRPayCheck.Application.Common.Interfaces;

public interface ITenantContext
{
    /// <summary>
    /// JWT'den çıkarılan tenant_id claim. Kimlik doğrulanmamış veya platform-admin için null.
    /// </summary>
    Guid? TenantId { get; }
}
