using FluentValidation;
using Mapster;
using Microsoft.EntityFrameworkCore;
using QRPayCheck.Application.Common.Interfaces;
using QRPayCheck.Domain.Entities;

namespace QRPayCheck.Application.Categories.Commands;

public sealed record CreateCategoryCommand(Guid MenuId, string Name, string? Description, string? ImageUrl, int SortOrder = 0);
public sealed record UpdateCategoryCommand(Guid Id, string Name, string? Description, string? ImageUrl, int SortOrder, bool IsActive);

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.MenuId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}

public static class CreateCategoryHandler
{
    public static async Task<CategoryDto> Handle(
        CreateCategoryCommand command,
        IApplicationDbContext db,
        ITenantContext tenantContext,
        CancellationToken ct)
    {
        var tenantId = tenantContext.TenantId
            ?? throw new UnauthorizedAccessException("Tenant bağlamı bulunamadı.");

        var menuExists = await db.Menus.AnyAsync(m => m.Id == command.MenuId, ct);
        if (!menuExists)
            throw new KeyNotFoundException($"Menü bulunamadı: {command.MenuId}");

        var category = new Category
        {
            TenantId = tenantId,
            MenuId = command.MenuId,
            Name = command.Name,
            Description = command.Description,
            ImageUrl = command.ImageUrl,
            SortOrder = command.SortOrder
        };

        db.Categories.Add(category);
        await db.SaveChangesAsync(ct);
        return category.Adapt<CategoryDto>();
    }
}

public static class UpdateCategoryHandler
{
    public static async Task<CategoryDto> Handle(
        UpdateCategoryCommand command,
        IApplicationDbContext db,
        CancellationToken ct)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == command.Id, ct)
            ?? throw new KeyNotFoundException($"Kategori bulunamadı: {command.Id}");

        category.Name = command.Name;
        category.Description = command.Description;
        category.ImageUrl = command.ImageUrl;
        category.SortOrder = command.SortOrder;
        category.IsActive = command.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return category.Adapt<CategoryDto>();
    }
}
