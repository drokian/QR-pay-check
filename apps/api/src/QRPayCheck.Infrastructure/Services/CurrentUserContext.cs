using System.Security.Claims;
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

        Role = u.FindFirstValue(ClaimTypes.Role)
            ?? u.FindFirstValue("realm_access")
            ?? u.FindFirstValue("roles");
    }
}
