using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.MenuItemOptions.Commands;

public static class DeleteMenuItemOptionHandler
{
    public static async Task Handle(
        DeleteMenuItemOptionCommand command,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var option = await db.MenuItemOptions.FirstOrDefaultAsync(o => o.Id == command.Id, ct)
            ?? throw new KeyNotFoundException($"Seçenek bulunamadı: {command.Id}");

        db.MenuItemOptions.Remove(option);
        await db.SaveChangesAsync(ct);
    }
}
