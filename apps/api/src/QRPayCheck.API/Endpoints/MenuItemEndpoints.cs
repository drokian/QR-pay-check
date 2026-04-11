using Microsoft.AspNetCore.Authorization;
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
}
