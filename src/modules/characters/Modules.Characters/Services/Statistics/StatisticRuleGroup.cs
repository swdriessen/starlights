using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services.Statistics;

/// <summary>
/// Represents a group of related registration statistic rules and their dependencies.
/// </summary>
public sealed record StatisticRuleGroup(string Name, List<RegistrationStatisticRule> Rules, HashSet<string> Dependencies);
