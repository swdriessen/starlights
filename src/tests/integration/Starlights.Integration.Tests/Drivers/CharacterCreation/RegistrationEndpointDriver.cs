using System.Net.Http.Json;
using FluentAssertions;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Extensions;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetRegistrations;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRuleOptions;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetSelectionRules;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.RegisterSelectionRule;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.UnregisterSelectionRule;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class RegistrationEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public RegistrationEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    public async Task<GetRegistrationsResponse> GetRegistrationsAsync(Guid characterId)
    {
        var api = _integration.CreateClient();

        var url = $"/api/characters/{characterId}/registrations";

        var response = await api.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetRegistrationsResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }

    public async Task<GetSelectionRulesResponse> GetSelectionRulesAsync(string[] types)
    {
        var api = _integration.CreateClient();

        var query = types is { Length: > 0 }
            ? string.Concat(types.Select(t => $"&type={Uri.EscapeDataString(t)}")).TrimStart('&')
            : string.Empty;

        var characterId = _integration.GetCharacterIdentifier();
        var url = $"/api/characters/{characterId}/builder/selection-rules?{query}";

        var response = await api.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetSelectionRulesResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }

    public async Task<GetSelectionRuleOptionsResponse> GetSelectionRuleOptionsAsync(Guid selectionRuleId)
    {
        var api = _integration.CreateClient();

        var characterId = _integration.GetCharacterIdentifier();
        var url = $"/api/characters/{characterId}/builder/selection-rules/{selectionRuleId}/options";

        var response = await api.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<GetSelectionRuleOptionsResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        return data;
    }

    public async Task<RegisterSelectionRuleResponse> RegisterSelectionRuleAsync(Guid parentRegistrationId, Guid selectionRuleId, Guid elementId)
    {
        var api = _integration.CreateClient();

        var characterId = _integration.GetCharacterIdentifier();
        var url = $"/api/characters/{characterId}/builder/selection-rules/{selectionRuleId}/register";
        var payload = new RegisterSelectionRuleRequest() { ElementId = elementId, ParentRegistration = parentRegistrationId };

        var response = await api.PostAsJsonAsync(url, payload);
        response.EnsureSuccessStatusCode();

        var data = await response.Content.ReadFromJsonAsync<RegisterSelectionRuleResponse>();
        data.Should().NotBeNull("Expected response content to be deserializable.");

        data.RegistrationId.Should().NotBe(Guid.Empty, "Expected a valid registration ID to be returned.");

        return data;
    }

    public async Task UnregisterSelectionRuleAsync(Guid parentRegistrationId, Guid selectionRuleId, Guid elementId)
    {
        var api = _integration.CreateClient();

        var characterId = _integration.GetCharacterIdentifier();
        var url = $"/api/characters/{characterId}/builder/selection-rules/{selectionRuleId}/unregister";
        var payload = new UnregisterSelectionRuleRequest() { ElementId = elementId, ParentRegistration = parentRegistrationId };

        var response = await api.PostAsJsonAsync(url, payload);
        response.EnsureSuccessStatusCode();
    }
}
