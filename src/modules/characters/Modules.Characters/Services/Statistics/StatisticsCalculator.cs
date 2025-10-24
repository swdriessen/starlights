using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Starlights.Modules.Characters.Domain;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services.Statistics;

public class StatisticsCalculator
{
    private readonly ILogger<StatisticsCalculator> _logger;
    private readonly IEnumerable<IStatisticsCalculationInitializer> _initializers;
    private readonly IEnumerable<IStatisticGroupProcessor> _groupProcessors;
    private readonly IEnumerable<IStatisticsPostProcessor> _postProcessors;

    public StatisticsCalculator(
        ILogger<StatisticsCalculator> logger,
        IEnumerable<IStatisticsCalculationInitializer> initializers,
        IEnumerable<IStatisticGroupProcessor> groupProcessors,
        IEnumerable<IStatisticsPostProcessor> postProcessors)
    {
        _logger = logger;
        _initializers = initializers;
        _groupProcessors = groupProcessors;
        _postProcessors = postProcessors;
    }

    public StatisticsCalculationResult Calculate(Character character, List<Registration> registrations)
    {
        using var activity = CharactersInstrumentation.StartActivity("Statistics Calculator", a => a.AddTag("registrations", registrations.Count));
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
        var pendingGroups = GetPendingGroups([.. characterStatisticRules.Except(valueRules)]);

        // process all value rules that only set direct numeric values
        foreach (var rule in valueRules)
        {
            var originalName = rule.FriendlyName ?? rule.Name;
            var originalValue = rule.GetValue();

            context.Statistics.WithGroup(rule.Name, group =>
            {
                var determinedName = originalName;
                var determinedValue = EnforceRuleConstraints(originalValue, rule);

                // resolve parent name if available
                if (context.RegistrationLookup.TryGetValue(rule.ParentRegistrationId, out var parentName))
                {
                    determinedName = parentName;
                }

                // add the value to the statistic group
                group.WithValue(determinedValue, determinedName);

                // if this group does not exist in pending groups, mark it as complete, this helps with dependency resolution later
                // also check that no other pending rules exist for this group in valueRules (in case of multiple registrations adding direct values to same group)
                if (!pendingGroups.ContainsKey(rule.Name) && !valueRules.Except([rule]).Any(x => x.Name == rule.Name))
                {
                    group.Complete();
                }
            });
        }

        // process special group processors first, other statistic groups may depend on these
        foreach (var groupProcessor in _groupProcessors)
        {
            groupProcessor.Process(pendingGroups, context, ProcessGroupNode);
        }

        // detect circular dependencies before processing
        var detectedCircularDependencies = DetectCircularDependencies(pendingGroups, context);
        if (detectedCircularDependencies.Count > 0)
        {
            var circularDependencyPath = string.Join(" -> ", detectedCircularDependencies);

            context.AddError($"A circular dependency detected in statistic rules: {circularDependencyPath}");
            _logger.LogError("[StatisticsCalculator] Circular dependency detected in statistic rules: {CircularDependencyPath}", circularDependencyPath);
        }

        foreach (var (key, pendingGroup) in pendingGroups)
        {
            var result = ProcessGroupNode(pendingGroup, context);
            if (result is StatisticValuesGroup group && group.IsCompleted)
            {
                pendingGroups.Remove(group.GroupName);
            }
            else
            {
                // add warning to context
                _logger.LogError("[StatisticsCalculator] Statistic group '{GroupName}' could not be finalized due to unresolved dependencies.", key);
            }
        }

        EvaluateIncompleteGroups(context);

        sw.Stop();

        activity?.AddTag("statisticCount", context.Statistics.Count);
        activity?.AddTag("errors", context.Errors.Count);
        activity?.AddTag("elapsedMilliseconds", sw.ElapsedMilliseconds);

        _logger.LogInformation("[StatisticsCalculator] Calculated statistics (count:{StatisticCount}) for Character '{CharacterName}' in {ElapsedMilliseconds}ms", context.Statistics.Count, character.Name, sw.ElapsedMilliseconds);

        foreach (var postProcessor in _postProcessors)
        {
            postProcessor.Process(context);
        }

        return new StatisticsCalculationResult(context.Statistics, context.Errors);
    }

    /// <summary>
    /// Groups the specified pending rules by name and creates a dictionary of rule group nodes, each containing the
    /// group's dependencies. The returned groups are ordered by dependencies using topological sort, with groups
    /// having no dependencies appearing first.
    /// </summary>
    /// <param name="pendingRules">The list of registration statistic rules to be grouped and analyzed for dependencies. Cannot be null.</param>
    /// <returns>A dictionary mapping each group name to a corresponding rule group node that includes the group's rules and
    /// their dependencies, ordered so that groups with no dependencies appear first.</returns>
    private static Dictionary<string, StatisticRuleGroup> GetPendingGroups(List<RegistrationStatisticRule> pendingRules)
    {
        var pendingGroups = new Dictionary<string, StatisticRuleGroup>();

        foreach (var group in pendingRules.GroupBy(r => r.Name))
        {
            var dependencies = new HashSet<string>();

            foreach (var rule in group.Where(rule => rule.HasReferenceValue()))
            {
                dependencies.Add(rule.Value);
            }

            pendingGroups[group.Key] = new StatisticRuleGroup(group.Key, [.. group], dependencies);
        }

        // Perform topological sort to order groups by dependencies
        return TopologicalSort(pendingGroups);

        static Dictionary<string, StatisticRuleGroup> TopologicalSort(Dictionary<string, StatisticRuleGroup> groups)
        {
            var result = new Dictionary<string, StatisticRuleGroup>();
            var inDegree = new Dictionary<string, int>();
            var queue = new Queue<string>();

            // Calculate in-degree (number of dependencies) for each group
            foreach (var (groupName, group) in groups)
            {
                inDegree[groupName] = group.Dependencies.Count(dep => groups.ContainsKey(dep));
            }

            // Start with groups that have no dependencies
            foreach (var (groupName, degree) in inDegree.Where(kvp => kvp.Value == 0))
            {
                queue.Enqueue(groupName);
            }

            // Process groups in dependency order
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result[current] = groups[current];

                // Find groups that depend on the current group and reduce their in-degree
                foreach (var (groupName, group) in groups)
                {
                    if (group.Dependencies.Contains(current))
                    {
                        inDegree[groupName]--;
                        if (inDegree[groupName] == 0)
                        {
                            queue.Enqueue(groupName);
                        }
                    }
                }
            }

            // Add remaining groups (those with circular dependencies or unresolved dependencies)
            foreach (var (groupName, group) in groups.Where(kvp => !result.ContainsKey(kvp.Key)))
            {
                result[groupName] = group;
            }

            return result;
        }
    }

    /// <summary>
    /// Processes a statistic rule group and calculates its aggregated values based on the group's rules and dependencies.
    /// </summary>
    /// <remarks>If the group has unresolved dependencies, processing is skipped and an error is logged. Rules
    /// with stacking bonuses are processed by selecting the rule with the highest value within each bonus group.
    /// Constraints such as minimum and maximum values are applied to each rule as appropriate.</remarks>
    private StatisticValuesGroup? ProcessGroupNode(StatisticRuleGroup group, StatisticsProcessorContext context)
    {
        group.Dependencies
            .RemoveWhere(d => context.Statistics.ContainsGroup(d) && context.Statistics.GetGroup(d).IsCompleted);

        // handle the group, taking into account stacking bonuses, etc
        var hasDependencies = group.Dependencies.Count > 0;
        if (hasDependencies)
        {
            _logger.LogError("[StatisticsCalculator] Statistic rules for '{GroupName}' have unresolved dependencies: {Dependencies}", group.Name, string.Join(", ", group.Dependencies));

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
            var determinedValue = rule.HasReferenceValue() ? context.Statistics.GetValue(rule.Value) : rule.GetValue();
            determinedValue = EnforceRuleConstraints(determinedValue, rule);

            var determinedName = rule.FriendlyName ?? rule.Name;

            // resolve parent name if available
            if (context.RegistrationLookup.TryGetValue(rule.ParentRegistrationId, out var parentName))
            {
                determinedName = parentName;
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
                determinedValue = EnforceRuleConstraints(determinedValue, rule);

                if (determinedValue > highestValue)
                {
                    highestValue = determinedValue;
                    highestRule = rule;
                }
            }

            // add highest value to the statistics group
            if (highestRule != null)
            {
                var determinedName = highestRule.FriendlyName ?? highestRule.Name;

                // resolve parent name if available
                if (context.RegistrationLookup.TryGetValue(highestRule.ParentRegistrationId, out var parentName))
                {
                    determinedName = parentName;
                }

                statisticsGroup.AddValue(determinedName, highestValue, determinedName, highestRule.AssociatedStatisticRuleId.Value);

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
    /// Adjusts the specified value to ensure it falls within the minimum and maximum constraints defined by the
    /// provided rule.
    /// </summary>
    private static int EnforceRuleConstraints(int originalValue, RegistrationStatisticRule rule)
    {
        if (rule.MaximumValue.HasValue && rule.MaximumValue > 0 && originalValue > rule.MaximumValue)
        {
            return rule.MaximumValue.Value;
        }

        if (rule.MinimumValue.HasValue && originalValue < rule.MinimumValue)
        {
            return rule.MinimumValue.Value;
        }

        return originalValue;
    }

    /// <summary>
    /// Evaluates statistic groups in the specified context and completes any groups that are incomplete and have no
    /// values.
    /// </summary>
    /// <remarks>If an incomplete group contains values, a warning is logged with details about the group.
    /// Groups with no values are automatically marked as complete.</remarks>
    private void EvaluateIncompleteGroups(StatisticsProcessorContext context)
    {
        foreach (var group in context.Statistics.Where(s => !s.IsCompleted))
        {
            if (group.HasStatisticValues())
            {
                // incomplete but has values, log warning to indicate potential issue 
                _logger.LogError("[StatisticsCalculator] incompleted statistic group '{GroupName}' with total value {TotalValue} and values: {Summary}", group.GroupName, group.Sum(), group.GetSummary());
            }
            else
            {
                group.Complete(); // no values added, just complete it
            }
        }
    }

    #region Circular Dependency Detection

    /// <summary>
    /// Detects circular dependencies in the pending groups using depth-first search.
    /// </summary>
    /// <returns>A list of statistic names involved in circular dependencies, or empty if none found.</returns>
    private static List<string> DetectCircularDependencies(Dictionary<string, StatisticRuleGroup> pendingGroups, StatisticsProcessorContext context)
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
    private static bool HasCircularDependency(string currentNode, Dictionary<string, StatisticRuleGroup> pendingGroups, StatisticsProcessorContext context, HashSet<string> visited, HashSet<string> recursionStack, out List<string> cycle)
    {
        cycle = [];
        visited.Add(currentNode);
        recursionStack.Add(currentNode);

        if (pendingGroups.TryGetValue(currentNode, out var node))
        {
            foreach (var dependency in node.Dependencies)
            {
                // Skip dependencies that are already finalized in context
                if (context.Statistics.ContainsGroup(dependency) && context.Statistics.GetGroup(dependency).IsCompleted)
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
    private static List<string> BuildCyclePath(string startNode, string endNode, HashSet<string> recursionStack, Dictionary<string, StatisticRuleGroup> pendingGroups)
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
}
