using Microsoft.AspNetCore.Authorization;
using QRPayCheck.Application.Tenants;
using QRPayCheck.Application.Tenants.Commands.CreateTenant;
using QRPayCheck.Application.Tenants.Commands.DeleteTenant;
using QRPayCheck.Application.Tenants.Commands.UpdateTenant;
using QRPayCheck.Application.Tenants.Queries;
using Wolverine;
using Wolverine.Http;

namespace QRPayCheck.API.Endpoints;

public static class TenantEndpoints
{
    [WolverineGet("/api/tenants/me")]
    [Authorize]
    public static async Task<IResult> GetMyTenant(IMessageBus bus)
    {
        var result = await bus.InvokeAsync<TenantDto?>(new GetMyTenantQuery());
        return result is null ? Results.NotFound() : Results.Ok(result);
    }

    [WolverinePost("/api/tenants")]
    [Authorize]
    public static async Task<IResult> CreateTenant(CreateTenantCommand command, IMessageBus bus)
    {
        var result = await bus.InvokeAsync<TenantDto>(command);
        return Results.Created($"/api/tenants/me", result);
    }

    [WolverinePut("/api/tenants/me")]
    [Authorize]
    public static async Task<IResult> UpdateTenant(UpdateTenantCommand command, IMessageBus bus)
    {
        var result = await bus.InvokeAsync<TenantDto>(command);
        return Results.Ok(result);
    }

    [WolverineDelete("/api/tenants/me")]
    [Authorize]
    public static async Task<IResult> DeleteTenant(IMessageBus bus)
    {
        await bus.InvokeAsync(new DeleteTenantCommand());
        return Results.NoContent();
    }
}
