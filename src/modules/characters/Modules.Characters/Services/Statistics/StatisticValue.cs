namespace Starlights.Modules.Characters.Services.Statistics;

/// <summary>
/// Represents a single statistic value contribution with its source information.
/// </summary>
/// <param name="Source">The identifier of the source (e.g., rule ID or calculated source)</param>
/// <param name="Value">The numeric value contributed</param>
/// <param name="DisplayName">Optional display name for the source</param>
/// <param name="RuleId">Optional ID of the originating registration statistic rule</param>
public record StatisticValue(string Source, int Value, string? DisplayName = null, Guid? RuleId = null);
