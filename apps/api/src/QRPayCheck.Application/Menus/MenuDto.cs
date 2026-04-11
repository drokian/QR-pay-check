namespace QRPayCheck.Application.Menus;

public sealed record MenuDto(
    Guid Id,
    Guid BranchId,
    string Name,
    bool IsActive,
    int SortOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
