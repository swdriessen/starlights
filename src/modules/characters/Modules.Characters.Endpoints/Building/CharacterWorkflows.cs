using System.Text;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Building;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Appearances;
using Starlights.Modules.Characters.Domain.Building;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Classes;
using Starlights.Modules.Characters.Domain.Elements;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Domain.Services;
using Starlights.Modules.Characters.Endpoints.Characters.CreateCharacter;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterDetails;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;
using Starlights.Platform.Hosting;

namespace Starlights.Modules.Characters.Endpoints.Building;

internal sealed class CharacterWorkflows(
    ICurrentUser currentUser, CharacterRulesets rulesets, CharacterContentCatalog catalog,
    ICharacterBuildStore builds, ICharacterOwnership ownership, IPersistence persistence,
    ICharacterCreationService legacyCreation, IElementsModuleQueries elements,
    ILogger<CharacterWorkflows> logger, CharacterMechanics mechanics) : ICharacterWorkflows
{
    private Guid OwnerId => currentUser.UserId
        ?? throw new InvalidOperationException("Character access requires an authenticated owner.");

    public IReadOnlyList<RulesetOptionResponse> Options => catalog.Options.Select(option => new RulesetOptionResponse(
        option.RulesetId, option.Name, option.PackName, Content(option.Content))).ToArray();

    public async Task<Guid> CreateAsync(CreateCharacterRequest request, CancellationToken cancellationToken)
    {
        using var activity = CharactersInstrumentation.StartActivity("Character.Create");
        if (request.Name?.Length > 100)
            throw new CharacterRequestException("Character name cannot exceed 100 characters.");
        var legacy = request.CharacterCreationOptionId.HasValue;
        if (legacy)
        {
            if (request.RulesetId is not null || request.PackId is not null
                || request.PackVersion is not null || request.CharacterElementId is not null)
                throw new CharacterRequestException("Legacy creation cannot be combined with a Ruleset selection.");
            return await CreateLegacyAsync(request);
        }
        if (string.IsNullOrWhiteSpace(request.RulesetId) || !rulesets.Contains(request.RulesetId))
            throw new CharacterRequestException("Select a supported Ruleset.");
        if (request.PortraitUrl is not null)
            throw new CharacterRequestException("Portrait assignment is not supported for this build.");
        var ruleset = rulesets.GetRequired(request.RulesetId);
        var options = catalog.Options.Where(option => option.RulesetId == ruleset.Id).ToArray();
        if (options.Length == 0)
            throw new InvalidOperationException("The selected Ruleset has no bundled Character content.");
        var explicitContent = request.PackId is not null || request.PackVersion is not null || request.CharacterElementId is not null;
        var option = explicitContent
            ? options.SingleOrDefault(candidate => candidate.Content.PackId == request.PackId
                && candidate.Content.PackVersion == request.PackVersion && candidate.Content.CharacterElementId == request.CharacterElementId)
            : options.Length == 1 ? options[0] : null;
        if (option is null)
            throw new CharacterRequestException("Select an available exact Character content version.");
        logger.LogInformation("Initializing Character with Ruleset {RulesetId}", ruleset.Id);
        var composition = ruleset.Initialize(option.Content);
        var character = Character.Create(OwnerId, request.Name, new(ruleset, option.Content, composition));
        await builds.CreateAsync(character, cancellationToken);
        logger.LogInformation("Created Character {CharacterId} with Ruleset {RulesetId}", character.Id, ruleset.Id);
        return character.Id;
    }

    public async Task<GetCharactersResponse> ListAsync(int offset, CancellationToken cancellationToken)
    {
        var page = await builds.ListAsync(OwnerId, offset, cancellationToken);
        var visible = page.Take(50).ToArray();
        // Legacy summaries retain their old shape without loading D&D state for new builds.
        var legacy = visible.Any(header => header.RulesetId is null)
            ? (await persistence.GetRepository<ICharactersRepository>().GetCharactersAsync()).ToDictionary(character => character.Id.Value)
            : [];
        return new GetCharactersResponse
        {
            Characters = visible.Select(header => header.RulesetId is null
                ? LegacySummary(legacy[header.CharacterId])
                : new CharacterDetailsDataModel
                {
                    CharacterId = header.CharacterId, Name = header.Name, RulesetId = header.RulesetId, Status = "incomplete",
                    BuildSupported = header.Content is not null && mechanics.Supports(header.Content)
                }).ToList(),
            NextOffset = page.Count > 50 ? offset + 50 : null
        };
    }

    public async Task<GetCharacterDetailsResponse?> GetAsync(Guid characterId, CancellationToken cancellationToken)
    {
        using var activity = CharactersInstrumentation.StartActivity("Character.Reopen");
        if (!await ownership.IsOwnerAsync(characterId, OwnerId, cancellationToken))
            return null;
        if (!await builds.IsInitializedAsync(characterId, cancellationToken))
        {
            var legacy = await persistence.GetRepository<ICharactersRepository>().GetCharacterAsync(characterId)
                ?? throw new InvalidOperationException("The authorized legacy Character is unavailable.");
            return new() { Character = LegacySummary(legacy) };
        }
        var character = await builds.GetAsync(characterId, OwnerId, cancellationToken)
            ?? throw new InvalidOperationException("The authorized Character is unavailable.");
        var build = character.Build ?? throw new InvalidOperationException("Saved Character initialization is missing.");
        _ = catalog.GetRequired(build.RulesetId, build.Content);
        return new() { Character = new()
        {
            CharacterId = character.Id, Name = character.Name, RulesetId = build.RulesetId,
            Status = "incomplete", Content = Content(build.Content), BuildSupported = mechanics.Supports(build.Content)
        } };
    }

    private async Task<Guid> CreateLegacyAsync(CreateCharacterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.PortraitUrl?.Length > 2048)
            throw new CharacterRequestException("Legacy creation requires a name and a valid portrait URL length.");
        var root = await elements.GetCharacterCreationElement(request.CharacterCreationOptionId!.Value)
            ?? throw new CharacterRequestException("The legacy Character creation option does not exist.");
        var character = legacyCreation.Create(request.Name);
        if (request.PortraitUrl is not null)
            character.UpdateComponent<AppearanceComponent>((appearance, _) => appearance.PortraitUrl = request.PortraitUrl);
        persistence.GetRepository<ICharactersRepository>().Add(character);
        persistence.GetRepository<IRegistrationRepository>().Add(
            Registration.Create(character.Id, new ElementId(root.ElementId), root.Name, root.Type));
        if (await persistence.SaveChangesAsync() == 0)
            throw new InvalidOperationException("Legacy Character creation did not commit any state.");
        return character.Id;
    }

    private static CharacterContentResponse Content(CharacterContentBinding binding)
        => new(binding.PackId, binding.PackVersion, binding.CharacterElementId);

    private static CharacterDetailsDataModel LegacySummary(Character character)
    {
        var classes = character.GetRequiredComponent<ClassComponent>();
        var build = new StringBuilder();
        foreach (var item in classes.Classes)
        {
            build.Append(item.Name);
            if (classes.IsMulticlass) build.Append($" ({item.Level}) /");
        }
        return new()
        {
            CharacterId = character.Id, Name = character.Name,
            PortraitUrl = character.GetRequiredComponent<AppearanceComponent>().PortraitUrl,
            Level = character.GetRequiredComponent<ProgressionComponent>().CharacterLevel,
            Build = build.ToString().TrimEnd('/').Trim()
        };
    }
}
