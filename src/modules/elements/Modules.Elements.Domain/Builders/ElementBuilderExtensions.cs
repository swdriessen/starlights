using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Domain.Builders;

public static class ElementBuilderExtensions
{
    public static ElementBuilder WithStatisticRule(this ElementBuilder builder, string name, string value, string? stackingBonus = null, int? levelRequirement = 0)
    {
        return builder.WithComponent(id =>
        {
            var s = new StatisticRuleComponent(id, name, value, 0);

            if (!string.IsNullOrWhiteSpace(stackingBonus))
            {
                s.UpdateStackingBonus(stackingBonus);
            }

            if (levelRequirement.HasValue)
            {
                s.UpdateLevelRequirement(levelRequirement.Value);
            }

            return s;
        });
    }

    public static ElementBuilder WithStatisticRule(this ElementBuilder builder, string name, string value, Action<StatisticRuleComponent>? configure = null)
    {
        return builder.WithComponent(id =>
        {
            var s = new StatisticRuleComponent(id, name, value, 0);
            configure?.Invoke(s);
            return s;
        });
    }

    public static ElementBuilder WithIncludeRule(this ElementBuilder builder, ElementId includeElementId, int levelRequirement = 0)
    {
        return builder.WithComponent(id =>
        {
            var s = new IncludeRuleComponent(id, includeElementId, levelRequirement);
            return s;
        });
    }

    public static ElementBuilder WithShortDescription(this ElementBuilder builder, string description)
    {
        return builder.WithComponent(id =>
        {
            var component = new ShortDescriptionComponent(id, description);
            return component;
        });
    }

    public static ElementBuilder WithAbbreviationComponent(this ElementBuilder builder, string abbreviation)
    {
        return builder.WithComponent(id =>
        {
            var component = new AbbreviationComponent(id, abbreviation);
            return component;
        });
    }

    public static ElementBuilder WithSorting(this ElementBuilder builder, double sortingOrder)
    {
        return builder.WithComponent(id =>
        {
            var component = new SortingComponent(id, sortingOrder);
            return component;
        });
    }
}