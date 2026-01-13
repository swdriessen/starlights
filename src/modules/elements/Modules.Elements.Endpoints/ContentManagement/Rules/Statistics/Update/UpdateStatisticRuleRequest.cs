namespace Starlights.Modules.Elements.Endpoints.Content.Rules.Statistics.Update;

public sealed record UpdateStatisticRuleRequest
{
    /// <summary>
    /// Gets the name of the statistic.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the value of the statistic.
    /// </summary>
    public required string Value { get; init; }

    /// <summary>
    /// Gets the stacking bonus name, if any.
    /// </summary>
    public string? StackingBonus { get; init; }

    /// <summary>
    /// Gets the level requirement.
    /// </summary>
    public int LevelRequirement { get; init; }

    /// <summary>
    /// Gets the display name of the statistic, if any.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Gets the minimum allowed value for this statistic rule, if any.
    /// </summary>
    public int? Minimum { get; init; }

    /// <summary>
    /// Gets the maximum allowed value for this statistic rule, if any.
    /// </summary>
    public int? Maximum { get; init; }

    /// <summary>
    /// Gets the requirements expression for this statistic rule, if any.
    /// </summary>
    public string? RequirementsExpression { get; init; }
}
