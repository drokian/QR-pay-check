namespace QRPayCheck.Application.MenuItems;

public sealed record MenuItemDto(
    Guid Id,
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    bool IsAvailable,
    int? PreparationTime,
    string? Allergens,
    int SortOrder,
    bool IsActive
);
