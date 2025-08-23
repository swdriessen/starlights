using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Integration.Models;
using Starlights.Modules.Elements.Integration.Models.Rules;

namespace Starlights.Modules.Elements.Data;

public static class DataModelMappingExtensions
{
    public static CharacterCreationDataModel AsCharacterCreationDataModel(this Element element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));

        var description = element.Components
            .OfType<ShortDescriptionComponent>()
            .SingleOrDefault();

        return new CharacterCreationDataModel(element.Id, element.Name, element.Type)
        {
            ShortDescription = description?.Content
        };
    }

    public static ElementDataModel AsElementDataModel(this Element element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        return new ElementDataModel
        {
            Id = element.Id,
            Name = element.Name,
            Type = element.Type,
            Source = "Internal",
            IncludeRules = [.. element.GetComponents<IncludeRuleComponent>().Select(rule => rule.AsIncludeRuleDataModel())],
            StatisticRules = [.. element.GetComponents<StatisticRuleComponent>().Select(rule => rule.AsStatisticRuleDataModel())],
            SelectionRules = [.. element.GetComponents<SelectionRuleComponent>().Select(rule => rule.AsSelectionRuleDataModel())]
        };
    }

    public static IncludeRuleDataModel AsIncludeRuleDataModel(this IncludeRuleComponent rule)
    {
        ArgumentNullException.ThrowIfNull(rule, nameof(rule));
        return new IncludeRuleDataModel(rule.Id, rule.IncludeElement, rule.LevelRequirement);
    }

    public static StatisticRuleDataModel AsStatisticRuleDataModel(this StatisticRuleComponent rule)
    {
        ArgumentNullException.ThrowIfNull(rule, nameof(rule));
        return new StatisticRuleDataModel(rule.Id, rule.Name, rule.Value, rule.StackingBonus, rule.LevelRequirement);
    }

    public static SelectionRuleDataModel AsSelectionRuleDataModel(this SelectionRuleComponent rule)
    {
        ArgumentNullException.ThrowIfNull(rule, nameof(rule));
        return new SelectionRuleDataModel(rule.Id, rule.ElementType, rule.Name, rule.LevelRequirement);
    }

    public static AbilityDataModel AsAbilityDataModel(this Element element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        return new AbilityDataModel(element.Id, element.Name, element.GetComponent<AbbreviationComponent>().Abbreviation);
    }

    public static SkillDataModel AsSkillDataModel(this Element element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        return new SkillDataModel(element.Id, element.Name, element.GetComponent<PrimaryAbilityComponent>().PrimaryAbility);
    }

    public static SavingThrowDataModel AsSavingThrowDataModel(this Element element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        return new SavingThrowDataModel(element.Id, element.Name, element.GetComponent<PrimaryAbilityComponent>().PrimaryAbility);
    }
}