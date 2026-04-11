using Mapster;
using QRPayCheck.Application.Common.Interfaces;
using QRPayCheck.Domain.Entities;

namespace QRPayCheck.Application.Branches.Commands.CreateBranch;

public static class CreateBranchHandler
{
    public static async Task<BranchDto> Handle(
        CreateBranchCommand command,
        IApplicationDbContext db,
        ITenantContext tenantContext,
        CancellationToken ct)
    {
        var tenantId = tenantContext.TenantId
            ?? throw new UnauthorizedAccessException("Tenant bağlamı bulunamadı.");

        var branch = new Branch
        {
            TenantId = tenantId,
            Name = command.Name,
            Address = command.Address,
            City = command.City,
            District = command.District,
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Phone = command.Phone
        };

        db.Branches.Add(branch);
        await db.SaveChangesAsync(ct);
        return branch.Adapt<BranchDto>();
    }
}
