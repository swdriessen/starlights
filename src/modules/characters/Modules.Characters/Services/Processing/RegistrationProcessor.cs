using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Data;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;
using Starlights.Modules.Characters.Services.Statistics;
using Starlights.Modules.Elements.Integration;
using Starlights.Platform.Data;

namespace Starlights.Modules.Characters.Services.Processing;

public class RegistrationProcessor : IRegistrationProcessor
{
    private readonly ILogger<RegistrationProcessor> _logger;
    private readonly IPersistence _persistence;
    private readonly IRegistrationManager _registrationManager;
    private readonly IElementsModuleQueries _elements;
    private readonly StatisticsCalculator _statisticsCalculator;

    public RegistrationProcessor(
        ILogger<RegistrationProcessor> logger,
        IPersistence persistence,
        IRegistrationManager registrationManager,
        IElementsModuleQueries elements, StatisticsCalculator statisticsCalculator)
    {
        _logger = logger;
        _persistence = persistence;
        _registrationManager = registrationManager;
        _elements = elements;
        _statisticsCalculator = statisticsCalculator;
    }

    public async Task<ProcessRegistrationResult> ProcessRegistration(RegistrationId registrationId)
    {
        using var _ = CharactersInstrumentation.StartActivity();

        var context = await InitializeContext(registrationId);

        _logger.LogInformation("processing registration '{ElementName} ({ElementType})'", context.Registration.AssociatedElementName, context.Registration.AssociatedElementType);

        await ProcessIncludeRules(context);
        await ProcessSelectionRules(context);
        await ProcessStatisticRules(context);

        context.Registration.Processed();

        //var registrationRepository = _persistence.GetRepository<IRegistrationRepository>();
        //var registrations = await registrationRepository.GetRegistrationsAsync(context.Character.Id);
        //context.SetCharacterRegistations(registrations);
        //_statisticsCalculator.Calculate(context.Character, context.GetCharacterRegistrations());

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

        _statisticsCalculator.Calculate(character, registrations);

        var affectedRows = await _persistence.SaveChangesAsync();

        _logger.LogInformation("reprocessing character completed for '{CharacterName}' with affectedRows {AffectedRows}", character.Name, affectedRows);

        return ProcessRegistrationResult.Success(affectedRows);
    }

    #region Include Rules

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
                var levelProgression = context.GetProgressionLevel(context.Registration);
                if (levelProgression < ruleDefinition.LevelRequirement)
                {
                    // level requirement not met, skip
                    continue;
                }
            }

            // get the element to be included according to the rule
            var newElement = await _elements.GetElementWithRules(ruleDefinition.IncludedElementId)
                ?? throw new RegistrationProcessingException($"The included element '{ruleDefinition.IncludedElementId}' for the include rule '{ruleDefinition.RuleId}' was not found.");

            // create the new registration include rule, this is to keep track of the rules applied
            var newIncludeRule = context.Registration.CreateIncludeRule(new(ruleDefinition.RuleId), new(newElement.Id), newElement.Name);

            // create the new registration for the included element
            var newRegistration = Registration.Create(context.Character.Id, new(newElement.Id), newElement.Name, newElement.Type);
            newRegistration.SetParentRegistration(context.Registration);
            newRegistration.SetOriginatingRule(newIncludeRule.Id);
            newRegistration.SetProgressionOrigin(context.Registration);

            _logger.LogInformation("created new include rule '{RuleId}' on registration {ElementName} ({ElementType}) including element '{IncludedElementName} ({IncludedElementType})' as registration '{IncludedRegistrationId}'",
                newIncludeRule.Id.Value, context.Registration.AssociatedElementName, context.Registration.AssociatedElementType,
                newElement.Name, newElement.Type, newRegistration.Id.Value);

            await _registrationManager.Register(newRegistration);
        }
    }

    #endregion

    #region Selection Rules

    private async Task ProcessSelectionRules(ProcessingContext context)
    {
        using var _ = CharactersInstrumentation.StartActivity();

        _logger.LogInformation("processing selection rules for registration '{ElementName} ({ElementType})'",
            context.Registration.AssociatedElementName, context.Registration.AssociatedElementType);

        await ProcessExistingSelectionRules(context);
        await ProcessElementSelectionRules(context);
    }

    private async Task ProcessExistingSelectionRules(ProcessingContext context)
    {
        if (!context.Registration.HasSelectionRules())
        {
            return;
        }

        var registrationRepository = _persistence.GetRepository<IRegistrationRepository>();
        var associatedElement = context.GetAssociatedElement();

        foreach (var existingRule in context.Registration.SelectionRules.ToList())
        {
            var ruleDefinition = associatedElement.SelectionRules.SingleOrDefault(r => r.RuleId == existingRule.AssociatedSelectionRuleId)
                ?? throw new RegistrationProcessingException($"The associated selection rule not found for existing rule '{existingRule.Id}'.");

            // apply progression-aware level gating
            if (ruleDefinition.LevelRequirement > 0) // no lookup needed if no level requirement
            {
                var level = context.GetProgressionLevel(context.Registration);
                if (level < ruleDefinition.LevelRequirement)
                {
                    _logger.LogInformation("removing selection rule '{RuleId}' from registration {ElementName} ({ElementType}) due to level requirement not met (required: {RequiredLevel}, current: {CurrentLevel})",
                        existingRule.Id, context.Registration.AssociatedElementName, context.Registration.AssociatedElementType, ruleDefinition.LevelRequirement, level);

                    // remove the selection rule from the current registration
                    context.Registration.RemoveSelectionRule(existingRule);

                    // check if there is an associated registration for the current selection rule
                    if (existingRule.HasCurrentSelection())
                    {
                        // unregister the selected registration, as the rule is now removed
                        var selectedRegistation = await registrationRepository.GetRegistrationByOriginatingRuleAsync(existingRule.Id);
                        if (selectedRegistation is not null)
                        {
                            _logger.LogInformation("unregistering selected registration {SelectedRegistrationId} for selection rule {RuleId} from registration {RegistrationId}", selectedRegistation.Id, existingRule.AssociatedSelectionRuleId, context.Registration.Id);
                            await _registrationManager.Unregister(selectedRegistation);
                        }
                    }
                }
            }
        }
    }

    private Task ProcessElementSelectionRules(ProcessingContext context)
    {
        var registrationElement = context.GetAssociatedElement();

        foreach (var rule in registrationElement.SelectionRules)
        {
            if (context.Registration.HasAssociatedRule(rule.RuleId))
            {
                // rule applied, skip
                continue;
            }

            if (rule.LevelRequirement > 0)
            {
                var levelProgression = context.GetProgressionLevel(context.Registration);
                if (levelProgression < rule.LevelRequirement)
                {
                    // level requirement not met, skip
                    continue;
                }
            }

            // create the new registration selection rule, this is to keep track of the rules applied
            var newSelectionRule = context.Registration.CreateSelectionRule(new(rule.RuleId), rule.ElementType, rule.Name);

            _logger.LogInformation("created new selection rule '{RuleId}' on registration {ElementName} ({ElementType})",
                newSelectionRule.Id.Value, context.Registration.AssociatedElementName, context.Registration.AssociatedElementType);
        }

        return Task.CompletedTask;
    }

    #endregion

    #region Statistic Rules

    private async Task ProcessStatisticRules(ProcessingContext context)
    {
        using var _ = CharactersInstrumentation.StartActivity();

        _logger.LogInformation("processing statistic rules for registration '{ElementName} ({ElementType})'",
            context.Registration.AssociatedElementName, context.Registration.AssociatedElementType);

        await ProcessExistingStatisticRules(context);
        await ProcessElementStatisticRules(context);
    }

    private Task ProcessExistingStatisticRules(ProcessingContext context)
    {
        if (!context.Registration.HasStatisticRules())
        {
            return Task.CompletedTask;
        }

        var associatedElement = context.GetAssociatedElement();

        foreach (var existingRule in context.Registration.StatisticRules.ToList())
        {
            var ruleDefinition = associatedElement.StatisticRules.SingleOrDefault(r => r.RuleId == existingRule.AssociatedStatisticRuleId)
                ?? throw new RegistrationProcessingException($"The associated statistic rule not found for existing rule '{existingRule.Id}'.");

            // apply progression-aware level gating
            if (ruleDefinition.LevelRequirement > 0)
            {
                var level = context.GetProgressionLevel(context.Registration);
                if (level < ruleDefinition.LevelRequirement)
                {
                    _logger.LogInformation("removing statistic rule '{RuleId}' from registration {ElementName} ({ElementType}) due to level requirement not met (required: {RequiredLevel}, current: {CurrentLevel})",
                        existingRule.Id, context.Registration.AssociatedElementName, context.Registration.AssociatedElementType, ruleDefinition.LevelRequirement, level);

                    // remove the statistic rule from the current registration
                    context.Registration.RemoveStatisticRule(existingRule);
                }
            }
        }

        return Task.CompletedTask;
    }

    private Task ProcessElementStatisticRules(ProcessingContext context)
    {
        var associatedElement = context.GetAssociatedElement();
        var currentRegistration = context.Registration;

        foreach (var rule in associatedElement.StatisticRules)
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

            _logger.LogInformation("created new statistic rule '{RuleId}' on registration {ElementName} ({ElementType}) [{StatisticName}/{StatisticValue}]",
                newStatisticRule.Id.Value, context.Registration.AssociatedElementName, context.Registration.AssociatedElementType,
                newStatisticRule.Name, newStatisticRule.Value);
        }

        return Task.CompletedTask;
    }

    #endregion

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
            ?? throw new RegistrationProcessingException($"The registration with ID '{registrationId}' not found while creating the context.");

        var characterRepository = _persistence.GetRepository<ICharactersRepository>();
        var character = await characterRepository.GetCharacterAsync(registration.CharacterId)
            ?? throw new RegistrationProcessingException($"The character with ID '{registration.CharacterId}' not found for registration '{registration.Id}' while creating the context.");

        var associatedElement = await _elements.GetElementWithRules(registration.AssociatedElementId) ??
            throw new RegistrationProcessingException($"The associated element '{registration.AssociatedElementId}' for the registration '{registration.Id.Value}' was not found while creating the context.");

        // we have the registration, character and associated element, we can now create the context for processing
        var context = new ProcessingContext(registration, character, _persistence);
        context.SetAssociatedElement(associatedElement);

        return context;
    }
}
