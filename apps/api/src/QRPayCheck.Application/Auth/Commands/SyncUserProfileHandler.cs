using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;
using QRPayCheck.Domain.Entities;

namespace QRPayCheck.Application.Auth.Commands;

/// <summary>
/// İlk giriş veya profil isteğinde Keycloak kullanıcısını yerel DB ile senkronize eder.
/// </summary>
public sealed record SyncUserProfileCommand;

public static class SyncUserProfileHandler
{
    public static async Task<UserProfileDto> Handle(
        SyncUserProfileCommand command,
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        CancellationToken ct)
    {
        if (!currentUser.IsAuthenticated || currentUser.KeycloakId is null)
            throw new UnauthorizedAccessException("Kimlik doğrulama gereklidir.");

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.KeycloakId == currentUser.KeycloakId, ct);

        if (user is null)
        {
            user = new User
            {
                KeycloakId = currentUser.KeycloakId,
                FullName = currentUser.FullName,
                Role = currentUser.Role ?? "customer",
                LastLoginAt = DateTime.UtcNow
            };
            db.Users.Add(user);
        }
        else
        {
            user.FullName = currentUser.FullName ?? user.FullName;
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync(ct);
        return user.Adapt<UserProfileDto>();
    }
}
