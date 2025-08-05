using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Endpoints.Entities.Characters.Create;

public record CreateCharacterRequest(
    Guid CharacterCreationOptionId,
    string Name,
    string? PortraitUrl = null);

public record CreateCharacterResponse(Guid Id);

public sealed class CreateCharacterRequestValidator : Validator<CreateCharacterRequest>
{
    public CreateCharacterRequestValidator()
    {
        RuleFor(x => x.CharacterCreationOptionId)
            .NotEmpty().WithMessage("Character creation option is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Character name is required.")
            .MaximumLength(100).WithMessage("Character name cannot exceed 100 characters.");

        RuleFor(x => x.PortraitUrl)
            .MaximumLength(2048).WithMessage("Portrait URL cannot exceed 2048 characters.");
    }
}

public sealed class CreateCharacterEndpoint : Endpoint<CreateCharacterRequest, CreateCharacterResponse>
{
    private readonly ILogger<CreateCharacterEndpoint> _logger;
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _queries;

    public CreateCharacterEndpoint(ILogger<CreateCharacterEndpoint> logger, IPersistence persistence, IElementsModuleQueries queries)
    {
        _logger = logger;
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

        var rootElement = await _queries.GetCharacterCreationElement(req.CharacterCreationOptionId);
        if (rootElement is null)
        {
            AddError($"The character creation option '{req.CharacterCreationOptionId}' does not exist.");
            await Send.ErrorsAsync(cancellation: ct);
            return;
        }

        // create service that constructs a valid character in steps
        // e.g. create appearance, background, first registration

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

        // we got this far...
        // we have a character with a name, appearance, and registration of the root element used in character creation
        // saving this character should trigger processing of new element registrations asynchronously

        await _persistence.SaveChangesAsync();

        // we can return the character id to the user...

        // start processing the character now that it is created with the character creation option
        // this element will grant the things like abilities, saving throws, skills, etc.

        // create hosted service that will process the character creation



        var reponse = new CreateCharacterResponse(newCharacter.Id);

        await Send.CreatedAtAsync($"/api/characters/{newCharacter.Id}", reponse, cancellation: ct);
    }



    public Character Create(string name)
    {
        var character = Character.Create(name);
        var appearance = Appearance.Create(character.Id);

        var characterRepository = _persistence.GetRepository<ICharactersRepository>();
        var appearanceRepository = _persistence.GetRepository<IAppearanceRepository>();

        characterRepository.Add(character);
        appearanceRepository.Add(appearance);

        return character;
    }
}

public class CharacterCreationContext
{
    private readonly IPersistence _persistence;

    public CharacterCreationContext(IPersistence persistence)
    {
        _persistence = persistence;
    }

    public required Character Character { get; init; }

    public T GetRepository<T>() where T : IRepository
    {
        return _persistence.GetRepository<T>();
    }
}

public interface INewCharacterCreatedBehavior
{
    Task Handle(CharacterCreationContext context);
}

public class AppearanceBehavior : INewCharacterCreatedBehavior
{
    public Task Handle(CharacterCreationContext context)
    {
        var appearance = Appearance.Create(context.Character.Id);

        IAppearanceRepository appearances = context.GetRepository<IAppearanceRepository>();
        appearances.Add(appearance);

        return Task.CompletedTask;
    }
}