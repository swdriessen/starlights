using AwesomeAssertions;
using Starlights.Integration.Drivers.CharacterCreation.Endpoints;
using Starlights.Modules.Characters.Endpoints.Characters.SavingThrows;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class SavingThrowDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly SavingThrowEndpointDriver _api;

    public SavingThrowDriver(IIntegrationHost integration, SavingThrowEndpointDriver api)
    {
        _integration = integration;
        _api = api;
    }

    public async Task<List<SavingThrowDataModel>> GetSavingThrows()
    {
        var response = await _api.GetSavingThrows();
        response.SavingThrows.Should().NotBeNull();
        return response.SavingThrows;
    }
}