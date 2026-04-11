namespace QRPayCheck.Application.Branches;

public sealed record BranchDto(
    Guid Id,
    Guid TenantId,
    string Name,
    string? Address,
    string? City,
    string? District,
    decimal? Latitude,
    decimal? Longitude,
    string? Phone,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
