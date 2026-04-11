using QRPayCheck.Domain.Entities.Base;

namespace QRPayCheck.Domain.Entities;

public sealed class Branch : TenantAwareEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;

    public Tenant Tenant { get; set; } = null!;
    public ICollection<Menu> Menus { get; set; } = [];
}
