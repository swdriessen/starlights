using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Domain.Builders;

public static class ElementBuilderExtensions
{
    public static ElementBuilder WithStatisticRule(this ElementBuilder builder, string name, string value, string? stackingBonus = null, int? levelRequirement = 0)
    {
        return builder.WithComponent(id =>
        {
            var component = new StatisticRuleComponent(id, name, value, 0);

            if (!string.IsNullOrWhiteSpace(stackingBonus))
            {
                component.UpdateStackingBonus(stackingBonus);
            }

            if (levelRequirement.HasValue)
            {
                component.UpdateLevelRequirement(levelRequirement.Value);
            }

            return component;
        });
    }

    public static ElementBuilder WithStatisticRule(this ElementBuilder builder, string name, string value, Action<StatisticRuleComponent>? configure = null)
    {
        return builder.WithComponent(id =>
        {
            var component = new StatisticRuleComponent(id, name, value, 0);
            configure?.Invoke(component);
            return component;
        });
    }

    public static ElementBuilder WithIncludeRule(this ElementBuilder builder, ElementId includeElementId, int levelRequirement = 0)
    {
        return builder.WithComponent(id => new IncludeRuleComponent(id, includeElementId, levelRequirement));
    }

    public static ElementBuilder WithSelectionRule(this ElementBuilder builder, string elementType, string name, int levelRequirement = 0, Action<SelectionRuleComponent>? configure = null)
    {
        return builder.WithComponent(id =>
        {
            var component = new SelectionRuleComponent(id, elementType, name, levelRequirement);
            configure?.Invoke(component);
            return component;
        });
    }

    public static ElementBuilder WithDescription(this ElementBuilder builder, string description)
    {
        return builder.WithComponent(id => new DescriptionComponent(id, description));
    }

    public static ElementBuilder WithShortDescription(this ElementBuilder builder, string description)
    {
        return builder.WithComponent(id => new ShortDescriptionComponent(id, description));
    }

    public static ElementBuilder WithAbbreviationComponent(this ElementBuilder builder, string abbreviation)
    {
        return builder.WithComponent(id => new AbbreviationComponent(id, abbreviation));
    }

    public static ElementBuilder WithSorting(this ElementBuilder builder, double sortingOrder)
    {
        return builder.WithComponent(id => new SortingComponent(id, sortingOrder));
    }
}