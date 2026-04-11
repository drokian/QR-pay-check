namespace QRPayCheck.Application.Auth;

public sealed record UserProfileDto(
    Guid Id,
    string KeycloakId,
    Guid? TenantId,
    string? Phone,
    string? FullName,
    string Role,
    DateTime CreatedAt
);
