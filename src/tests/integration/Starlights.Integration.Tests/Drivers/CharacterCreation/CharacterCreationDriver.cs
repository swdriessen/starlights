using FluentAssertions;
using Starlights.Integration.Constants;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Eventing;
using Starlights.Integration.Core.Extensions;
using Starlights.Modules.Characters.Domain.Abilities.Eventing;
using Starlights.Modules.Characters.Domain.Characters.Eventing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Characters.Domain.SavingThrows.Eventing;
using Starlights.Modules.Characters.Domain.Skills.Eventing;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacters;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class CharacterCreationDriver
{
    private readonly IIntegrationHost _integration;
    private readonly CharacterCreationEndpointDriver _api;
    private readonly CharacterCreationOptionsDriver _creationOptionsDriver;
    private readonly EventObserverCollection _events;

    public CharacterCreationDriver(
        IIntegrationHost integration,
        CharacterCreationEndpointDriver api,
        CharacterCreationOptionsDriver creationOptionsDriver,
        EventObserverCollection events)
    {
        _integration = integration;
        _api = api;
        _creationOptionsDriver = creationOptionsDriver;
        _events = events;
    }

    public async Task<Guid> CreateCharacterAsync()
    {
        // at some point we will have a specific event to indicate the character is fully set up
        // until then, we wait for at least one ability, saving throw, and skill to be created
        List<Task> observations = [
            _events.EnsureObservation<CharacterCreatedEvent>(),
            _events.EnsureObservation<AbilityScoreCreatedEvent>(),
            _events.EnsureObservation<SavingThrowCreatedEvent>(),
            _events.EnsureObservation<SkillCreatedEvent>(),
            _events.EnsureObservation<RegistrationSelectionRuleCreatedEvent>(e => e.ElementType == SelectionRuleTypes.Class)
        ];

        // default character creation option
        var option = await _creationOptionsDriver.GetDefaultCharacterCreationOptionAsync();

        // create character
        using var api = _integration.CreateClient();

        var id = await _api.CreateCharacterAsync(option.Id, "Integration Character", "/portrait.image");

        id.Should().NotBe(Guid.Empty, "Character creation should return a valid Id.");

        _integration.SetCharacterIdentifier(id);

        await Task.WhenAll(observations);

        return id;
    }

    public async Task<List<CharacterDetailsDataModel>> GetCharacters()
    {
        using var api = _integration.CreateClient();
        var response = await _api.GetCharactersAsync();
        response.Characters.Should().NotBeNull();
        return response.Characters;
    }

    public async Task<CharacterDetailsDataModel> GetCharacter(Guid characterId)
    {
        using var api = _integration.CreateClient();
        var response = await _api.GetCharacterAsync(characterId);
        response.Character.Should().NotBeNull();
        return response.Character;
    }

    public async Task DeleteCharacter(Guid characterId)
    {
        using var api = _integration.CreateClient();

        await _api.DeleteCharacterAsync(characterId);
    }
}
