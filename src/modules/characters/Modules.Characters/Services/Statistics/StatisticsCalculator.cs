using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services.Statistics;

public class StatisticsCalculator
{
    public const int MaxReferenceResolutionIterations = 10;
    private readonly ILogger<StatisticsCalculator> _logger;
    private readonly IEnumerable<IStatisticsCalculationInitializer> _initializers;
    private readonly IEnumerable<IStatisticsPostProcessor> _postProcessors;

    public StatisticsCalculator(ILogger<StatisticsCalculator> logger,
        IEnumerable<IStatisticsCalculationInitializer> initializers,
        IEnumerable<IStatisticsPostProcessor> postProcessors)
    {
        _logger = logger;
        _initializers = initializers;
        _postProcessors = postProcessors;
    }

    public StatisticValuesGroupCollection Calculate(Character character, List<Registration> registrations)
    {
        var sw = Stopwatch.StartNew();
        var context = new StatisticsProcessorContext(character, registrations);

        // get all statistic rules from registrations
        var characterStatisticRules = registrations.SelectMany(r => r.StatisticRules).ToList();

        // seed the character level statistic for use in other calculations
        foreach (var processor in _initializers)
        {
            processor.Initialize(context);
        }

        // split into direct value rules (numeric value) and reference rules (reference to another statistic by name)        
        var valueRules = characterStatisticRules.Where(r => !r.HasReferenceValue() && !r.HasStackingBonus()).ToList();
        var pendingGroups = GetPendingGroups([.. characterStatisticRules.Except(valueRules)], context);

        // process all value rules that only set direct numeric values
        foreach (var rule in valueRules)
        {
            var originalName = rule.Name; // TODO: rule can have optional display name to override this and prevent using parent element name
            var originalValue = rule.GetValue();

            context.Statistics.WithGroup(rule.Name, group =>
            {
                var determinedName = originalName;
                var determinedValue = originalValue;

                // resolve parent name if available
                if (context.RegistrationLookup.TryGetValue(rule.ParentRegistrationId, out var parentName))
                {
                    determinedName = parentName;
                }

                // apply max constraint if applicable
                if (rule.MaximumValue.HasValue && rule.MaximumValue > 0 && originalValue > rule.MaximumValue)
                {
                    // cap the value
                    determinedValue = rule.MaximumValue.Value;
                }

                // apply min constraint if applicable
                if (rule.MinimumValue.HasValue && rule.MinimumValue > 0 && originalValue < rule.MinimumValue)
                {
                    // raise to min value
                    determinedValue = rule.MinimumValue.Value;
                }

                // add the value to the statistic group
                group.WithValue(determinedValue, determinedName);

                // if this group does not exist in pending groups, mark it as complete, this helps with dependency resolution later
                if (!pendingGroups.ContainsKey(rule.Name))
                {
                    group.Complete();
                }
            });
        }

        // first process proficiency and ability rules as those are needed for other calculations
        if (pendingGroups.TryGetValue("proficiency", out var proficiencyNode))
        {
            var result = ProcessGroupNode(proficiencyNode, context);
            if (result is StatisticValuesGroup group && group.IsFinalized)
            {
                pendingGroups.Remove(group.GroupName);
            }
        }

        context.Statistics.WithGroupVariants("proficiency");

        // make sure pendingRules do not contain more proficiency related rules as those are now processed
        if (pendingGroups.Any(r => r.Key.StartsWith("proficiency")))
        {
            _logger.LogError("[StatisticsCalculator] Pending statistic rules contain proficiency related rules that should have been processed already.");
        }

        character.UpdateComponent<AbilitiesComponent>((a, _) =>
        {
            foreach (var score in a.AbilityScores)
            {
                var key = score.Name.ToLowerInvariant();
                var group = context.Statistics.GetGroup(key);
                score.UpdateAdditionalScore(group.Sum());
                // TODO: max score

                context.Statistics.WithGroup($"{key}:score", g =>
                {
                    g.WithValue(score.CalculatedScore, $"{score.Name}");
                    g.Complete();
                });
                context.Statistics.WithGroup($"{key}:modifier", g =>
                {
                    g.WithValue(score.CalculatedModifier, $"{score.Name} Modifier");
                    g.Complete();
                });

                context.Statistics.WithGroupVariants($"{key}:score", score.Name);
                context.Statistics.WithGroupVariants($"{key}:modifier", score.Name);

                // make sure pendingRules do not container more ability score related rules as those are now processed
                // throw exception for now if they do
                if (pendingGroups.Any(r => r.Key.StartsWith(key)))
                {
                    throw new InvalidOperationException($"Pending statistic rules contain ability score related rules that should have been processed already");
                }
            }
        });


        // Detect circular dependencies before processing
        var circularDeps = DetectCircularDependencies(pendingGroups, context);
        if (circularDeps.Count > 0)
        {
            var cyclePath = string.Join(" -> ", circularDeps);
            _logger.LogError("[StatisticsCalculator] Circular dependency detected in statistic rules: {cyclePath}", cyclePath);
            context.AddError($"A circular dependency detected in statistic rules: {cyclePath}");
            //throw new StatisticsCalculatorException($"Circular dependency detected in statistic rules: {cyclePath}");
        }

        foreach (var (key, pendingGroup) in pendingGroups)
        {
            var result = ProcessGroupNode(pendingGroup, context);
            if (result is StatisticValuesGroup group && group.IsFinalized)
            {
                pendingGroups.Remove(group.GroupName);
            }
            else
            {
                // add warning to context
                _logger.LogWarning("[StatisticsCalculator] Statistic group '{GroupName}' could not be finalized due to unresolved dependencies.", key);
            }
        }


        // process all other pending rules (references, stacking bonuses, etc.)
        //var unprocessedRules = ProcessPendingRules(pendingRules, context);
        //if (unprocessedRules.Count > 0)
        //{
        //    Trace.WriteLine($"[StatisticsCalculator] Warning: {unprocessedRules.Count} rules could not be processed due to unresolved dependencies.");
        //    foreach (var item in unprocessedRules)
        //    {
        //        Trace.WriteLine($" - Unprocessed Rule: Name={item.Name}, Value={item.Value}, StackingBonus={item.StackingBonus}");
        //    }
        //}



        //var (simpleRulesOld, _) = SeparateStatisticRules(characterStatisticRules);


        // ApplyStackingBonuses(simpleRulesOld, context);
        // ApplyMinMaxConstraints(characterStatisticRules, context);

        //foreach (var processor in _postProcessors)
        //{
        //    processor.Process(context);
        //}
        FinalizeStatistics(context);


        sw.Stop();

        Trace.WriteLine($"[StatisticsCalculator] Calculated statistics (count:{context.Statistics.Count}) for CharacterId={character.Name} in {sw.ElapsedMilliseconds}ms");




        return context.Statistics;
    }

    #region Used 

    private StatisticValuesGroup? ProcessGroupNode(RuleGroupNode group, StatisticsProcessorContext context)
    {
        group.Dependencies
            .RemoveWhere(d => context.Statistics.ContainsGroup(d) && context.Statistics.GetGroup(d).IsFinalized);

        // handle the group, taking into account stacking bonuses, etc
        var hasDependencies = group.Dependencies.Count > 0;
        if (hasDependencies)
        {
            _logger.LogError($"[StatisticsCalculator] Statistic rules for '{group.Name}' have unresolved dependencies: {string.Join(", ", group.Dependencies)}");

            // skip processing for now
            // add error to context
            // maybe return int.MinValue for now to indicate error but not break the calculation flow
            return null;
        }

        var statisticsGroup = context.Statistics.GetGroup(group.Name);


        // process rules without stacking bonus first
        var pendingRules = new List<RegistrationStatisticRule>(group.Rules);

        var rulesWithoutBonus = pendingRules.Where(r => !r.HasStackingBonus()).ToList();
        var rulesWithBonus = pendingRules.Except(rulesWithoutBonus).ToList();

        // process rules without stacking bonus
        foreach (var rule in rulesWithoutBonus)
        {
            var determinedName = rule.Name;
            var determinedValue = rule.HasReferenceValue() ? context.Statistics.GetValue(rule.Value) : rule.GetValue();

            // resolve parent name if available
            if (context.RegistrationLookup.TryGetValue(rule.ParentRegistrationId, out var parentName))
            {
                determinedName = parentName;
            }

            // apply max constraint if applicable
            if (rule.MaximumValue.HasValue && rule.MaximumValue > 0 && determinedValue > rule.MaximumValue)
            {
                determinedValue = rule.MaximumValue.Value;
            }

            // apply min constraint if applicable
            if (rule.MinimumValue.HasValue && rule.MinimumValue > 0 && determinedValue < rule.MinimumValue)
            {
                determinedValue = rule.MinimumValue.Value;
            }

            statisticsGroup.AddValue(determinedName, determinedValue, determinedName, rule.AssociatedStatisticRuleId.Value);

            pendingRules.Remove(rule);

            if (pendingRules.Count == 0)
            {
                statisticsGroup.Complete();
            }
        }

        // process rules with stacking bonus
        var bonusGroups = rulesWithBonus.GroupBy(r => r.StackingBonus);

        foreach (var bonusGroup in bonusGroups)
        {
            // find highest value in the group
            var highestValue = int.MinValue;
            RegistrationStatisticRule? highestRule = null;

            foreach (var rule in bonusGroup)
            {
                var determinedValue = rule.HasReferenceValue() ? context.Statistics.GetValue(rule.Value) : rule.GetValue();

                // apply max constraint if applicable
                if (rule.MaximumValue.HasValue && rule.MaximumValue > 0 && determinedValue > rule.MaximumValue)
                {
                    determinedValue = rule.MaximumValue.Value;
                }

                // apply min constraint if applicable
                if (rule.MinimumValue.HasValue && rule.MinimumValue > 0 && determinedValue < rule.MinimumValue)
                {
                    determinedValue = rule.MinimumValue.Value;
                }

                if (determinedValue > highestValue)
                {
                    highestValue = determinedValue;
                    highestRule = rule;
                }
            }

            // add highest value to the statistics group
            if (highestRule != null)
            {
                var determinedName = highestRule.Name;

                // resolve parent name if available
                if (context.RegistrationLookup.TryGetValue(highestRule.ParentRegistrationId, out var parentName))
                {
                    determinedName = parentName;
                }

                statisticsGroup.AddValue(determinedName, highestValue, $"{determinedName} ({bonusGroup.Key})", highestRule.AssociatedStatisticRuleId.Value);

                pendingRules.Remove(highestRule);

                foreach (var item in bonusGroup) // rules in the bonus group that were not highest, still remove them from pending
                {
                    pendingRules.Remove(item);
                }

                if (pendingRules.Count == 0)
                {
                    statisticsGroup.Complete();
                }
            }

        }



        return statisticsGroup;
    }


    /// <summary>
    /// Builds a dependency graph for pending rules, identifying which statistics reference others.
    /// </summary>
    private Dictionary<string, RuleGroupNode> GetPendingGroups(List<RegistrationStatisticRule> pendingRules, StatisticsProcessorContext context)
    {
        var pendingGroups = new Dictionary<string, RuleGroupNode>();
        var ruleGroups = pendingRules.GroupBy(r => r.Name);

        foreach (var group in ruleGroups)
        {
            var dependencies = new HashSet<string>();

            foreach (var rule in group)
            {
                if (rule.HasReferenceValue())
                {
                    dependencies.Add(rule.Value);
                }
            }

            pendingGroups[group.Key] = new RuleGroupNode(group.Key, [.. group], dependencies);
        }

        return pendingGroups;
    }

    #endregion

    private void FinalizeStatistics(StatisticsProcessorContext context)
    {
        foreach (var group in context.Statistics)
        {
            if (group.IsFinalized)
            {
                continue;
            }

            if (group.GetValues().Count == 0)
            {
                group.Complete();
                continue;
            }

            _logger.LogWarning($"[StatisticsCalculator] Finalizing statistic group '{group.GroupName}' with total value {group.Sum()} and values: {group.GetSummary()}");
            group.Complete();
        }
    }

    #region Circular Dependencies

    /// <summary>
    /// Detects circular dependencies in the pending groups using depth-first search.
    /// </summary>
    /// <returns>A list of statistic names involved in circular dependencies, or empty if none found.</returns>
    private static List<string> DetectCircularDependencies(Dictionary<string, RuleGroupNode> pendingGroups, StatisticsProcessorContext context)
    {
        var visited = new HashSet<string>();
        var recursionStack = new HashSet<string>();
        var circularDependencies = new List<string>();

        foreach (var groupName in pendingGroups.Keys)
        {
            if (!visited.Contains(groupName))
            {
                if (HasCircularDependency(groupName, pendingGroups, context, visited, recursionStack, out var cycle))
                {
                    circularDependencies.AddRange(cycle);
                }
            }
        }

        return circularDependencies.Distinct().ToList();
    }

    /// <summary>
    /// Recursively checks for circular dependencies starting from a given node.
    /// </summary>
    private static bool HasCircularDependency(
        string currentNode,
        Dictionary<string, RuleGroupNode> pendingGroups,
        StatisticsProcessorContext context,
        HashSet<string> visited,
        HashSet<string> recursionStack,
        out List<string> cycle)
    {
        cycle = [];
        visited.Add(currentNode);
        recursionStack.Add(currentNode);

        if (pendingGroups.TryGetValue(currentNode, out var node))
        {
            foreach (var dependency in node.Dependencies)
            {
                // Skip dependencies that are already finalized in context
                if (context.Statistics.ContainsGroup(dependency) && context.Statistics.GetGroup(dependency).IsFinalized)
                {
                    continue;
                }

                // If dependency is in recursion stack, we found a cycle
                if (recursionStack.Contains(dependency))
                {
                    cycle = BuildCyclePath(dependency, currentNode, recursionStack, pendingGroups);
                    return true;
                }

                // If not visited, recursively check
                if (!visited.Contains(dependency))
                {
                    if (HasCircularDependency(dependency, pendingGroups, context, visited, recursionStack, out cycle))
                    {
                        return true;
                    }
                }
            }
        }

        recursionStack.Remove(currentNode);
        return false;
    }

    /// <summary>
    /// Builds a readable path of the circular dependency chain.
    /// </summary>
    private static List<string> BuildCyclePath(string startNode, string endNode, HashSet<string> recursionStack, Dictionary<string, RuleGroupNode> pendingGroups)
    {
        var path = new List<string> { endNode };
        var current = endNode;

        // Trace back through dependencies to find the cycle
        while (current != startNode && pendingGroups.TryGetValue(current, out var node))
        {
            var nextInCycle = node.Dependencies.FirstOrDefault(d => recursionStack.Contains(d));
            if (nextInCycle != null)
            {
                path.Add(nextInCycle);
                current = nextInCycle;
            }
            else
            {
                break;
            }
        }

        path.Reverse();
        return path;
    }

    #endregion

    /// <summary>
    /// Represents a group of rules with their dependencies.
    /// </summary>
    private sealed record RuleGroupNode(string Name, List<RegistrationStatisticRule> Rules, HashSet<string> Dependencies)
    {
        public bool HasDependencies => Dependencies.Count > 0;
    }
}

