using FastEndpoints;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Entities.Characters.Create;

public sealed class CreateCharacterEndpoint : Endpoint<CreateCharacterRequest, CreateCharacterResponse>
{
    private readonly IPersistence _persistence;

    public CreateCharacterEndpoint(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public override void Configure()
    {
        Post("/create");
        AllowAnonymous();
        Group<CharactersGroup>();
    }
    public override async Task HandleAsync(CreateCharacterRequest req, CancellationToken ct)
    {
        using var _ = CharactersInstrumentation.StartActivity();

        // create a 'CharacterCreationService' in the application layer
        // e.g. when creating a character, it should ensure an appearance entity is also created 

        var character = Character.Create(req.Name);
        var appearance = Appearance.Create(character.Id);

        if (!string.IsNullOrWhiteSpace(req.PortraitUrl))
        {
            appearance.UpdatePortraitUrl(req.PortraitUrl);
        }

        var repository = _persistence.GetRepository<ICharactersRepository>();
        var appearanceRepository = _persistence.GetRepository<IAppearanceRepository>();

        repository.Add(character);
        appearanceRepository.Add(appearance);

        await _persistence.SaveChangesAsync();

        // start processing the character now that it is created with the character creation option
        // this element will grant the things like abilities, saving throws, skills, etc.

        var response = new CreateCharacterResponse(character.Id);

        await Send.CreatedAtAsync($"/api/characters/{character.Id}", response, cancellation: ct);
    }
}

public record CreateCharacterRequest
{
    public Guid CharacterCreationOptionId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? PortraitUrl { get; init; }
}

public record CreateCharacterResponse(Guid Id);
