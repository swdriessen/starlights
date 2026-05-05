using System.Net.Http.Json;
using AwesomeAssertions;
using Starlights.Integration.Drivers.Elements.Endpoints;
using Starlights.Integration.Eventing;
using Starlights.Integration.Extensions;
using Starlights.Modules.Elements.Domain.Eventing;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Includes.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Selections.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Create;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetById;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.GetList;
using Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Update;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Elements;
using Starlights.Modules.Elements.Endpoints.ContentManagement.Elements.Create;

namespace Starlights.Integration.Drivers.Elements;

public sealed class ManageElementsDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly ElementsDriverContext _driverContext;
    private readonly EventObserverCollection _events;
    private readonly ManageElementsEndpointDriver _api;
    private readonly ManageElementRulesEndpointDriver _rulesApi;
    private readonly ManageElementLabelsEndpointDriver _labelsApi;

    public ManageElementsDriver(
        IIntegrationHost integration,
        ElementsDriverContext driverContext,
        ManageElementsEndpointDriver endpointDriver,
        ManageElementRulesEndpointDriver rulesEndpointDriver,
        ManageElementLabelsEndpointDriver labelsEndpointDriver)
    {
        _integration = integration;
        _driverContext = driverContext;
        _api = endpointDriver;
        _rulesApi = rulesEndpointDriver;
        _labelsApi = labelsEndpointDriver;
        _events = _integration.Events;
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

        _driverContext.WithCreatedElement(id, properties.Name, properties.Type);

        await _events.EnsureObservation<ElementCreatedEvent>(e => e.ElementId == id && e.Type == properties.Type);

        return id;
    }

    public Task<ElementDataModel> GetElementById(Guid id)
    {
        return _api.GetByIdAsync(id);
    }

    public Task<ElementDataModel> GetElementByName(string name)
    {
        var element = _driverContext.GetElement(name);
        return GetElementById(element.Id);
    }

    public Task<ElementDataModel> GetLastCreatedElement()
    {
        return GetElementById(_driverContext.CurrentElement.Id);
    }

    public async Task<ManageElementsEndpointDriver.UpdateElementResponse> UpdateElement(Guid elementId, UpdateProperties properties)
    {
        var current = await GetElementById(elementId);

        var request = new ManageElementsEndpointDriver.UpdateElementRequest
        {
            Id = elementId,
            Name = string.IsNullOrWhiteSpace(properties.Name) ? current.Name : properties.Name,
            Description = properties.Description
        };

        var (response, statusCode) = await _api.UpdateAsync(elementId, request);
        statusCode.Should().Be(System.Net.HttpStatusCode.OK);
        response.Should().NotBeNull();

        return response!;
    }

    public async Task<List<ElementDataModel>> GetElements(string? type = null)
    {
        var response = await _api.GetListAsync(type);
        return [.. response.Items];
    }

    public async Task AddLabels(Guid elementId, IReadOnlyCollection<string> labels)
    {
        var statusCode = await _labelsApi.CreateAsync(elementId, labels);
        statusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    public Task<IReadOnlyList<string>> GetLabels(Guid elementId)
    {
        return _labelsApi.ListAsync(elementId);
    }

    public async Task ReplaceLabels(Guid elementId, IReadOnlyCollection<string> labels)
    {
        var (_, statusCode) = await _labelsApi.UpdateAsync(elementId, labels);
        statusCode.Should().Be(System.Net.HttpStatusCode.OK);
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

    public async Task<bool> DeleteRules(Guid elementId, IReadOnlyList<Guid> ruleIds)
    {
        var (success, _) = await _rulesApi.DeleteRulesAsync(elementId, ruleIds);
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

        _integration.IntegrationContext.WriteLine($"Creating include rule for ElementId={elementId} with IncludedElementId={properties.IncludedElementId}");
        var response = await _rulesApi.CreateIncludeRuleAsync(elementId, request);

        response.ElementId.Should().Be(elementId);
        response.RuleId.Should().NotBeEmpty();

        _integration.Set(response, "last-created-include-rule");
        _integration.Set(properties, "last-created-include-rule-properties");

        return response;
    }

    public async Task<IReadOnlyList<GetIncludeRulesResponse.IncludeRuleItem>> GetIncludeRules(Guid elementId)
    {
        _integration.IntegrationContext.WriteLine($"Getting include rules for ElementId={elementId}");
        using var client = _integration.CreateClient();
        var response = await client.GetFromJsonAsync<GetIncludeRulesResponse>($"/api/elements/{elementId}/rules/includes");
        response.Should().NotBeNull();
        response.ElementId.Should().Be(elementId);
        return response.Rules;
    }

    public async Task<GetIncludeRuleResponse> GetIncludeRuleById(Guid elementId, Guid ruleId)
    {
        _integration.IntegrationContext.WriteLine($"Getting include rule by ID: ElementId={elementId}, RuleId={ruleId}");
        using var client = _integration.CreateClient();
        var response = await client.GetFromJsonAsync<GetIncludeRuleResponse>($"/api/elements/{elementId}/rules/includes/{ruleId}");
        response.Should().NotBeNull();
        response.ElementId.Should().Be(elementId);
        response.RuleId.Should().Be(ruleId);
        return response;
    }

    public async Task<CreateSelectionRuleResponse> CreateSelectionRule(Guid elementId, CreateSelectionRuleProperties properties)
    {
        var request = new CreateSelectionRuleRequest(
            ElementId: elementId,
            DisplayName: properties.DisplayName,
            Type: properties.Type,
            Supports: properties.Supports,
            Range: properties.Range,
            Quantity: properties.Quantity,
            Optional: properties.Optional,
            LevelRequirement: properties.LevelRequirement,
            Requirements: properties.Requirements);

        var response = await _rulesApi.CreateSelectionRuleAsync(elementId, request);

        response.ElementId.Should().Be(elementId);
        response.RuleId.Value.Should().NotBeEmpty();

        _integration.Set(response, "last-created-selection-rule");
        _integration.Set(properties, "last-created-selection-rule-properties");

        return response;
    }

    public async Task<IReadOnlyList<GetSelectionRulesResponse.SelectionRuleItem>> GetSelectionRules(Guid elementId)
    {
        var response = await _rulesApi.GetSelectionRulesAsync(elementId);
        response.Should().NotBeNull();
        response.Rules.Should().NotBeNull();
        return response!.Rules;
    }

    public async Task<GetSelectionRuleResponse> GetSelectionRuleById(Guid elementId, Guid ruleId)
    {
        var response = await _rulesApi.GetSelectionRuleByIdAsync(elementId, ruleId);
        response.Should().NotBeNull();
        response!.RuleId.Should().Be(ruleId);
        return response;
    }

    public async Task<bool> DeleteElement(Guid elementId)
    {
        var (success, _) = await _api.DeleteAsync(elementId);
        return success;
    }

    public sealed record CreateProperties
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public string? Description { get; set; }
    }

    public sealed record UpdateProperties
    {
        public string? Name { get; set; }
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

    public sealed record CreateSelectionRuleProperties
    {
        public required string DisplayName { get; set; }
        public required string Type { get; set; }
        public string? Supports { get; set; }
        public string? Range { get; set; }
        public int Quantity { get; set; } = 1;
        public bool Optional { get; set; }
        public int LevelRequirement { get; set; }
        public string? Requirements { get; set; }
        public string? Default { get; set; }
    }
}
