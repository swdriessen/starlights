using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Endpoints.Content.Elements;
using Starlights.Modules.Elements.Endpoints.Content.Elements.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetList;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageElementsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ManageElementsEndpointDriver _api;
    private readonly ManageElementRulesEndpointDriver _rulesApi;

    public ManageElementsDriver(IIntegrationHost integration, ManageElementsEndpointDriver endpointDriver, ManageElementRulesEndpointDriver rulesEndpointDriver)
    {
        _integration = integration;
        _api = endpointDriver;
        _rulesApi = rulesEndpointDriver;
    }

    public async Task<Guid> CreateElement(CreateProperties properties)
    {
        var request = new CreateElementRequest
        {
            Name = properties.Name,
            Type = properties.Type,
            Description = properties.Description
        };

        var id = await _api.CreateAsync(request);
        id.Should().NotBeEmpty();

        _integration.Set(id, "last-created-element-id");
        _integration.Set(properties, "last-created-element-properties");

        return id;
    }

    public Task<ElementDataModel> GetElementById(Guid id)
    {
        return _api.GetByIdAsync(id);
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
            LevelRequirement = properties.LevelRequirement
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
        response!.ElementId.Should().Be(elementId);
        response.Rules.Should().NotBeNull();
        return response.Rules;
    }

    public async Task<GetStatisticRuleResponse> GetStatisticRuleById(Guid elementId, Guid ruleId)
    {
        var response = await _rulesApi.GetStatisticRuleByIdAsync(elementId, ruleId);
        response.Should().NotBeNull();
        response!.ElementId.Should().Be(elementId);
        response.RuleId.Should().Be(ruleId);
        return response;
    }

    public sealed class CreateProperties
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public string? Description { get; set; }
    }

    public sealed class CreateStatisticRuleProperties
    {
        public required string Name { get; set; }
        public required string Value { get; set; }
        public string? StackingBonus { get; set; }
        public int LevelRequirement { get; set; }
    }
}
