using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;
using Starlights.Modules.Elements.Integration.Models;

namespace Starlights.Modules.Elements.Data;

public static class DataModelMappingExtensions
{
    public static ElementDataModel AsElementDataModel(this Element element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        return new ElementDataModel
        {
            Id = element.Id,
            Name = element.Name,
            Type = element.Type,
            Source = "Internal",
            IncludeRules = [.. element.GetComponents<IncludeRuleComponent>().Select(rule => rule.AsIncludeRuleDataModel())]
        };
    }

    public static IncludeRuleDataModel AsIncludeRuleDataModel(this IncludeRuleComponent rule)
    {
        ArgumentNullException.ThrowIfNull(rule, nameof(rule));
        return new IncludeRuleDataModel(rule.Id, rule.IncludeElement, rule.LevelRequirement);
    }

    public static AbilityDataModel AsAbilityDataModel(this Element element)
    {
        ArgumentNullException.ThrowIfNull(element, nameof(element));
        return new AbilityDataModel(element.Id, element.Name, element.GetComponent<AbbreviationComponent>().Abbreviation);
    }
}