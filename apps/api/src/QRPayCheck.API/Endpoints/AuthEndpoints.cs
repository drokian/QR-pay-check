using Microsoft.AspNetCore.Authorization;
using QRPayCheck.Application.Auth;
using QRPayCheck.Application.Auth.Commands;
using Wolverine;
using Wolverine.Http;

namespace QRPayCheck.API.Endpoints;

public static class AuthEndpoints
{
    [WolverineGet("/api/auth/profile")]
    [Authorize]
    public static Task<UserProfileDto> GetProfile(IMessageBus bus)
        => bus.InvokeAsync<UserProfileDto>(new SyncUserProfileCommand());
}
