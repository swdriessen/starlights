using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Services.Processing;

public class RegistrationProcessor : IRegistrationProcessor
{
    private readonly ILogger<RegistrationProcessor> _logger;
    private readonly IPersistence _persistence;
    private readonly IRegistrationManager _registrationManager;
    private readonly IElementsModuleQueries _elements;

    public RegistrationProcessor(ILogger<RegistrationProcessor> logger, IPersistence persistence, IRegistrationManager registrationManager, IElementsModuleQueries elements)
    {
        _logger = logger;
        _persistence = persistence;
        _registrationManager = registrationManager;
        _elements = elements;
    }

    public async Task<ProcessRegistrationResult> ProcessRegistration(RegistrationId registrationId)
    {
        using var processActivity = CharactersInstrumentation.StartActivity();

        var registrationRepository = _persistence.GetRepository<IRegistrationRepository>();
        var registration = await registrationRepository.GetRegistrationAsync(registrationId);
        if (registration is null)
        {
            _logger.LogError("Registration with ID {RegistrationId} not found.", registrationId);
            return ProcessRegistrationResult.Failure("Registration not found");
        }

        _logger.LogInformation("processing registration '{ElementName} ({ElementType})'", registration.AssociatedElementName, registration.AssociatedElementType);

        var characterRepository = _persistence.GetRepository<ICharactersRepository>();
        var character = await characterRepository.GetCharacterAsync(registration.CharacterId);
        if (character is null)
        {
            _logger.LogError("Character with ID {CharacterId} not found for registration {RegistrationId}.", registration.CharacterId, registration.Id);
            return ProcessRegistrationResult.Failure("Character associated with registration not found");
        }

        var context = new ProcessingContext(registration, character, _persistence);

        await ProcessIncludeRules(context);
        await ProcessStatisticRules(context);
        await ProcessSelectionRules(context);

        registration.Processed();

        var affectedRows = await _persistence.SaveChangesAsync();

        return ProcessRegistrationResult.Success(affectedRows);
    }

    public async Task<ProcessRegistrationResult> ReproccessRegistrations(CharacterId characterId)
    {
        using var activity = CharactersInstrumentation.StartActivity();

        var characterRepository = _persistence.GetRepository<ICharactersRepository>();
        var character = await characterRepository.GetCharacterAsync(characterId);
        if (character is null)
        {
            _logger.LogError("Character with ID {CharacterId} not found.", characterId);
            return ProcessRegistrationResult.Failure("Character not found");
        }

        _logger.LogInformation("reprocessing registrations for '{CharacterName}'", character.Name);

        var registrationRepository = _persistence.GetRepository<IRegistrationRepository>();
        var registrations = await registrationRepository.GetRegistrationsAsync(characterId);

        _logger.LogInformation("found {RegistrationCount} registrations for character '{CharacterName}'", registrations.Count, character.Name);

        // TODO: Optimize this to only reprocess registrations that need it (e.g. have unprocessed rules or are marked as needing reprocessing)
        // For now, we reprocess all registrations to ensure consistency

        foreach (var registration in registrations)
        {
            var context = new ProcessingContext(registration, character, _persistence);

            // TODO: check if the registration itself still meets its requirements

            await ProcessIncludeRules(context);
            await ProcessStatisticRules(context);
            await ProcessSelectionRules(context);
        }

        _logger.LogInformation("reprocessed {RegistrationCount} registrations for character '{CharacterName}'", registrations.Count, character.Name);

        var affectedRows = await _persistence.SaveChangesAsync();
        activity?.SetTag("affectedRows", affectedRows);

        _logger.LogInformation("reprocessing complete for '{CharacterName}' with affectedRows {AffectedRows}", character.Name, affectedRows);

        return ProcessRegistrationResult.Success(affectedRows);
    }

    private async Task ProcessIncludeRules(ProcessingContext context)
    {
        using var includeActivity = CharactersInstrumentation.StartActivity();

        var currentRegistration = context.Registration;

        var currentElement = await _elements.GetElementWithRules(currentRegistration.AssociatedElementId);
        if (currentElement is null)
        {
            _logger.LogError("Element with ID {ElementId} not found for registration {RegistrationId}. Skipping include rules processing.", currentRegistration.AssociatedElementId, currentRegistration.Id);
            return;
        }

        // check for existing include rules first - this is done when reprocessing registrations e.g. at character level up
        if (currentRegistration.IncludeRules.Count > 0)
        {
            var registrationRepository = context.GetRepository<IRegistrationRepository>();
            var registrations = await registrationRepository.GetRegistrationsAsync(context.Character.Id);
            // check if processed include rules still meet the requirements,
            // if not, we should remove them

            // we need to iterate over a copy of the list, as we might modify the original list during iteration
            foreach (var existingRule in currentRegistration.IncludeRules.ToList())
            {
                _logger.LogInformation("checking existing include rule {RuleId} for registration {RegistrationId}", existingRule.AssociatedIncludeRuleId, currentRegistration.Id);

                var ruleDefinition = currentElement.IncludeRules.SingleOrDefault(r => r.RuleId == existingRule.AssociatedIncludeRuleId);
                if (ruleDefinition is null)
                {
                    _logger.LogError("Include rule with ID {RuleId} not found for registration {RegistrationId}. Skipping include rules processing.", existingRule.AssociatedIncludeRuleId, currentRegistration.Id);
                    continue;
                }

                // apply progression-aware level gating
                if (ruleDefinition.LevelRequirement > 0)
                {
                    var level = context.GetProgressionLevel(currentRegistration);
                    if (level < ruleDefinition.LevelRequirement)
                    {
                        _logger.LogInformation("removing include rule {RuleId} from registration {RegistrationId} due to level requirement not met (required: {RequiredLevel}, current: {CurrentLevel})", existingRule.AssociatedIncludeRuleId, currentRegistration.Id, ruleDefinition.LevelRequirement, level);
                        // remove the include rule from the current registration
                        currentRegistration.RemoveIncludeRule(existingRule);

                        // unregister the associated registration, as it is now removed
                        // we can assume that there is only one registration per character per included element
                        var associated = registrations.SingleOrDefault(r => r.OriginatingRule == existingRule.Id && r.CharacterId == currentRegistration.CharacterId);
                        if (associated is not null)
                        {
                            _logger.LogInformation("unregistering associated registration {AssociatedRegistrationId} for include rule {RuleId} from registration {RegistrationId}", associated.Id, existingRule.AssociatedIncludeRuleId, currentRegistration.Id);
                            await _registrationManager.Unregister(associated);
                        }
                    }
                }
            }

        }




        // if the current element has no include rules, we can skip processing

        includeActivity?.DisplayName = $"ProcessIncludeRules | {currentElement.IncludeRules.Count}";

        foreach (var rule in currentElement.IncludeRules)
        {
            if (currentRegistration.HasAssociatedRule(rule.RuleId))
            {
                continue;
            }

            // IRuleConditionEvaluator could be used here to
            // evaluate other requirements here in the future (e.g. stats, tags, etc.)

            // apply progression-aware level gating
            if (rule.LevelRequirement > 0)
            {
                var level = context.GetProgressionLevel(currentRegistration);

                if (level < rule.LevelRequirement)
                {
                    continue;
                }
            }

            // when all requirements are met, we can include the element
            var newIncludeElement = await _elements.GetElementWithRules(rule.IncludedElementId);
            if (newIncludeElement is null)
            {
                _logger.LogError("Element with ID {ElementId} not found for registration {AssociatedElementName}. Skipping include rules processing.", rule.IncludedElementId, currentRegistration.AssociatedElementName);
                continue;
            }

            // create the new registration include rule, this is to keep track of the rules applied
            var newIncludeRule = currentRegistration.CreateIncludeRule(new(rule.RuleId), new(newIncludeElement.Id), newIncludeElement.Name);

            // create the new registration for the included element
            var newRegistration = Registration.Create(currentRegistration.CharacterId, new(newIncludeElement.Id), newIncludeElement.Name, newIncludeElement.Type);
            newRegistration.SetParentRegistration(currentRegistration);
            newRegistration.SetOriginatingRule(newIncludeRule.Id);
            newRegistration.SetProgressionOrigin(currentRegistration);

            await _registrationManager.Register(newRegistration);
        }
    }

    private async Task ProcessStatisticRules(ProcessingContext context)
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

            // apply progression-aware level gating
            if (rule.LevelRequirement > 0)
            {
                var level = context.GetProgressionLevel(currentRegistration);
                if (level < rule.LevelRequirement)
                {
                    continue;
                }
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

    private async Task ProcessSelectionRules(ProcessingContext context)
    {
        using var selectionActivity = CharactersInstrumentation.StartActivity();

        var currentRegistration = context.Registration;

        var currentElement = await _elements.GetElementWithRules(currentRegistration.AssociatedElementId);
        if (currentElement is null)
        {
            _logger.LogError("Element with ID {ElementId} not found for registration {RegistrationId}. Skipping include rules processing.", currentRegistration.AssociatedElementId, currentRegistration.Id);
            return;
        }


        var registrationRepository = context.GetRepository<IRegistrationRepository>();

        // check for existing include rules first - this is done when reprocessing registrations e.g. at character level up
        if (currentRegistration.SelectionRules.Count > 0)
        {
            var registrations = await registrationRepository.GetRegistrationsAsync(context.Character.Id);
            // check if processed include rules still meet the requirements,
            // if not, we should remove them

            foreach (var existingRule in currentRegistration.SelectionRules.ToList())
            {
                _logger.LogInformation("checking existing selection rule {ElementType}", existingRule.ElementType);


                var ruleDefinition = currentElement.SelectionRules.SingleOrDefault(r => r.RuleId == existingRule.AssociatedSelectionRuleId);
                if (ruleDefinition is null)
                {
                    _logger.LogError("Selection rule with ID {RuleId} not found for registration {RegistrationId}. Skipping selection rules processing.", existingRule.AssociatedSelectionRuleId, currentRegistration.Id);
                    continue;
                }

                // apply progression-aware level gating
                if (ruleDefinition.LevelRequirement > 0)
                {
                    var level = context.GetProgressionLevel(currentRegistration);
                    if (level < ruleDefinition.LevelRequirement)
                    {
                        _logger.LogInformation("removing selection rule {RuleId} from registration {RegistrationId} due to level requirement not met (required: {RequiredLevel}, current: {CurrentLevel})", existingRule.AssociatedSelectionRuleId, currentRegistration.Id, ruleDefinition.LevelRequirement, level);
                        // remove the include rule from the current registration
                        currentRegistration.RemoveSelectionRule(existingRule);

                        // unregister the associated registration, as it is now removed
                        // we can assume that there is only one registration per character per included element
                        var associated = registrations.SingleOrDefault(r => r.OriginatingRule == existingRule.Id && r.CharacterId == currentRegistration.CharacterId);
                        if (associated is not null)
                        {
                            _logger.LogInformation("unregistering associated registration {AssociatedRegistrationId} for selection rule {RuleId} from registration {RegistrationId}", associated.Id, existingRule.AssociatedSelectionRuleId, currentRegistration.Id);
                            await _registrationManager.Unregister(associated);
                        }
                    }
                }
            }

        }















        // if the current element has no selection rules, we can skip processing


        selectionActivity?.DisplayName = $"ProcessSelectionRules | {currentElement.SelectionRules.Count}";

        foreach (var rule in currentElement.SelectionRules)
        {
            if (currentRegistration.HasAssociatedRule(rule.RuleId))
            {
                continue;
            }

            // apply progression-aware level gating
            if (rule.LevelRequirement > 0)
            {
                var level = context.GetProgressionLevel(currentRegistration);
                if (level < rule.LevelRequirement)
                {
                    continue;
                }
            }

            // create the new registration selection rule, this is to keep track of the rules applied
            _ = currentRegistration.CreateSelectionRule(new(rule.RuleId), rule.ElementType, rule.Name);
        }
    }
}
