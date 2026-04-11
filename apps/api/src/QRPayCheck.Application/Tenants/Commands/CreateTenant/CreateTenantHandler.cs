using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;
using QRPayCheck.Domain.Entities;

namespace QRPayCheck.Application.Tenants.Commands.CreateTenant;

public static class CreateTenantHandler
{
    public static async Task<TenantDto> Handle(
        CreateTenantCommand command,
        IApplicationDbContext db,
        ICurrentUserContext currentUser,
        CancellationToken ct)
    {
        var slugExists = await db.Tenants
            .AnyAsync(t => t.Slug == command.Slug, ct);

        if (slugExists)
            throw new InvalidOperationException($"'{command.Slug}' slug'ı zaten kullanımda.");

        var tenant = new Tenant
        {
            Name = command.Name,
            Slug = command.Slug,
            Phone = command.Phone,
            Email = command.Email,
            TaxNumber = command.TaxNumber
        };

        db.Tenants.Add(tenant);

        // Restoran sahibinin kullanıcı kaydını tenant'a bağla
        if (currentUser.KeycloakId is not null)
        {
            var user = await db.Users
                .FirstOrDefaultAsync(u => u.KeycloakId == currentUser.KeycloakId, ct);

            if (user is not null)
            {
                user.TenantId = tenant.Id;
                user.UpdatedAt = DateTime.UtcNow;
            }
        }

        await db.SaveChangesAsync(ct);
        return tenant.Adapt<TenantDto>();
    }
}
