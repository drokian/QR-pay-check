using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.MenuItems.Commands;

public static class DeleteMenuItemHandler
{
    public static async Task Handle(
        DeleteMenuItemCommand command,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var item = await db.MenuItems.FirstOrDefaultAsync(m => m.Id == command.Id, ct)
            ?? throw new KeyNotFoundException($"Ürün bulunamadı: {command.Id}");

        item.IsActive = false;
        item.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }
}
