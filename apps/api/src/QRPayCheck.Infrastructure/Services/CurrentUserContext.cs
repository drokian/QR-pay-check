using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Infrastructure.Services;

public sealed class CurrentUserContext : ICurrentUserContext
{
    public string? KeycloakId { get; }
    public string? Email { get; }
    public string? FullName { get; }
    public string? Role { get; }
    public bool IsAuthenticated { get; }

    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;

        if (!IsAuthenticated) return;

        var u = user!;

        KeycloakId = u.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? u.FindFirstValue("sub");

        Email = u.FindFirstValue(ClaimTypes.Email)
            ?? u.FindFirstValue("email");

        var givenName = u.FindFirstValue(ClaimTypes.GivenName) ?? u.FindFirstValue("given_name");
        var familyName = u.FindFirstValue(ClaimTypes.Surname) ?? u.FindFirstValue("family_name");
        FullName = string.IsNullOrWhiteSpace($"{givenName} {familyName}".Trim())
            ? null
            : $"{givenName} {familyName}".Trim();

        Role = ParseRole(u);
    }

    private static string? ParseRole(ClaimsPrincipal user)
    {
        // 1. Standart rol claim'i (JwtBearer bazı konfigürasyonlarda map eder)
        var standardRole = user.FindFirstValue(ClaimTypes.Role);
        if (!string.IsNullOrWhiteSpace(standardRole))
            return Truncate(standardRole, 50);

        // 2. Keycloak realm_access claim: {"roles":["restaurant-owner"]}
        var realmAccess = user.FindFirstValue("realm_access");
        if (!string.IsNullOrWhiteSpace(realmAccess))
        {
            try
            {
                using var doc = JsonDocument.Parse(realmAccess);
                if (doc.RootElement.TryGetProperty("roles", out var rolesElement))
                {
                    foreach (var role in rolesElement.EnumerateArray())
                    {
                        var roleName = role.GetString();
                        if (!string.IsNullOrWhiteSpace(roleName))
                            return Truncate(roleName, 50);
                    }
                }
            }
            catch (JsonException)
            {
                // JSON değil; ham string olarak kullan
                return Truncate(realmAccess, 50);
            }
        }

        return null;
    }

    private static string Truncate(string value, int maxLength)
        => value.Length <= maxLength ? value : value[..maxLength];
}
