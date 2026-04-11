using Microsoft.AspNetCore.Authorization;
using QRPayCheck.Application.Branches;
using QRPayCheck.Application.Branches.Commands.CreateBranch;
using QRPayCheck.Application.Branches.Commands.UpdateBranch;
using QRPayCheck.Application.Branches.Queries;
using Wolverine;
using Wolverine.Http;

namespace QRPayCheck.API.Endpoints;

public static class BranchEndpoints
{
    [WolverineGet("/api/branches")]
    [Authorize]
    public static Task<IReadOnlyList<BranchDto>> ListBranches(IMessageBus bus)
        => bus.InvokeAsync<IReadOnlyList<BranchDto>>(new ListBranchesQuery());

    [WolverineGet("/api/branches/{id:guid}")]
    [Authorize]
    public static async Task<IResult> GetBranch(Guid id, IMessageBus bus)
    {
        var result = await bus.InvokeAsync<BranchDto?>(new GetBranchQuery(id));
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    [WolverinePost("/api/branches")]
    [Authorize]
    public static async Task<IResult> CreateBranch(CreateBranchCommand command, IMessageBus bus)
    {
        var result = await bus.InvokeAsync<BranchDto>(command);
        return Results.Created($"/api/branches/{result.Id}", result);
    }

    [WolverinePut("/api/branches/{id:guid}")]
    [Authorize]
    public static async Task<IResult> UpdateBranch(Guid id, UpdateBranchCommand command, IMessageBus bus)
    {
        var cmd = command with { Id = id };
        var result = await bus.InvokeAsync<BranchDto>(cmd);
        return Results.Ok(result);
    }
}
