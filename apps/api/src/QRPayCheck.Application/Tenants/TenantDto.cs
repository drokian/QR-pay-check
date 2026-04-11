namespace QRPayCheck.Application.Tenants;

public sealed record TenantDto(
    Guid Id,
    string Name,
    string Slug,
    string? LogoUrl,
    string? Phone,
    string? Email,
    string? TaxNumber,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
