namespace QRPayCheck.Application.Common.Interfaces;

public interface ICurrentUserContext
{
    /// <summary>Keycloak sub claim.</summary>
    string? KeycloakId { get; }
    string? Email { get; }
    string? FullName { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
