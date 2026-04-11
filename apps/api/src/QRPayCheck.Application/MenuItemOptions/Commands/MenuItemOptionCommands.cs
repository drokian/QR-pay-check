using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Domain.Entities;
using QRPayCheck.Application.Common.Interfaces;

namespace QRPayCheck.Application.MenuItemOptions.Commands;

public sealed record CreateMenuItemOptionCommand(
    Guid MenuItemId,
    string GroupName,
    string Name,
    decimal PriceModifier = 0,
    bool IsDefault = false,
    bool IsRequired = false,
    int MaxSelections = 1,
    int SortOrder = 0
);

public sealed record UpdateMenuItemOptionCommand(
    Guid Id,
    string GroupName,
    string Name,
    decimal PriceModifier,
    bool IsDefault,
    bool IsRequired,
    int MaxSelections,
    int SortOrder
);

public sealed class CreateMenuItemOptionCommandValidator : AbstractValidator<CreateMenuItemOptionCommand>
{
    public CreateMenuItemOptionCommandValidator()
    {
        RuleFor(x => x.MenuItemId).NotEmpty();
        RuleFor(x => x.GroupName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MaxSelections).GreaterThan(0);
    }
}

public static class CreateMenuItemOptionHandler
{
    public static async Task<MenuItemOptionDto> Handle(
        CreateMenuItemOptionCommand command,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var itemExists = await db.MenuItems.AnyAsync(m => m.Id == command.MenuItemId, ct);
        if (!itemExists)
            throw new KeyNotFoundException($"Ürün bulunamadı: {command.MenuItemId}");

        var option = new MenuItemOption
        {
            MenuItemId = command.MenuItemId,
            GroupName = command.GroupName,
            Name = command.Name,
            PriceModifier = command.PriceModifier,
            IsDefault = command.IsDefault,
            IsRequired = command.IsRequired,
            MaxSelections = command.MaxSelections,
            SortOrder = command.SortOrder
        };

        db.MenuItemOptions.Add(option);
        await db.SaveChangesAsync(ct);
        return option.Adapt<MenuItemOptionDto>();
    }
}

public static class UpdateMenuItemOptionHandler
{
    public static async Task<MenuItemOptionDto> Handle(
        UpdateMenuItemOptionCommand command,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var option = await db.MenuItemOptions.FirstOrDefaultAsync(o => o.Id == command.Id, ct)
            ?? throw new KeyNotFoundException($"Seçenek bulunamadı: {command.Id}");

        option.GroupName = command.GroupName;
        option.Name = command.Name;
        option.PriceModifier = command.PriceModifier;
        option.IsDefault = command.IsDefault;
        option.IsRequired = command.IsRequired;
        option.MaxSelections = command.MaxSelections;
        option.SortOrder = command.SortOrder;

        await db.SaveChangesAsync(ct);
        return option.Adapt<MenuItemOptionDto>();
    }
}
