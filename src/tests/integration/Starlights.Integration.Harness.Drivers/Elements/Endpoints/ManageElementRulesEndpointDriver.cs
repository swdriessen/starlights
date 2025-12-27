using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetList;

namespace Starlights.Integration.Drivers.Elements.Endpoints;

public sealed class ManageElementRulesEndpointDriver : IDriver
{
    private readonly IIntegrationHost _integration;

    public ManageElementRulesEndpointDriver(IIntegrationHost integration)
    {
        _integration = integration;
    }

    /// <summary>
    /// Add a statistic rule to an element via the API <code>/api/elements/{elementId}/rules/statistics</code>
    /// </summary>
    public async Task<CreateStatisticRuleResponse> CreateStatisticRuleAsync(Guid elementId, CreateStatisticRuleRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync($"/api/elements/{elementId}/rules/statistics/create", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<CreateStatisticRuleResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!;
    }

    public async Task<(bool IsSuccessStatusCode, HttpStatusCode StatusCode)> DeleteStatisticRuleAsync(Guid elementId, Guid ruleId)
    {
        using var client = _integration.CreateClient();

        var response = await client.DeleteAsync($"/api/elements/{elementId}/rules/statistics/{ruleId}", _integration.CancellationToken);
        return (response.IsSuccessStatusCode, response.StatusCode);
    }

    public async Task<GetStatisticRulesResponse?> GetStatisticRulesAsync(Guid elementId)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/{elementId}/rules/statistics", _integration.CancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GetStatisticRulesResponse>(_integration.CancellationToken);
    }

    public async Task<GetStatisticRuleResponse?> GetStatisticRuleByIdAsync(Guid elementId, Guid ruleId)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/{elementId}/rules/statistics/{ruleId}", _integration.CancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GetStatisticRuleResponse>(_integration.CancellationToken);
    }
}
