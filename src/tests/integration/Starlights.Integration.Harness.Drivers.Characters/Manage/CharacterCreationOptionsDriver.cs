using AwesomeAssertions;
using Starlights.Integration.Drivers.Characters.Endpoints;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;

namespace Starlights.Integration.Drivers.Characters.Manage;

public sealed class CharacterCreationOptionsDriver : IDriver
{
    private readonly CharactersEndpointDriver _api;

    public CharacterCreationOptionsDriver(CharactersEndpointDriver api)
    {
        _api = api;
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
}
