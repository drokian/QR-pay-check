using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.Categories.Commands;

public static class DeleteCategoryHandler
{
    public static async Task Handle(
        DeleteCategoryCommand command,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == command.Id, ct)
            ?? throw new KeyNotFoundException($"Kategori bulunamadı: {command.Id}");

        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }
}
