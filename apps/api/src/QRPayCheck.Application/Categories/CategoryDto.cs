namespace QRPayCheck.Application.Categories;

public sealed record CategoryDto(
    Guid Id,
    Guid MenuId,
    string Name,
    string? Description,
    string? ImageUrl,
    int SortOrder,
    bool IsActive
);
