namespace QRPayCheck.Application.MenuItemOptions;

public sealed record MenuItemOptionDto(
    Guid Id,
    Guid MenuItemId,
    string GroupName,
    string Name,
    decimal PriceModifier,
    bool IsDefault,
    bool IsRequired,
    int MaxSelections,
    int SortOrder
);
