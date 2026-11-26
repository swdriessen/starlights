using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.DependencyInjection;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.Characters.Endpoints.Building;

internal static class CharacterBuildEndpoints
{
    public static void MapBuild(WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            if (context.Request.Path.StartsWithSegments("/api/characters/notifications")
                && context.RequestServices.GetRequiredService<ICurrentUser>().UserId is { } owner)
                context.Items[CharacterNotificationsHub.OwnerKey] = owner;
            await next(context);
        });
        app.MapHub<CharacterNotificationsHub>("/api/characters/notifications",
            options => options.CloseOnAuthenticationExpiration = true).RequireAuthorization();
        var group = app.MapGroup("/api/characters/{characterId:guid}").RequireAuthorization().WithTags("Character build");
        group.MapGet("/build", async Task<Results<Ok<CharacterBuildResponse>, NotFound>> (
            Guid characterId, ICharacterBuildWorkflows workflows, CancellationToken cancellationToken) =>
            await workflows.GetAsync(characterId, cancellationToken) is { } snapshot ? TypedResults.Ok(snapshot) : TypedResults.NotFound())
            .WithName("GetCharacterBuild").WithSummary("Read the owned Character's authoritative revision and build status.")
            .Produces(401).ProducesProblem(400);
        group.MapPost("/choices", (Guid characterId, RecordCharacterChoiceRequest request, ICharacterBuildWorkflows workflows, CancellationToken cancellationToken)
            => Edit(characterId, request, workflows, cancellationToken))
            .WithName("RecordCharacterChoice").WithSummary("Record a revision-checked Selection answer.")
            .ProducesProblem(400).ProducesProblem(409).Produces(401);
        group.MapPost("/ability-scores", (Guid characterId, AssignCharacterAbilitiesRequest request, ICharacterBuildWorkflows workflows, CancellationToken cancellationToken)
            => Edit(characterId, request, workflows, cancellationToken))
            .WithName("AssignCharacterAbilities").WithSummary("Record valid partial or complete standard-array inputs.")
            .ProducesProblem(400).ProducesProblem(409).Produces(401);
        group.MapPost("/equipment-claims", (Guid characterId, ClaimCharacterEquipmentRequest request, ICharacterBuildWorkflows workflows, CancellationToken cancellationToken)
            => Edit(characterId, request, workflows, cancellationToken))
            .WithName("ClaimCharacterEquipment").WithSummary("Acquire a selected starting package once, independently of the Choice.")
            .ProducesProblem(400).ProducesProblem(409).Produces(401);
        group.MapPost("/equipment", (Guid characterId, EquipCharacterRequest request, ICharacterBuildWorkflows workflows, CancellationToken cancellationToken)
            => Edit(characterId, request, workflows, cancellationToken))
            .WithName("EquipCharacter").WithSummary("Allocate owned armor and a weapon.")
            .ProducesProblem(400).ProducesProblem(409).Produces(401);
        group.MapPost("/name", (Guid characterId, NameCharacterRequest request, ICharacterBuildWorkflows workflows, CancellationToken cancellationToken)
            => Edit(characterId, request, workflows, cancellationToken))
            .WithName("NameCharacter").WithSummary("Rename the owned Character using a revision-checked operation.")
            .ProducesProblem(400).ProducesProblem(409).Produces(401);
    }

    private static async Task<Results<Ok<CharacterBuildResponse>, NotFound>> Edit(Guid characterId,
        ICharacterEditRequest request, ICharacterBuildWorkflows workflows, CancellationToken cancellationToken)
        => await workflows.EditAsync(characterId, request, cancellationToken) is { } snapshot ? TypedResults.Ok(snapshot) : TypedResults.NotFound();
}
