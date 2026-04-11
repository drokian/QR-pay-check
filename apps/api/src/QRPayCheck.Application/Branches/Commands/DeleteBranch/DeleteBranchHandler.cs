using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.Branches.Commands.DeleteBranch;

public static class DeleteBranchHandler
{
    public static async Task Handle(
        DeleteBranchCommand command,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var branch = await db.Branches.FirstOrDefaultAsync(b => b.Id == command.Id, ct)
            ?? throw new KeyNotFoundException($"Şube bulunamadı: {command.Id}");

        branch.IsActive = false;
        branch.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
    }
}
