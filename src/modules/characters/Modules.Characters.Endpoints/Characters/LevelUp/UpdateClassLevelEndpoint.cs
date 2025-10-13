using FastEndpoints;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Characters.LevelUp;

public sealed class UpdateClassLevelEndpoint : Endpoint<UpdateClassLevelRequest>
{
    private readonly ILogger<UpdateClassLevelEndpoint> _logger;
    private readonly IPersistence _persistence;

    public UpdateClassLevelEndpoint(ILogger<UpdateClassLevelEndpoint> logger, IPersistence persistence)
    {
        _logger = logger;
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/{characterId:guid}/classes/{classId:guid}/level");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpdateClassLevelRequest req, CancellationToken ct)
    {
        var characterId = new CharacterId(Route<Guid>("characterId"));
        var classId = new CharacterClassId(Route<Guid>("classId"));

        using var _ = CharactersInstrumentation.StartActivity($"{nameof(UpdateClassLevelEndpoint)} | {characterId.Value} | {classId.Value}");

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var character = await characters.GetCharacterAsync(characterId);
        if (character is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        if (req.NewLevel <= 0)
        {
            AddError("NewLevel must be a positive integer.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // update class level and progression in one atomic update across components

        _logger.LogInformation("Level up to level '{NewLevel}' (class '{ClassId}')", req.NewLevel, classId.Value);

        character.UpdateComponents<ClassComponent, ProgressionComponent>((classComponent, progressionComponent, _) =>
        {
            classComponent.SetClassLevel(classId, req.NewLevel);
            progressionComponent.SetCharacterLevel(classComponent.CalculateCharacterLevel()); // CharacterLevelChangedEvent
        });

        //if (updatedClass is null)
        //{
        //    AddError($"Class with ID {classId} not found for Character {characterId}.");
        //    await Send.ErrorsAsync(cancellation: ct);
        //    return;
        //}

        await _persistence.SaveChangesAsync();

        await Send.OkAsync(cancellation: ct);
    }

}

public sealed record UpdateClassLevelRequest
{
    public int NewLevel { get; set; }
}