using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Entities.Characters.Create;

public sealed class CreateCharacterEndpoint : Endpoint<CreateCharacterRequest, CreateCharacterResponse>
{
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _queries;

    public CreateCharacterEndpoint(IPersistence persistence, IElementsModuleQueries queries)
    {
        _persistence = persistence;
        _queries = queries;
    }

    public override void Configure()
    {
        Post("/create");
        AllowAnonymous();
        Group<CharactersGroup>();
    }

    public override async Task HandleAsync(CreateCharacterRequest req, CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity(nameof(CreateCharacterEndpoint));

        // get the root element which handles all rules for character creation
        var rootElement = await _queries.GetCharacterCreationElement(req.CharacterCreationOptionId);
        if (rootElement is null)
        {
            AddError($"The character creation option '{req.CharacterCreationOptionId}' does not exist.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // character entity
        var newCharacter = Character.Create(req.Name);

        var characters = _persistence.GetRepository<ICharactersRepository>();
        characters.Add(newCharacter);

        // appearance entity
        var newAppearance = Appearance.Create(newCharacter.Id);

        var appearances = _persistence.GetRepository<IAppearanceRepository>();
        appearances.Add(newAppearance);

        // registration of the root element used in character creation, this will grant abilities, skills, etc.
        var newRegistration = Registration.Create(newCharacter.Id, new ElementId(rootElement.ElementId), rootElement.Name);

        var registrations = _persistence.GetRepository<IRegistrationRepository>();
        registrations.Add(newRegistration);

        await _persistence.SaveChangesAsync();

        var reponse = new CreateCharacterResponse(newCharacter.Id);

        await Send.CreatedAtAsync($"/api/characters/{newCharacter.Id}", reponse, cancellation: ct);
    }
}
