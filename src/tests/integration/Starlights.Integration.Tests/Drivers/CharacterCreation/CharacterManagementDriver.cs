using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Eventing;
using Starlights.Integration.Core.Extensions;
using Starlights.Modules.Characters.Domain.Progression.Eventing;
using Starlights.Modules.Characters.Endpoints.Characters.GetCharacterClasses;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetRegistrations;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class CharacterManagementDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly CharacterManagementEndpointDriver _api;
    private readonly EventObserverCollection _events;

    public CharacterManagementDriver(IIntegrationHost integration,
        CharacterManagementEndpointDriver api,
        EventObserverCollection events)
    {
        _integration = integration;
        _api = api;
        _events = events;
    }

    public async Task<List<CharacterClassDataModel>> GetCharacterClassesAsync()
    {
        var characterId = _integration.GetCharacterIdentifier();
        var response = await _api.GetCharacterClasses(characterId);
        return response.Classes;
    }

    public async Task<CharacterClassDataModel> GetPrimaryClassAsync()
    {
        var characterId = _integration.GetCharacterIdentifier();

        var response = await _api.GetCharacterClasses(characterId);
        response.Classes.Should().NotBeEmpty("Expected the character to have at least one class.");

        var primaryClass = response.Classes.SingleOrDefault(c => c.IsPrimary);
        primaryClass.Should().NotBeNull("Expected exactly one primary class to be set.");

        return primaryClass;
    }

    public async Task<CharacterClassDataModel> GetClassByName(string className)
    {
        var classes = await GetCharacterClassesAsync();
        var characterClass = classes.SingleOrDefault(c => c.Name == className);
        characterClass.Should().NotBeNull($"Expected to find a character class with name '{className}'.");
        return characterClass;
    }

    public async Task LevelUp(Guid classId, int newLevel)
    {
        var characterId = _integration.GetCharacterIdentifier();
        await _api.UpdateClassLevel(characterId, classId, newLevel);

        // wait for the level change event of the character
        await _events.EnsureObservation<CharacterLevelChangedEvent>(e => e.NewLevel >= newLevel);
    }



    public async Task<List<RegistrationDataModel>> GetFeatures()
    {
        var characterId = _integration.GetCharacterIdentifier();

        var response = await _api.GetFeatures(characterId);
        response.Features.Should().NotBeEmpty();

        return response.Features;
    }



}