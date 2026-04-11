using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.Branches.Commands.UpdateBranch;

public static class UpdateBranchHandler
{
    public static async Task<BranchDto> Handle(
        UpdateBranchCommand command,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var branch = await db.Branches.FirstOrDefaultAsync(b => b.Id == command.Id, ct)
            ?? throw new KeyNotFoundException($"Şube bulunamadı: {command.Id}");

        branch.Name = command.Name;
        branch.Address = command.Address;
        branch.City = command.City;
        branch.District = command.District;
        branch.Latitude = command.Latitude;
        branch.Longitude = command.Longitude;
        branch.Phone = command.Phone;
        branch.IsActive = command.IsActive;
        branch.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return branch.Adapt<BranchDto>();
    }
}
