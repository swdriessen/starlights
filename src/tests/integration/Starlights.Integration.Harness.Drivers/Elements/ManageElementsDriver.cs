using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Elements;
using Starlights.Modules.Elements.Endpoints.Content.Elements.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Update;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageElementsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ManageElementsEndpointDriver _api;
    private readonly ManageElementRulesEndpointDriver _rulesApi;

    // potentially create a class to hold this data if it grows
    private readonly Dictionary<string, Guid> _createdElementsByName = [];

    public ManageElementsDriver(IIntegrationHost integration, ManageElementsEndpointDriver endpointDriver, ManageElementRulesEndpointDriver rulesEndpointDriver)
    {
        _integration = integration;
        _api = endpointDriver;
        _rulesApi = rulesEndpointDriver;
    }

    public async Task<Guid> CreateElement(CreateProperties properties, bool storeAsLastCreated = true)
    {
        var request = new CreateElementRequest
        {
            Name = properties.Name,
            Type = properties.Type,
            Description = properties.Description
        };

        var id = await _api.CreateAsync(request);
        id.Should().NotBeEmpty();

        _createdElementsByName.Add(properties.Name, id);

        if (storeAsLastCreated)
        {
            _integration.Set(id, "last-created-element-id");
            _integration.Set(properties, "last-created-element-properties");
        }

        return id;
    }

    public Task<ElementDataModel> GetElementById(Guid id)
    {
        return _api.GetByIdAsync(id);
    }

    public Task<ElementDataModel> GetElementByName(string name)
    {
        return !_createdElementsByName.TryGetValue(name, out var id)
            ? throw new KeyNotFoundException($"No element found with name '{name}'.")
            : GetElementById(id);
    }

    public Task<ElementDataModel> GetLastCreatedElement()
    {
        var id = _integration.Get<Guid>("last-created-element-id");
        return GetElementById(id);
    }

    public async Task<List<ElementDataModel>> GetElements(string? type = null)
    {
        var response = await _api.GetListAsync(type);
        return [.. response.Items];
    }

    public async Task<CreateStatisticRuleResponse> CreateStatisticRule(Guid elementId, CreateStatisticRuleProperties properties)
    {
        var request = new CreateStatisticRuleRequest
        {
            ElementId = elementId,
            Name = properties.Name,
            Value = properties.Value,
            StackingBonus = properties.StackingBonus,
            LevelRequirement = properties.LevelRequirement,
            DisplayName = properties.DisplayName,
            Minimum = properties.Minimum,
            Maximum = properties.Maximum,
            RequirementsExpression = properties.RequirementsExpression
        };

        var response = await _rulesApi.CreateStatisticRuleAsync(elementId, request);

        response.ElementId.Should().Be(elementId);
        response.RuleId.Should().NotBeEmpty();

        _integration.Set(response, "last-created-statistic-rule");
        _integration.Set(properties, "last-created-statistic-rule-properties");

        return response;
    }

    public async Task<IReadOnlyList<GetStatisticRulesResponse.StatisticRuleItem>> GetStatisticRules(Guid elementId)
    {
        var response = await _rulesApi.GetStatisticRulesAsync(elementId);
        response.Should().NotBeNull();
        response.ElementId.Should().Be(elementId);
        response.Rules.Should().NotBeNull();
        return response.Rules;
    }

    public async Task<GetStatisticRuleResponse> GetStatisticRuleById(Guid elementId, Guid ruleId)
    {
        var response = await _rulesApi.GetStatisticRuleByIdAsync(elementId, ruleId);
        response.Should().NotBeNull();
        response.ElementId.Should().Be(elementId);
        response.RuleId.Should().Be(ruleId);
        return response;
    }

    public async Task<bool> DeleteStatisticRule(Guid elementId, Guid ruleId)
    {
        var (success, _) = await _rulesApi.DeleteStatisticRuleAsync(elementId, ruleId);
        return success;
    }

    public async Task ReorderRules(Guid elementId, List<Guid> orderedRuleIds)
    {
        await _rulesApi.ReorderRulesAsync(elementId, orderedRuleIds);
    }

    public async Task<UpdateStatisticRuleResponse> UpdateStatisticRule(Guid elementId, Guid ruleId, UpdateStatisticRuleProperties properties)
    {
        var request = new UpdateStatisticRuleRequest
        {
            Name = properties.Name,
            Value = properties.Value,
            StackingBonus = properties.StackingBonus,
            LevelRequirement = properties.LevelRequirement,
            DisplayName = properties.DisplayName,
            Minimum = properties.Minimum,
            Maximum = properties.Maximum,
            RequirementsExpression = properties.RequirementsExpression
        };

        var (response, statusCode) = await _rulesApi.UpdateStatisticRuleAsync(elementId, ruleId, request);
        statusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Should().NotBeNull();
        response.ElementId.Should().Be(elementId);
        response.RuleId.Should().Be(ruleId);

        _integration.Set(response, "last-updated-statistic-rule");
        _integration.Set(properties, "last-updated-statistic-rule-properties");

        return response;
    }

    public async Task<CreateIncludeRuleResponse> CreateIncludeRule(Guid elementId, CreateIncludeRuleProperties properties)
    {
        var request = new CreateIncludeRuleRequest(
            ElementId: elementId,
            IncludedElementId: properties.IncludedElementId,
            LevelRequirement: properties.LevelRequirement,
            RequirementsExpression: properties.RequirementsExpression,
            DisplayName: properties.DisplayName);

        _integration.WriteLine($"Creating include rule for ElementId={elementId} with IncludedElementId={properties.IncludedElementId}");
        var response = await _rulesApi.CreateIncludeRuleAsync(elementId, request);

        response.ElementId.Should().Be(elementId);
        response.RuleId.Should().NotBeEmpty();

        _integration.Set(response, "last-created-include-rule");
        _integration.Set(properties, "last-created-include-rule-properties");

        return response;
    }

    public async Task<IReadOnlyList<GetIncludeRulesResponse.IncludeRuleItem>> GetIncludeRules(Guid elementId)
    {
        _integration.WriteLine($"Getting include rules for ElementId={elementId}");
        using var client = _integration.CreateClient();
        var response = await client.GetFromJsonAsync<GetIncludeRulesResponse>($"/api/elements/{elementId}/rules/includes");
        response.Should().NotBeNull();
        response.ElementId.Should().Be(elementId);
        return response.Rules;
    }

    public async Task<GetIncludeRuleResponse> GetIncludeRuleById(Guid elementId, Guid ruleId)
    {
        _integration.WriteLine($"Getting include rule by ID: ElementId={elementId}, RuleId={ruleId}");
        using var client = _integration.CreateClient();
        var response = await client.GetFromJsonAsync<GetIncludeRuleResponse>($"/api/elements/{elementId}/rules/includes/{ruleId}");
        response.Should().NotBeNull();
        response.ElementId.Should().Be(elementId);
        response.RuleId.Should().Be(ruleId);
        return response;
    }

    public sealed record CreateProperties
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public string? Description { get; set; }
    }

    public sealed record CreateStatisticRuleProperties
    {
        public required string Name { get; set; }
        public required string Value { get; set; }
        public string? StackingBonus { get; set; }
        public int LevelRequirement { get; set; }
        public string? DisplayName { get; set; }
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        public string? RequirementsExpression { get; set; }
    }

    public sealed record UpdateStatisticRuleProperties
    {
        public required string Name { get; set; }
        public required string Value { get; set; }
        public string? StackingBonus { get; set; }
        public int LevelRequirement { get; set; }
        public string? DisplayName { get; set; }
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        public string? RequirementsExpression { get; set; }
    }

    public sealed record CreateIncludeRuleProperties
    {
        public required Guid IncludedElementId { get; set; }
        public int LevelRequirement { get; set; }
        public string? RequirementsExpression { get; set; }
        public string? DisplayName { get; set; }
    }
}
