using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Tests.Core;
using Starlights.Modules.Characters.Endpoints.Generation.CreationOptions;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class CharacterCreationOptionsDriver
{
    private readonly IIntegrationHost _integration;

    public CharacterCreationOptionsDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    public async Task<List<CharacterCreationOption>> GetCharacterCreationOptionsAsync()
    {
        using var api = _integration.CreateClient();
        var response = await api.GetCharacterCreationOptionsAsync();
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
