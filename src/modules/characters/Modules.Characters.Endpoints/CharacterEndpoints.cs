using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Starlights.Modules.Characters.Endpoints.Building;
using Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterDetails;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;
using Starlights.Modules.Characters.Endpoints.Middleware;

namespace Starlights.Modules.Characters.Endpoints;

/// <summary>Registers Character commands and queries without duplicate legacy routes.</summary>
public static class CharacterEndpoints
{
    /// <summary>Registers feature handlers and explicit invalid-request error handling.</summary>
    public static void AddCharacterWorkflows(this IServiceCollection services)
    {
        services.AddScoped<ICharacterWorkflows, CharacterWorkflows>();
        services.AddSingleton<CharacterMechanics>();
        services.AddScoped<ICharacterBuildWorkflows, CharacterBuildWorkflows>();
        services.AddSingleton<ICharacterSheetCalculation, CharacterSheetCalculation>();
        services.AddSingleton<ICharacterBuildNotifications, CharacterBuildNotifications>();
        services.AddHostedService<CharacterCalculationRecovery>();
        services.AddSignalR();
        services.AddExceptionHandler<CharacterRequestExceptionHandler>();
    }

    /// <summary>Maps authenticated create, collection, reopen and supported-option APIs.</summary>
    public static void MapCharacters(this WebApplication app)
    {
        CharacterBuildEndpoints.MapBuild(app);
        var group = app.MapGroup("/api/characters").WithTags("Characters").RequireAuthorization();
        group.MapPost("", async (CreateCharacterRequest request, ICharacterWorkflows workflows, CancellationToken cancellationToken) =>
        {
            var id = await workflows.CreateAsync(request, cancellationToken);
            return TypedResults.Created($"/api/characters/{id}", new CreateCharacterResponse(id));
        }).WithName("CreateCharacter").WithSummary("Create and durably initialize an owned Character.")
            .WithDescription("Commits the selected Ruleset's initial composition and exact content binding atomically. No sheet is calculated.")
            .ProducesProblem(400).Produces(401);

        group.MapGet("", async Task<Results<Ok<GetCharactersResponse>, ProblemHttpResult>> (
            ICharacterWorkflows workflows, CancellationToken cancellationToken, int offset = 0) =>
        {
            if (offset < 0 || offset > int.MaxValue - 50)
                return TypedResults.Problem(statusCode: 400, title: "The Character offset is outside the supported range.");
            return TypedResults.Ok(await workflows.ListAsync(offset, cancellationToken));
        }).WithName("ListCharacters").WithSummary("List a page of the authenticated owner's Characters.").Produces(401);

        group.MapGet("/{characterId:guid}", async Task<Results<Ok<GetCharacterDetailsResponse>, NotFound>> (
            Guid characterId, ICharacterWorkflows workflows, CancellationToken cancellationToken) =>
            await workflows.GetAsync(characterId, cancellationToken) is { } character
                ? TypedResults.Ok(character) : TypedResults.NotFound())
            .WithName("GetCharacter").WithSummary("Reopen an owned Character without reinitializing its saved state.").Produces(401);

        group.MapGet("/rulesets", (ICharacterWorkflows workflows) => TypedResults.Ok(workflows.Options))
            .WithName("GetCharacterRulesets").WithSummary("List supported content-defined Character creation options.").Produces(401);
    }
}
