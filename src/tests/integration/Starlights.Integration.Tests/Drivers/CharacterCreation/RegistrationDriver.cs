using FluentAssertions;
using Starlights.Integration.Constants;
using Starlights.Integration.Core;
using Starlights.Integration.Core.Eventing;
using Starlights.Integration.Core.Extensions;
using Starlights.Modules.Characters.Domain.Classes.Eventing;
using Starlights.Modules.Characters.Domain.Registrations.Eventing;
using Starlights.Modules.Characters.Endpoints.Generation.Registrations.GetRegistrations;
using Starlights.Modules.Characters.Endpoints.Generation.Statistics.GetStatistics;
using Starlights.Modules.Characters.Endpoints.Models;

namespace Starlights.Integration.Drivers.CharacterCreation;

internal sealed class RegistrationDriver : IDriver
{
    private readonly IIntegrationHost _integration;
    private readonly EventObserverCollection _events;
    private readonly RegistrationEndpointDriver _api;

    public RegistrationDriver(IIntegrationHost integration, RegistrationEndpointDriver api)
    {
        _integration = integration;
        _api = api;

        _events = _integration.GetEventObserverCollection();
    }

    public async Task<List<RegistrationDataModel>> GetRegistrations()
    {
        var characterId = _integration.GetCharacterIdentifier();

        var response = await _api.GetRegistrationsAsync(characterId);
        response.Registrations.Should().NotBeEmpty("Expected at least one registration to be available.");
        return response.Registrations;
    }

    public async Task<List<StatisticGroupDataModel>> GetStatistics()
    {
        var characterId = _integration.GetCharacterIdentifier();

        var response = await _api.GetStatisticsAsync(characterId);
        response.Statistics.Should().NotBeEmpty("Expected at least one statistic to be available.");
        return response.Statistics;
    }

    public async Task<List<SelectionRuleDataModel>> GetSelectionRules(params string[] types)
    {
        var response = await _api.GetSelectionRulesAsync(types);
        //response.Rules.Should().NotBeEmpty("Expected at least one selection rule to be available.");
        return response.Rules;
    }

    public async Task<SelectionRuleDataModel> GetSingleSelectionRule(string type)
    {
        var response = await _api.GetSelectionRulesAsync([type]);
        response.Rules.Should().HaveCount(1, $"Expected exactly one selection rule of type '{type}' to be available.");

        return response.Rules.Single();
    }

    public async Task<List<SelectionRuleOptionModel>> GetSelectionRuleOptions(Guid selectionRuleId)
    {
        var response = await _api.GetSelectionRuleOptionsAsync(selectionRuleId);
        response.Options.Should().NotBeEmpty("Expected at least one selection rule to be available.");

        return response.Options;
    }

    public async Task<SelectionRuleOptionModel> GetSelectionRuleOption(Guid selectionRuleId, string elementName)
    {
        if (string.IsNullOrWhiteSpace(elementName))
        {
            throw new ArgumentException("Element name cannot be null or empty.", nameof(elementName));
        }

        var options = await GetSelectionRuleOptions(selectionRuleId);

        var singleOption = options.SingleOrDefault(o => o.Name.Contains(elementName));
        singleOption.Should().NotBeNull($"Expected exactly one option to contain '{elementName}' in its name.");

        return singleOption;
    }

    public async Task<Guid> RegisterSelectionRule(SelectionRuleDataModel rule, SelectionRuleOptionModel option)
    {
        var response = await _api.RegisterSelectionRuleAsync(rule.RegistrationId, rule.RegistrationSelectionRuleId, option.ElementId);
        response.RegistrationId.Should().NotBe(Guid.Empty, "Expected a valid registration ID to be returned.");

        await _events.EnsureObservation<RegistrationCreatedEvent>(x => x.RegistrationId == response.RegistrationId);

        return response.RegistrationId;
    }

    public Task UnregisterSelectionRule(SelectionRuleDataModel rule, SelectionRuleOptionModel option)
    {
        return _api.UnregisterSelectionRuleAsync(rule.RegistrationId, rule.RegistrationSelectionRuleId, option.ElementId);
    }

    #region Potential Extension Methods
    public async Task<(SelectionRuleDataModel Rule, SelectionRuleOptionModel Option, Guid RegistrationId)> RegisterClass(string className)
    {
        _integration.WriteLine($"Registering class '{className}'...");
        var classRule = await GetSingleSelectionRule(SelectionRuleTypes.Class);
        var classOption = await GetSelectionRuleOption(classRule.RegistrationSelectionRuleId, className);
        var registrationId = await RegisterSelectionRule(classRule, classOption);
        await _events.EnsureObservation<CharacterClassCreatedEvent>();
        return (classRule, classOption, registrationId);
    }

    public async Task UnregisterClass(string className)
    {
        var classRule = await GetSingleSelectionRule(SelectionRuleTypes.Class);
        var classOption = await GetSelectionRuleOption(classRule.RegistrationSelectionRuleId, className);
        await UnregisterSelectionRule(classRule, classOption);
        await _events.EnsureObservation<CharacterClassRemovedEvent>();
    }

    public async Task<(SelectionRuleDataModel Rule, SelectionRuleOptionModel Option, Guid RegistrationId)> RegisterSubClass(string name)
    {
        var classRule = await GetSingleSelectionRule(SelectionRuleTypes.SubClass);
        var classOption = await GetSelectionRuleOption(classRule.RegistrationSelectionRuleId, name);
        var registrationId = await RegisterSelectionRule(classRule, classOption);
        await _events.EnsureObservation<RegistrationCreatedEvent>(x => x.RegistrationId == registrationId); // wait for the registration event
        return (classRule, classOption, registrationId);
    }
    #endregion
}