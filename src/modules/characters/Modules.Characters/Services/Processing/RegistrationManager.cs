using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Services.Processing;

public class RegistrationManager : IRegistrationManager
{
    private readonly ILogger<RegistrationManager> _logger;
    private readonly IPersistence _persistence;
    private readonly IElementsModuleQueries _elements;
    private readonly IEnumerable<IRegistrationBehavior> _registrationBehaviors;

    public RegistrationManager(ILogger<RegistrationManager> logger,
                               IPersistence persistence,
                               IElementsModuleQueries elements,
                               IEnumerable<IRegistrationBehavior> registrationBehaviors)
    {
        _logger = logger;
        _persistence = persistence;
        _elements = elements;
        _registrationBehaviors = [.. registrationBehaviors];
    }

    public async Task<ProcessRegistrationResult> ProcessRegistration(RegistrationId registrationId)
    {
        using var processActivity = CharactersInstrumentation.StartActivity();

        var characters = _persistence.GetRepository<ICharactersRepository>();
        var registrations = _persistence.GetRepository<IRegistrationRepository>();

        // ensure the registration exists before processing
        var registration = await registrations.GetRegistrationAsync(registrationId);
        if (registration is null)
        {
            _logger.LogError("Registration with ID {RegistrationId} not found.", registrationId);
            return new ProcessRegistrationResult();
        }

        var context = new RegistrationProcessContext(registration, _persistence);

        await ProcessIncludeRules(context);
        await ProcessStatisticRules(context);
        await ProcessSelectionRules(context);

        registration.Processed();

        var affectedRows = await _persistence.SaveChangesAsync();

        processActivity?.SetTag("affectedRows", affectedRows);

        return new ProcessRegistrationResult() { AffectedRows = affectedRows };
    }

    private async Task ProcessIncludeRules(RegistrationProcessContext context)
    {
        using var includeActivity = CharactersInstrumentation.StartActivity();

        var currentRegistration = context.Registration;

        var currentElement = await _elements.GetElementWithRules(currentRegistration.AssociatedElementId);
        if (currentElement is null)
        {
            _logger.LogError("Element with ID {ElementId} not found for registration {RegistrationId}. Skipping include rules processing.", currentRegistration.AssociatedElementId, currentRegistration.Id);
            return;
        }

        // if the current element has no include rules, we can skip processing

        var registrations = context.GetRepository<IRegistrationRepository>();

        includeActivity?.DisplayName = $"ProcessIncludeRules | {currentElement.IncludeRules.Count}";

        foreach (var rule in currentElement.IncludeRules)
        {
            if (currentRegistration.HasAssociatedRule(rule.RuleId))
            {
                continue;
            }

            // when all requirements are met, we can include the element
            var newIncludeElement = await _elements.GetElementWithRules(rule.IncludedElementId);

            if (newIncludeElement is null)
            {
                _logger.LogError("Element with ID {ElementId} not found for registration {AssociatedElementName}. Skipping include rules processing.", rule.IncludedElementId, currentRegistration.AssociatedElementName);
                continue;
            }

            // create the new registration for the included element
            var newRegistration = Registration.Create(currentRegistration.CharacterId, new(newIncludeElement.Id), newIncludeElement.Name, newIncludeElement.Type);
            newRegistration.UpdateParentRegistration(currentRegistration);

            // create the new registration include rule, this is to keep track of the rules applied
            currentRegistration.CreateIncludeRule(new(rule.RuleId), new(newIncludeElement.Id), newIncludeElement.Name);

            registrations.Add(newRegistration);
            context.NewRegistrations.Add(newRegistration);

            // apply any registration behavior in the current context
            foreach (var behavior in _registrationBehaviors)
            {
                await behavior.Registered(newRegistration, context);
            }
        }
    }

    private async Task ProcessStatisticRules(RegistrationProcessContext context)
    {
        using var statisticsActivity = CharactersInstrumentation.StartActivity();

        var currentRegistration = context.Registration;

        var currentElement = await _elements.GetElementWithRules(currentRegistration.AssociatedElementId);
        if (currentElement is null)
        {
            _logger.LogError("Element with ID {ElementId} not found for registration {RegistrationId}. Skipping include rules processing.", currentRegistration.AssociatedElementId, currentRegistration.Id);
            return;
        }

        // if the current element has no statistic rules, we can skip processing

        var registrations = context.GetRepository<IRegistrationRepository>();

        statisticsActivity?.DisplayName = $"ProcessStatisticRules | {currentElement.StatisticRules.Count}";

        foreach (var rule in currentElement.StatisticRules)
        {
            if (currentRegistration.HasAssociatedRule(rule.RuleId))
            {
                continue;
            }

            // create the new registration statistic rule, this is to keep track of the rules applied
            var newStatisticRule = currentRegistration.CreateStatisticRule(new(rule.RuleId), rule.Name, rule.Value);

            if (rule.StackingBonus is not null)
            {
                newStatisticRule.UpdateStackingBonus(rule.StackingBonus);
            }

            if (rule.LevelRequirement > 0)
            {
                newStatisticRule.UpdateLevelRequirement(rule.LevelRequirement);
            }
        }
    }

    private async Task ProcessSelectionRules(RegistrationProcessContext context)
    {
        using var selectionActivity = CharactersInstrumentation.StartActivity();

        var currentRegistration = context.Registration;

        var currentElement = await _elements.GetElementWithRules(currentRegistration.AssociatedElementId);
        if (currentElement is null)
        {
            _logger.LogError("Element with ID {ElementId} not found for registration {RegistrationId}. Skipping include rules processing.", currentRegistration.AssociatedElementId, currentRegistration.Id);
            return;
        }

        // if the current element has no selection rules, we can skip processing

        var registrations = context.GetRepository<IRegistrationRepository>();

        selectionActivity?.DisplayName = $"ProcessSelectionRules | {currentElement.SelectionRules.Count}";

        foreach (var rule in currentElement.SelectionRules)
        {
            if (currentRegistration.HasAssociatedRule(rule.RuleId))
            {
                continue;
            }

            // create the new registration selection rule, this is to keep track of the rules applied
            _ = currentRegistration.CreateSelectionRule(new(rule.RuleId), rule.ElementType, rule.Name);
        }
    }
}
