using QRPayCheck.Domain.Entities.Base;

namespace QRPayCheck.Domain.Entities;

public sealed class MenuItem : TenantAwareEntity
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsAvailable { get; set; } = true;
    public int? PreparationTime { get; set; }
    public string? Allergens { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public Category Category { get; set; } = null!;
    public ICollection<MenuItemOption> Options { get; set; } = [];
}
