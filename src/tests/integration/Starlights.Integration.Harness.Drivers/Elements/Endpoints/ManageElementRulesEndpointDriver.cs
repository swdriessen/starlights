using System.Net;
using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Reorder;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Update;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Reorder;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Update;

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

    public async Task<(bool IsSuccessStatusCode, HttpStatusCode StatusCode)> ReorderRulesAsync(Guid elementId, List<Guid> orderedRuleIds)
    {
        using var client = _integration.CreateClient();

        var request = new ReorderElementRulesRequest { RuleIds = orderedRuleIds };

        var response = await client.PutAsJsonAsync($"/api/elements/{elementId}/rules/order", request, _integration.CancellationToken);
        return (response.IsSuccessStatusCode, response.StatusCode);
    }

    public async Task<(UpdateStatisticRuleResponse? Response, HttpStatusCode StatusCode)> UpdateStatisticRuleAsync(Guid elementId, Guid ruleId, UpdateStatisticRuleRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/elements/{elementId}/rules/statistics/{ruleId}", request, _integration.CancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return (null, response.StatusCode);
        }

        var responseContent = await response.Content.ReadFromJsonAsync<UpdateStatisticRuleResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return (responseContent!, response.StatusCode);
    }

    public async Task<GetIncludeRulesResponse?> GetIncludeRulesAsync(Guid elementId)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/{elementId}/rules/includes", CancellationToken.None);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GetIncludeRulesResponse>(CancellationToken.None);
    }

    public async Task<GetIncludeRuleResponse?> GetIncludeRuleByIdAsync(Guid elementId, Guid ruleId)
    {
        using var client = _integration.CreateClient();

        var response = await client.GetAsync($"/api/elements/{elementId}/rules/includes/{ruleId}", CancellationToken.None);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<GetIncludeRuleResponse>(CancellationToken.None);
    }

    /// <summary>
    /// Add an include rule to an element via the API <code>/api/elements/{elementId}/rules/includes</code>
    /// </summary>
    public async Task<CreateIncludeRuleResponse> CreateIncludeRuleAsync(Guid elementId, CreateIncludeRuleRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PostAsJsonAsync($"/api/elements/{elementId}/rules/includes/create", request, _integration.CancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadFromJsonAsync<CreateIncludeRuleResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return responseContent!;
    }

    public async Task<(UpdateIncludeRuleResponse? Response, HttpStatusCode StatusCode)> UpdateIncludeRuleAsync(Guid elementId, Guid ruleId, UpdateIncludeRuleRequest request)
    {
        using var client = _integration.CreateClient();

        var response = await client.PutAsJsonAsync($"/api/elements/{elementId}/rules/includes/{ruleId}", request, _integration.CancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return (null, response.StatusCode);
        }

        var responseContent = await response.Content.ReadFromJsonAsync<UpdateIncludeRuleResponse>(_integration.CancellationToken);
        responseContent.Should().NotBeNull();

        return (responseContent!, response.StatusCode);
    }

    public async Task<(bool IsSuccessStatusCode, HttpStatusCode StatusCode)> DeleteIncludeRuleAsync(Guid elementId, Guid ruleId)
    {
        using var client = _integration.CreateClient();

        var response = await client.DeleteAsync($"/api/elements/{elementId}/rules/includes/{ruleId}", _integration.CancellationToken);
        return (response.IsSuccessStatusCode, response.StatusCode);
    }

    public async Task<(bool IsSuccessStatusCode, HttpStatusCode StatusCode)> ReorderIncludeRulesAsync(Guid elementId, List<Guid> orderedRuleIds)
    {
        using var client = _integration.CreateClient();

        var request = new ReorderIncludeRulesRequest { RuleIds = orderedRuleIds };

        var response = await client.PutAsJsonAsync($"/api/elements/{elementId}/rules/includes/reorder", request, _integration.CancellationToken);
        return (response.IsSuccessStatusCode, response.StatusCode);
    }
}
