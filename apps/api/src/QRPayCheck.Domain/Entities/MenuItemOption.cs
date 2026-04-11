using QRPayCheck.Domain.Entities.Base;

namespace QRPayCheck.Domain.Entities;

public sealed class MenuItemOption : BaseEntity
{
    public Guid MenuItemId { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
    public bool IsDefault { get; set; }
    public bool IsRequired { get; set; }
    public int MaxSelections { get; set; } = 1;
    public int SortOrder { get; set; }

    public MenuItem MenuItem { get; set; } = null!;
}
