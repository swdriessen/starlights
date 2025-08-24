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
        Delete("/{id:guid}");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        using var activity = CharactersInstrumentation.StartActivity(nameof(DeleteCharacterEndpoint));

        var id = Route<Guid>("id");
        var characterId = new CharacterId(id);

        var characters = _persistence.GetRepository<ICharactersRepository>();

        var existingCharacter = await characters.GetCharacterAsync(characterId);
        if (existingCharacter is null)
        {
            await Send.NotFoundAsync(cancellation: c);
            return;
        }

        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        var appearances = _persistence.GetRepository<IAppearanceRepository>();

        await characters.DeleteCharacterAsync(characterId);

        await registrations.DeleteRegistrationsAsync(characterId);

        await appearances.DeleteAppearanceAsync(characterId);


        var rows = await _persistence.SaveChangesAsync();

        activity?.AddTag("db.rows_affected", rows);

        await Send.NoContentAsync(cancellation: c);
    }
}
