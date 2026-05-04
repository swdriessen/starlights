using AwesomeAssertions;
using Starlights.Integration.Drivers.Characters.Endpoints;
using Starlights.Integration.Eventing;
using Starlights.Integration.Extensions;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters.Eventing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Modules.Characters.Domain.Skills.Eventing;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;

namespace Starlights.Integration.Drivers.Characters.Manage;

public sealed class CharacterManagementDriver : IDriver
{
    private readonly IIntegrationHost _host;
    private readonly CharactersEndpointDriver _api;
    private readonly CharactersDriverContext _context;
    private readonly EventObserverCollection _events;

    public CharacterManagementDriver(IIntegrationHost host, CharactersEndpointDriver api, CharactersDriverContext context, EventObserverCollection events)
    {
        _host = host;
        _api = api;
        _context = context;
        _events = events;
    }

    public async Task<List<CharacterCreationOption>> GetCharacterCreationOptionsAsync()
    {
        var response = await _api.GetCharacterCreationOptionsAsync();
        response.Options.Should().NotBeEmpty("There should be at least one character creation option available.");
        return response.Options;
    }

    public async Task<CharacterCreationOption> GetDefaultCharacterCreationOptionAsync()
    {
        var options = await GetCharacterCreationOptionsAsync();
        var defaultOption = options.SingleOrDefault(o => o.Name == "Default Character");
        defaultOption.Should().NotBeNull("Default Character option should be present in the options.");
        return defaultOption;
    }










    public async Task<Guid> CreateCharacterAsync(string name = "Integration Character")
    {
        // at some point we will have a specific event to indicate the character is fully set up
        // until then, we wait for at least one ability, saving throw, and skill to be created
        List<Task> observations = [
            _events.EnsureObservation<CharacterCreatedEvent>(),
            _events.EnsureObservation<AbilityScoreCreatedEvent>(),
            _events.EnsureObservation<SavingThrowCreatedEvent>(),
            _events.EnsureObservation<SkillCreatedEvent>(),
            _events.EnsureObservation<RegistrationSelectionRuleCreatedEvent>(e => e.ElementType == "Class")
        ];

        // default character creation option
        var option = await GetDefaultCharacterCreationOptionAsync();

        // create character
        var id = await _api.CreateCharacterAsync(option.Id, name, "/portrait.image");

        id.Should().NotBe(Guid.Empty, "Character creation should return a valid Id.");

        _host.SetCharacterIdentifier(id);
        _context.WithCharacter(id, name);

        await Task.WhenAll(observations);

        return id;
    }

    public async Task<List<CharacterDetailsDataModel>> GetCharacters()
    {
        var response = await _api.GetCharactersAsync();
        response.Characters.Should().NotBeNull();
        return response.Characters;
    }

    public async Task<CharacterDetailsDataModel> GetCharacter(Guid characterId)
    {
        var response = await _api.GetCharacterAsync(characterId);
        response.Character.Should().NotBeNull();
        return response.Character;
    }

    public async Task DeleteCharacter(Guid characterId)
    {
        await _api.DeleteCharacterAsync(characterId);
    }









}
