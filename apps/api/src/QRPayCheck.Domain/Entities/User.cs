using QRPayCheck.Domain.Entities.Base;

namespace QRPayCheck.Domain.Entities;

public sealed class User : BaseEntity
{
    public string KeycloakId { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public string? Phone { get; set; }
    public string? FullName { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    public Tenant? Tenant { get; set; }
}
