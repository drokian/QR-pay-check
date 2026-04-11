using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.Menus.Commands;

public static class DeleteMenuHandler
{
    public static async Task Handle(
        DeleteMenuCommand command,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var menu = await db.Menus.FirstOrDefaultAsync(m => m.Id == command.Id, ct)
            ?? throw new KeyNotFoundException($"Menü bulunamadı: {command.Id}");

        menu.IsActive = false;
        menu.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }
}
