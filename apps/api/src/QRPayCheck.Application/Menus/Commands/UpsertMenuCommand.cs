using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;
using QRPayCheck.Domain.Entities;

namespace QRPayCheck.Application.Menus.Commands;

public sealed record CreateMenuCommand(Guid BranchId, string Name, int SortOrder = 0);
public sealed record UpdateMenuCommand(Guid Id, string Name, bool IsActive, int SortOrder);

public sealed class CreateMenuCommandValidator : AbstractValidator<CreateMenuCommand>
{
    public CreateMenuCommandValidator()
    {
        RuleFor(x => x.BranchId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public sealed class UpdateMenuCommandValidator : AbstractValidator<UpdateMenuCommand>
{
    public UpdateMenuCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public static class CreateMenuHandler
{
    public static async Task<MenuDto> Handle(
        CreateMenuCommand command,
        IApplicationDbContext db,
        ITenantContext tenantContext,
        CancellationToken ct)
    {
        var tenantId = tenantContext.TenantId
            ?? throw new UnauthorizedAccessException("Tenant bağlamı bulunamadı.");

        var branchExists = await db.Branches
            .AnyAsync(b => b.Id == command.BranchId, ct);

        if (!branchExists)
            throw new KeyNotFoundException($"Şube bulunamadı: {command.BranchId}");

        var menu = new Menu
        {
            TenantId = tenantId,
            BranchId = command.BranchId,
            Name = command.Name,
            SortOrder = command.SortOrder
        };

        db.Menus.Add(menu);
        await db.SaveChangesAsync(ct);
        return menu.Adapt<MenuDto>();
    }
}

public static class UpdateMenuHandler
{
    public static async Task<MenuDto> Handle(
        UpdateMenuCommand command,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var menu = await db.Menus.FirstOrDefaultAsync(m => m.Id == command.Id, ct)
            ?? throw new KeyNotFoundException($"Menü bulunamadı: {command.Id}");

        menu.Name = command.Name;
        menu.IsActive = command.IsActive;
        menu.SortOrder = command.SortOrder;
        menu.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return menu.Adapt<MenuDto>();
    }
}
