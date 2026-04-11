using Microsoft.AspNetCore.Authorization;
using QRPayCheck.Application.Categories;
using QRPayCheck.Application.Categories.Commands;
using QRPayCheck.Application.Categories.Queries;
using Wolverine;
using Wolverine.Http;

namespace QRPayCheck.API.Endpoints;

public static class CategoryEndpoints
{
    [WolverineGet("/api/menus/{menuId:guid}/categories")]
    [Authorize]
    public static Task<IReadOnlyList<CategoryDto>> ListCategories(Guid menuId, IMessageBus bus)
        => bus.InvokeAsync<IReadOnlyList<CategoryDto>>(new ListCategoriesQuery(menuId));

    [WolverinePost("/api/categories")]
    [Authorize]
    public static async Task<IResult> CreateCategory(CreateCategoryCommand command, IMessageBus bus)
    {
        var result = await bus.InvokeAsync<CategoryDto>(command);
        return Results.Created($"/api/categories/{result.Id}", result);
    }

    [WolverinePut("/api/categories/{id:guid}")]
    [Authorize]
    public static async Task<IResult> UpdateCategory(Guid id, UpdateCategoryCommand command, IMessageBus bus)
    {
        var cmd = command with { Id = id };
        var result = await bus.InvokeAsync<CategoryDto>(cmd);
        return Results.Ok(result);
    }
}
