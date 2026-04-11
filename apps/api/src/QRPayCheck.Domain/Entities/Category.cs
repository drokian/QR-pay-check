using QRPayCheck.Domain.Entities.Base;

namespace QRPayCheck.Domain.Entities;

public sealed class Category : TenantAwareEntity
{
    public Guid MenuId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public Menu Menu { get; set; } = null!;
    public ICollection<MenuItem> MenuItems { get; set; } = [];
}
