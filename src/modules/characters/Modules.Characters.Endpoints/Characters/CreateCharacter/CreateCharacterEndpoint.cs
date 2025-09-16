using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Components;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Services;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;

public sealed class CreateCharacterEndpoint : Endpoint<CreateCharacterRequest, CreateCharacterResponse>
{
    private readonly IPersistence _persistence;
    private readonly ICharacterCreationService _characterCreationService;
    private readonly IElementsModuleQueries _queries;

    public CreateCharacterEndpoint(IPersistence persistence, ICharacterCreationService characterCreationService, IElementsModuleQueries queries)
    {
        _persistence = persistence;
        _characterCreationService = characterCreationService;
        _queries = queries;
    }

    public override void Configure()
    {
        Post("");
        Group<CharactersGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CreateCharacterRequest req, CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity(nameof(CreateCharacterEndpoint));

        // character entity
        var newCharacter = _characterCreationService.Create(req.Name);

        // update appearance component if portrait url is provided
        if (req.PortraitUrl is not null)
        {
            newCharacter.UpdateComponent<AppearanceComponent>((appearance, _) => appearance.PortraitUrl = req.PortraitUrl);
        }

        // get the root element which handles all rules for character creation
        var rootElement = await _queries.GetCharacterCreationElement(req.CharacterCreationOptionId);
        if (rootElement is null)
        {
            AddError($"The character creation option '{req.CharacterCreationOptionId}' does not exist.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // registration of the root element used in character creation, this will grant abilities, skills, etc.
        var newRegistration = Registration.Create(newCharacter.Id, new ElementId(rootElement.ElementId), rootElement.Name, rootElement.Type);

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var registrations = _persistence.GetRepository<IRegistrationRepository>();

        characters.Add(newCharacter);
        registrations.Add(newRegistration);

        var affectedRows = await _persistence.SaveChangesAsync();
        if (affectedRows == 0)
        {
            AddError("Failed to create the character due to an unknown error.");
            await Send.ErrorsAsync(statusCode: 500, cancellation: ct);
            return;
        }

        await Send.ResponseAsync(new CreateCharacterResponse(newCharacter.Id), statusCode: 201, cancellation: ct);
    }
}
