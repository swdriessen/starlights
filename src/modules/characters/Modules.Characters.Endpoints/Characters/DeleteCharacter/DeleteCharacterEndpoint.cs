using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Characters.DeleteCharacter;

sealed class DeleteCharacterEndpoint : EndpointWithoutRequest
{
    private readonly IPersistence _persistence;

    public DeleteCharacterEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Delete("/{characterId:guid}");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        using var activity = CharactersInstrumentation.StartActivity(nameof(DeleteCharacterEndpoint));

        var characterId = new CharacterId(Route<Guid>("characterId"));

        var characters = _persistence.GetRepository<ICharactersRepository>();

        var deleted = await characters.DeleteCharacterAsync(characterId);
        if (!deleted)
        {
            await Send.NotFoundAsync(cancellation: c);
            return;
        }

        var appearances = _persistence.GetRepository<IAppearanceRepository>();
        await appearances.DeleteAppearanceAsync(characterId);

        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        await registrations.DeleteRegistrationsAsync(characterId);

        var rows = await _persistence.SaveChangesAsync();

        activity?.AddTag("db.rows_affected", rows);

        await Send.NoContentAsync(cancellation: c);
    }
}
