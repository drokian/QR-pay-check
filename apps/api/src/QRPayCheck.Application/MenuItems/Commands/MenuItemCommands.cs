using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;
using QRPayCheck.Domain.Entities;

namespace QRPayCheck.Application.MenuItems.Commands;

public sealed record CreateMenuItemCommand(
    Guid CategoryId,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    bool IsAvailable = true,
    int? PreparationTime = null,
    string? Allergens = null,
    int SortOrder = 0
);

public sealed record UpdateMenuItemCommand(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string? ImageUrl,
    bool IsAvailable,
    int? PreparationTime,
    string? Allergens,
    int SortOrder,
    bool IsActive
);

public sealed class CreateMenuItemCommandValidator : AbstractValidator<CreateMenuItemCommand>
{
    public CreateMenuItemCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateMenuItemCommandValidator : AbstractValidator<UpdateMenuItemCommand>
{
    public UpdateMenuItemCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
    }
}

public static class CreateMenuItemHandler
{
    public static async Task<MenuItemDto> Handle(
        CreateMenuItemCommand command,
        IApplicationDbContext db,
        ITenantContext tenantContext,
        CancellationToken ct)
    {
        var tenantId = tenantContext.TenantId
            ?? throw new UnauthorizedAccessException("Tenant bağlamı bulunamadı.");

        var categoryExists = await db.Categories.AnyAsync(c => c.Id == command.CategoryId, ct);
        if (!categoryExists)
            throw new KeyNotFoundException($"Kategori bulunamadı: {command.CategoryId}");

        var item = new MenuItem
        {
            TenantId = tenantId,
            CategoryId = command.CategoryId,
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            ImageUrl = command.ImageUrl,
            IsAvailable = command.IsAvailable,
            PreparationTime = command.PreparationTime,
            Allergens = command.Allergens,
            SortOrder = command.SortOrder
        };

        db.MenuItems.Add(item);
        await db.SaveChangesAsync(ct);
        return item.Adapt<MenuItemDto>();
    }
}

public static class UpdateMenuItemHandler
{
    public static async Task<MenuItemDto> Handle(
        UpdateMenuItemCommand command,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var item = await db.MenuItems.FirstOrDefaultAsync(m => m.Id == command.Id, ct)
            ?? throw new KeyNotFoundException($"Ürün bulunamadı: {command.Id}");

        item.Name = command.Name;
        item.Description = command.Description;
        item.Price = command.Price;
        item.ImageUrl = command.ImageUrl;
        item.IsAvailable = command.IsAvailable;
        item.PreparationTime = command.PreparationTime;
        item.Allergens = command.Allergens;
        item.SortOrder = command.SortOrder;
        item.IsActive = command.IsActive;
        item.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return item.Adapt<MenuItemDto>();
    }
}
