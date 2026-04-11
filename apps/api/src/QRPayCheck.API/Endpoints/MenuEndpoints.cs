using Microsoft.AspNetCore.Authorization;
using QRPayCheck.Application.Menus;
using QRPayCheck.Application.Menus.Commands;
using QRPayCheck.Application.Menus.Queries;
using Wolverine;
using Wolverine.Http;

namespace QRPayCheck.API.Endpoints;

public static class MenuEndpoints
{
    [WolverineGet("/api/branches/{branchId:guid}/menus")]
    [Authorize]
    public static Task<IReadOnlyList<MenuDto>> ListMenus(Guid branchId, IMessageBus bus)
        => bus.InvokeAsync<IReadOnlyList<MenuDto>>(new ListMenusQuery(branchId));

    [WolverineGet("/api/menus/{id:guid}")]
    [Authorize]
    public static async Task<IResult> GetMenu(Guid id, IMessageBus bus)
    {
        var result = await bus.InvokeAsync<MenuDto?>(new GetMenuQuery(id));
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    [WolverinePost("/api/menus")]
    [Authorize]
    public static async Task<IResult> CreateMenu(CreateMenuCommand command, IMessageBus bus)
    {
        var result = await bus.InvokeAsync<MenuDto>(command);
        return Results.Created($"/api/menus/{result.Id}", result);
    }

    [WolverinePut("/api/menus/{id:guid}")]
    [Authorize]
    public static async Task<IResult> UpdateMenu(Guid id, UpdateMenuCommand command, IMessageBus bus)
    {
        var cmd = command with { Id = id };
        var result = await bus.InvokeAsync<MenuDto>(cmd);
        return Results.Ok(result);
    }
}
