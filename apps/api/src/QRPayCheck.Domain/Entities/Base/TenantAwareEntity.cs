namespace QRPayCheck.Domain.Entities.Base;

/// <summary>
/// TenantId doğrudan kolonda tutulur — EF Core Global Query Filter performansı için denormalize.
/// </summary>
public abstract class TenantAwareEntity : BaseEntity
{
    public Guid TenantId { get; set; }
}
