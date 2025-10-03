using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Modules.Characters.Endpoints.Generation.PortraitOptions;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class CharacterPortraitDriver : IDriver
{
    private readonly CharacterCreationEndpointDriver _api;

    public CharacterPortraitDriver(CharacterCreationEndpointDriver api)
    {
        _api = api;
    }

    public async Task<List<CharacterPortraitOption>> GetCharacterPortraitOptions()
    {
        var response = await _api.GetCharacterPortraitOptionsAsync();
        response.Portraits.Should().NotBeEmpty("There should be at least one character portrait option available.");

        return response.Portraits;
    }
}