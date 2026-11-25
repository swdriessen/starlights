using Microsoft.AspNetCore.Http;
using Starlights.Modules.Characters.Data;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.Characters.Endpoints;

internal sealed class CharacterOwnershipFilter(ICharacterOwnership ownership, ICurrentUser currentUser, ICharacterBuildStore builds) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (context.HttpContext.Request.RouteValues.TryGetValue("characterId", out var value)
            && Guid.TryParse(value?.ToString(), out var characterId))
        {
            var userId = currentUser.UserId
                ?? throw new InvalidOperationException("Character access requires an authenticated owner.");
            if (!await ownership.IsOwnerAsync(characterId, userId, context.HttpContext.RequestAborted))
            {
                return Results.NotFound();
            }
            if (await builds.IsInitializedAsync(characterId, context.HttpContext.RequestAborted))
                return Results.Problem(statusCode: 409, title: "This operation is not supported for a Ruleset-based build.");
        }
        return await next(context);
    }
}
