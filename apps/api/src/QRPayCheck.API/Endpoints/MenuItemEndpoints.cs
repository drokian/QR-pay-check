using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRPayCheck.Application.Common.Interfaces;
using QRPayCheck.Application.MenuItemOptions;
using QRPayCheck.Application.MenuItemOptions.Commands;
using QRPayCheck.Application.MenuItems;
using QRPayCheck.Application.MenuItems.Commands;
using QRPayCheck.Application.MenuItems.Queries;
using Wolverine;
using Wolverine.Http;

namespace QRPayCheck.API.Endpoints;

public static class MenuItemEndpoints
{
    [WolverineGet("/api/categories/{categoryId:guid}/items")]
    [Authorize]
    public static Task<IReadOnlyList<MenuItemDto>> ListItems(Guid categoryId, IMessageBus bus)
        => bus.InvokeAsync<IReadOnlyList<MenuItemDto>>(new ListMenuItemsQuery(categoryId));

    [WolverinePost("/api/menu-items")]
    [Authorize]
    public static async Task<IResult> CreateItem(CreateMenuItemCommand command, IMessageBus bus)
    {
        var result = await bus.InvokeAsync<MenuItemDto>(command);
        return Results.Created($"/api/menu-items/{result.Id}", result);
    }

    [WolverinePut("/api/menu-items/{id:guid}")]
    [Authorize]
    public static async Task<IResult> UpdateItem(Guid id, UpdateMenuItemCommand command, IMessageBus bus)
    {
        var cmd = command with { Id = id };
        var result = await bus.InvokeAsync<MenuItemDto>(cmd);
        return Results.Ok(result);
    }

    [WolverinePost("/api/menu-items/{menuItemId:guid}/options")]
    [Authorize]
    public static async Task<IResult> CreateOption(Guid menuItemId, CreateMenuItemOptionCommand command, IMessageBus bus)
    {
        var cmd = command with { MenuItemId = menuItemId };
        var result = await bus.InvokeAsync<MenuItemOptionDto>(cmd);
        return Results.Created($"/api/menu-item-options/{result.Id}", result);
    }

    [WolverinePut("/api/menu-item-options/{id:guid}")]
    [Authorize]
    public static async Task<IResult> UpdateOption(Guid id, UpdateMenuItemOptionCommand command, IMessageBus bus)
    {
        var cmd = command with { Id = id };
        var result = await bus.InvokeAsync<MenuItemOptionDto>(cmd);
        return Results.Ok(result);
    }

    [WolverineDelete("/api/menu-items/{id:guid}")]
    [Authorize]
    public static async Task<IResult> DeleteItem(Guid id, IMessageBus bus)
    {
        await bus.InvokeAsync(new DeleteMenuItemCommand(id));
        return Results.NoContent();
    }

    [WolverineDelete("/api/menu-item-options/{id:guid}")]
    [Authorize]
    public static async Task<IResult> DeleteOption(Guid id, IMessageBus bus)
    {
        await bus.InvokeAsync(new DeleteMenuItemOptionCommand(id));
        return Results.NoContent();
    }

    [WolverinePost("/api/menu-items/{id:guid}/image")]
    [Authorize]
    [RequestSizeLimit(5_242_880)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5_242_880)]
    public static async Task<IResult> UploadImage(
        Guid id,
        IFormFile file,
        IFileStorageService fileStorage,
        IMessageBus bus,
        CancellationToken ct)
    {
        if (file.Length == 0)
            return Results.BadRequest("Dosya boş olamaz.");

        if (file.Length > 5 * 1024 * 1024)
            return Results.BadRequest("Dosya boyutu 5 MB'ı geçemez.");

        var allowedTypes = new HashSet<string> { "image/jpeg", "image/png", "image/webp", "image/gif" };
        if (!allowedTypes.Contains(file.ContentType))
            return Results.BadRequest("Yalnızca JPEG, PNG, WebP ve GIF dosyaları kabul edilir.");

        await using var stream = file.OpenReadStream();
        var imageUrl = await fileStorage.SaveAsync(stream, file.FileName, file.ContentType, ct);

        var result = await bus.InvokeAsync<MenuItemDto>(new UploadMenuItemImageCommand(id, imageUrl));
        return Results.Ok(result);
    }
}
