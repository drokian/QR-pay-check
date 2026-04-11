using QRPayCheck.Domain.Entities.Base;

namespace QRPayCheck.Domain.Entities;

public sealed class Menu : TenantAwareEntity
{
    public Guid BranchId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    public Branch Branch { get; set; } = null!;
    public ICollection<Category> Categories { get; set; } = [];
}
