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

    public RegistrationProcessor(
        ILogger<RegistrationProcessor> logger,
        IPersistence persistence,
        IRegistrationManager registrationManager,
        IElementsModuleQueries elements)
    {
        _logger = logger;
        _persistence = persistence;
        _registrationManager = registrationManager;
        _elements = elements;
    }

    public async Task<ProcessRegistrationResult> ProcessRegistration(RegistrationId registrationId)
    {
        using var _ = CharactersInstrumentation.StartActivity();

        var context = await InitializeContext(registrationId);

        _logger.LogInformation("processing registration '{ElementName} ({ElementType})'", context.Registration.AssociatedElementName, context.Registration.AssociatedElementType);

        await ProcessIncludeRules(context);
        await ProcessStatisticRules(context);
        await ProcessSelectionRules(context);

        context.Registration.Processed();

        var affectedRows = await _persistence.SaveChangesAsync();

        _logger.LogInformation("processing registration completed for '{CharacterName}' with affectedRows {AffectedRows}", context.Character.Name, affectedRows);

        return ProcessRegistrationResult.Success(affectedRows);
    }

    public async Task<ProcessRegistrationResult> ReproccessRegistrations(CharacterId characterId)
    {
        using var _ = CharactersInstrumentation.StartActivity();

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



        foreach (var registration in registrations)
        {
            // temp to fix tests, use init context method later
            var associatedElement = await _elements.GetElementWithRules(registration.AssociatedElementId) ??
                throw new RegistrationProcessingException($"The associated element '{registration.AssociatedElementId}' for the registration '{registration.Id.Value}' was not found.");


            var context = new ProcessingContext(registration, character, _persistence);
            context.SetAssociatedElement(associatedElement);

            // TODO: check if the registration itself still meets its requirements

            await ProcessIncludeRules(context);
            await ProcessStatisticRules(context);
            await ProcessSelectionRules(context);
        }

        var affectedRows = await _persistence.SaveChangesAsync();

        _logger.LogInformation("reprocessing character completed for '{CharacterName}' with affectedRows {AffectedRows}", character.Name, affectedRows);

        return ProcessRegistrationResult.Success(affectedRows);
    }

    private async Task ProcessIncludeRules(ProcessingContext context)
    {
        using var _ = CharactersInstrumentation.StartActivity();

        _logger.LogInformation("processing include rules for registration '{ElementName} ({ElementType})'",
            context.Registration.AssociatedElementName, context.Registration.AssociatedElementType);

        await ProcessExistingIncludeRules(context);
        await ProcessElementIncludeRules(context);
    }

    private async Task ProcessExistingIncludeRules(ProcessingContext context)
    {
        if (!context.Registration.HasIncludeRules())
        {
            return;
        }

        var registrationRepository = _persistence.GetRepository<IRegistrationRepository>();

        var associatedElement = context.GetAssociatedElement();

        foreach (var existingRule in context.Registration.IncludeRules.ToList())
        {
            var ruleDefinition = associatedElement.IncludeRules.SingleOrDefault(r => r.RuleId == existingRule.AssociatedIncludeRuleId)
                ?? throw new RegistrationProcessingException($"The associated include rule not found for existing rule '{existingRule.Id}'.");

            // check level requirement for now
            if (ruleDefinition.LevelRequirement > 0) // no lookup needed if no level requirement
            {
                var progressionLevel = context.GetProgressionLevel(context.Registration);
                if (progressionLevel < ruleDefinition.LevelRequirement)
                {
                    _logger.LogInformation("removing include rule '{RuleId}' from registration {ElementName} ({ElementType}) due to level requirement not met (required: {RequiredLevel}, current: {CurrentLevel})",
                        existingRule.Id, context.Registration.AssociatedElementName, context.Registration.AssociatedElementType, ruleDefinition.LevelRequirement, progressionLevel);

                    // remove the include rule from the current registration
                    context.Registration.RemoveIncludeRule(existingRule);

                    // unregister the included registration, as the rule is now removed
                    var includedRegistation = await registrationRepository.GetRegistrationByOriginatingRuleAsync(existingRule.Id);
                    if (includedRegistation is not null)
                    {
                        await _registrationManager.Unregister(includedRegistation);
                    }
                }
            }
        }
    }

    private async Task ProcessElementIncludeRules(ProcessingContext context)
    {
        var registrationElement = context.GetAssociatedElement();

        foreach (var ruleDefinition in registrationElement.IncludeRules)
        {
            if (context.Registration.HasAssociatedRule(ruleDefinition.RuleId))
            {
                // rule applied, skip
                continue;
            }

            if (ruleDefinition.LevelRequirement > 0)
            {
                var progressionLevel = context.GetProgressionLevel(context.Registration);
                if (progressionLevel < ruleDefinition.LevelRequirement)
                {
                    // level requirement not met, skip
                    continue;
                }
            }

            // get the element to be included according to the rule
            var newElement = await _elements.GetElementWithRules(ruleDefinition.IncludedElementId);
            if (newElement is null)
            {
                throw new RegistrationProcessingException($"The included element '{ruleDefinition.IncludedElementId}' for the include rule '{ruleDefinition.RuleId}' was not found.");
                //_logger.LogError("Included element with ID {ElementId} not found for include rule {RuleId}. Skipping include rules processing.", ruleDefinition.IncludedElementId, ruleDefinition.RuleId);
                //continue;
            }

            // create the new registration include rule, this is to keep track of the rules applied
            var newIncludeRule = context.Registration.CreateIncludeRule(new(ruleDefinition.RuleId), new(newElement.Id), newElement.Name);

            // create the new registration for the included element
            var newRegistration = Registration.Create(context.Character.Id, new(newElement.Id), newElement.Name, newElement.Type);
            newRegistration.SetParentRegistration(context.Registration);
            newRegistration.SetOriginatingRule(newIncludeRule.Id);
            newRegistration.SetProgressionOrigin(context.Registration);

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

    /// <summary>
    /// Initializes a new processing context for the specified registration by retrieving all required domain entities.
    /// </summary>
    /// <remarks>This method retrieves all necessary data for processing a registration, including the
    /// registration itself, the related character, and the associated element. All dependencies must exist; otherwise,
    /// a <see cref="RegistrationProcessingException"/> is thrown.</remarks>
    /// <exception cref="RegistrationProcessingException">Thrown if the registration, associated character, or associated element cannot be found for the specified
    /// registration ID.</exception>
    private async Task<ProcessingContext> InitializeContext(RegistrationId registrationId)
    {
        var registrationRepository = _persistence.GetRepository<IRegistrationRepository>();
        var registration = await registrationRepository.GetRegistrationAsync(registrationId)
            ?? throw new RegistrationProcessingException($"The registration with ID '{registrationId}' not found.");

        var characterRepository = _persistence.GetRepository<ICharactersRepository>();
        var character = await characterRepository.GetCharacterAsync(registration.CharacterId)
            ?? throw new RegistrationProcessingException($"The character with ID '{registration.CharacterId}' not found for registration {registration.Id}.");

        var associatedElement = await _elements.GetElementWithRules(registration.AssociatedElementId) ??
            throw new RegistrationProcessingException($"The associated element '{registration.AssociatedElementId}' for the registration '{registration.Id.Value}' was not found.");

        // we have the registration, character and associated element, we can now create the context for processing
        var context = new ProcessingContext(registration, character, _persistence);
        context.SetAssociatedElement(associatedElement);

        var registrations = await registrationRepository.GetRegistrationsAsync(context.Character.Id);
        context.SetCharacterRegistations(registrations);

        return context;
    }
}
