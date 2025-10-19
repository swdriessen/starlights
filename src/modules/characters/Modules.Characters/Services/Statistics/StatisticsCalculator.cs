using System.Diagnostics;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services.Statistics;



public class StatisticsCalculator
{
    public const int MaxReferenceResolutionIterations = 10;

    private readonly IEnumerable<IStatisticsCalculationInitializer> _initializers;
    private readonly IEnumerable<IStatisticsPostProcessor> _postProcessors;

    public StatisticsCalculator(
        IEnumerable<IStatisticsCalculationInitializer> initializers,
        IEnumerable<IStatisticsPostProcessor> postProcessors)
    {
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
        var (simpleRules, pendingRules) = SeparateStatisticRules(characterStatisticRules);

        // process all value rules that only set direct numeric values
        ProcessDirectValueRules(simpleRules, context);

        // process all pending rules, including reference value rules, stacking bonus rules, min/max constraints, etc.
        var unprocessedRules = ProcessPendingRules(pendingRules, context);
        if (unprocessedRules.Count > 0)
        {
            Trace.WriteLine($"[StatisticsCalculator] Warning: {unprocessedRules.Count} rules could not be processed due to unresolved dependencies.");
            foreach (var item in unprocessedRules)
            {
                Trace.WriteLine($" - Unprocessed Rule: Name={item.Name}, Value={item.Value}, StackingBonus={item.StackingBonus}");
            }
        }

        ApplyStackingBonuses(simpleRules, context);
        ApplyMinMaxConstraints(characterStatisticRules, context);

        foreach (var processor in _postProcessors)
        {
            processor.Process(context);
        }
        FinalizeStatistics(context);


        sw.Stop();

        Trace.WriteLine($"[StatisticsCalculator] Calculated statistics (count:{context.Statistics.Count}) for CharacterId={character.Name} in {sw.ElapsedMilliseconds}ms");



        return context.Statistics;
    }

    private static int GetCharacterLevel(Character character)
    {
        var progressionComponent = character.GetRequiredComponent<ProgressionComponent>();
        return progressionComponent.CharacterLevel;
    }


    private static (List<(RegistrationStatisticRule Rule, int Value)> DirectValueRules, List<RegistrationStatisticRule> ReferenceValueRules)
        SeparateStatisticRules(List<RegistrationStatisticRule> validStatistics)
    {
        var directValueRules = new List<(RegistrationStatisticRule Rule, int Value)>();
        var pendingRules = new List<RegistrationStatisticRule>();

        foreach (var stat in validStatistics)
        {
            if (stat.IsNumberValue() && stat.StackingBonus is null)
            {
                var value = stat.GetValue();
                if (stat.Value.StartsWith("-"))
                {
                    value = -value;
                }
                directValueRules.Add((stat, value));
            }
            else
            {
                pendingRules.Add(stat);
            }
        }

        return (directValueRules, pendingRules);
    }

    private void ProcessDirectValueRules(List<(RegistrationStatisticRule Rule, int Value)> directValueRules, StatisticsProcessorContext context)
    {
        var nonStackingRules = directValueRules.Where(r => string.IsNullOrWhiteSpace(r.Rule.StackingBonus));

        foreach (var (rule, value) in nonStackingRules)
        {
            var group = context.Statistics.GetGroup(rule.Name);




            var source = rule.AssociatedStatisticRuleId.Value.ToString();
            var displayName = context.RegistrationLookup.GetValueOrDefault(rule.ParentRegistrationId) ?? rule.Name;

            group.AddValue(source, value, displayName, rule.AssociatedStatisticRuleId.Value);
        }
    }

    /// <summary>
    /// Processes pending rules that couldn't be handled in the initial pass.
    /// Handles reference values, stacking bonuses, and rules with dependencies on other statistics.
    /// </summary>
    /// <returns>List of rules that still couldn't be processed after max iterations.</returns>
    private List<RegistrationStatisticRule> ProcessPendingRules(List<RegistrationStatisticRule> pendingRules, StatisticsProcessorContext context)
    {
        var remainingRules = new List<RegistrationStatisticRule>();
        var ruleGroups = new Queue<IGrouping<string, RegistrationStatisticRule>>(pendingRules.GroupBy(r => r.Name));
        var requeuedGroupNames = new HashSet<string>();
        var iteration = 0;

        while (ruleGroups.Count > 0 && iteration < MaxReferenceResolutionIterations)
        {
            iteration++;
            var currentGroup = ruleGroups.Dequeue();
            var groupName = currentGroup.Key;

            // Get names of statistics still pending (potential dependencies)
            var pendingStatisticNames = ruleGroups.Select(g => g.Key).ToHashSet();

            var statGroup = context.Statistics.GetGroup(groupName, createNonExisting: false)
                            ?? new StatisticValuesGroup(groupName);
            var unhandledRules = new List<RegistrationStatisticRule>();

            // Process rules without stacking bonuses first
            var rulesWithoutBonus = currentGroup.Where(r => string.IsNullOrWhiteSpace(r.StackingBonus)).ToList();
            foreach (var rule in rulesWithoutBonus)
            {
                if (!TryProcessSinglePendingRule(rule, statGroup, context, pendingStatisticNames))
                {
                    unhandledRules.Add(rule);
                }
            }

            // Process rules grouped by stacking bonus
            var stackingBonusGroups = currentGroup
                .Where(r => !string.IsNullOrWhiteSpace(r.StackingBonus))
                .GroupBy(r => r.StackingBonus);

            foreach (var bonusGroup in stackingBonusGroups)
            {
                if (!TryProcessStackingBonusGroup(bonusGroup, statGroup, context, pendingStatisticNames, out var unprocessedInGroup))
                {
                    unhandledRules.AddRange(unprocessedInGroup);
                }
            }

            // Determine what to do with this group
            if (unhandledRules.Count == 0)
            {
                // Successfully processed all rules in this group
                if (!context.Statistics.ContainsGroup(groupName))
                {
                    context.Statistics.AddGroup(statGroup);
                }
            }
            else if (requeuedGroupNames.Contains(groupName))
            {
                // Already requeued once - add what we can and track remaining
                if (statGroup.GetValues().Any() && !context.Statistics.ContainsGroup(groupName))
                {
                    context.Statistics.AddGroup(statGroup);
                }
                remainingRules.AddRange(unhandledRules);
            }
            else
            {
                // First time we couldn't process - requeue for another pass
                ruleGroups.Enqueue(currentGroup);
                requeuedGroupNames.Add(groupName);
            }
        }

        // Add any groups still in queue to remaining rules
        while (ruleGroups.Count > 0)
        {
            remainingRules.AddRange(ruleGroups.Dequeue());
        }

        return remainingRules;
    }

    /// <summary>
    /// Attempts to process a single pending rule by resolving references or evaluating expressions.
    /// </summary>
    private bool TryProcessSinglePendingRule(
        RegistrationStatisticRule rule,
        StatisticValuesGroup statGroup,
        StatisticsProcessorContext context,
        HashSet<string> pendingStatisticNames)
    {
        // Check if this is a reference to another statistic
        if (!rule.IsNumberValue())
        {
            // Don't try to resolve if the referenced statistic is still pending
            if (pendingStatisticNames.Contains(rule.Value))
            {
                return false;
            }

            // Try to resolve the reference
            if (!context.Statistics.ContainsGroup(rule.Value))
            {
                return false;
            }

            var referencedValue = context.Statistics.GetValue(rule.Value);
            var cappedValue = ApplyCap(rule, referencedValue, context, pendingStatisticNames);

            var source = rule.AssociatedStatisticRuleId.Value.ToString();
            var displayName = context.RegistrationLookup.GetValueOrDefault(rule.ParentRegistrationId) ?? rule.Name;

            statGroup.AddValue(source, cappedValue, displayName, rule.AssociatedStatisticRuleId.Value);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Processes a group of rules with the same stacking bonus, keeping only the highest value.
    /// </summary>
    private bool TryProcessStackingBonusGroup(
        IGrouping<string?, RegistrationStatisticRule> bonusGroup,
        StatisticValuesGroup statGroup,
        StatisticsProcessorContext context,
        HashSet<string> pendingStatisticNames,
        out List<RegistrationStatisticRule> unprocessedRules)
    {
        unprocessedRules = [];

        if (bonusGroup.Count() == 1)
        {
            var rule = bonusGroup.First();

            // Try to get the value (handles both numeric and reference values)
            if (TryGetRuleValue(rule, context, pendingStatisticNames, out var value))
            {
                var sourcex = $"{bonusGroup.Key}:{rule.AssociatedStatisticRuleId.Value}";
                var displayNamex = context.RegistrationLookup.GetValueOrDefault(rule.ParentRegistrationId) ?? rule.Name;

                statGroup.AddValue(sourcex, value, $"{displayNamex} ({bonusGroup.Key})", rule.AssociatedStatisticRuleId.Value);
                return true;
            }

            unprocessedRules.Add(rule);
            return false;
        }

        var highestValue = int.MinValue;
        var sourceRules = new List<RegistrationStatisticRule>();
        var allResolved = true;

        foreach (var rule in bonusGroup)
        {
            if (!TryGetRuleValue(rule, context, pendingStatisticNames, out var value))
            {
                unprocessedRules.Add(rule);
                allResolved = false;
                continue;
            }

            if (value > highestValue)
            {
                highestValue = value;
                sourceRules.Clear();
                sourceRules.Add(rule);
            }
            else if (value == highestValue)
            {
                sourceRules.Add(rule);
            }
        }

        if (!allResolved)
        {
            return false;
        }

        // Build display name from all sources with the highest value
        var displayNames = sourceRules
            .Select(r => context.RegistrationLookup.GetValueOrDefault(r.ParentRegistrationId) ?? r.Name)
            .ToList();
        var displayName = string.Join(" | ", displayNames);

        var firstRule = sourceRules.First();
        var source = $"{bonusGroup.Key}:{firstRule.AssociatedStatisticRuleId.Value}";

        statGroup.AddValue(source, highestValue, $"{displayName} ({bonusGroup.Key})", firstRule.AssociatedStatisticRuleId.Value);
        return true;
    }

    /// <summary>
    /// Gets the numeric value for a rule, resolving references if necessary.
    /// </summary>
    private bool TryGetRuleValue(
        RegistrationStatisticRule rule,
        StatisticsProcessorContext context,
        HashSet<string> pendingStatisticNames,
        out int value)
    {
        value = 0;

        if (rule.IsNumberValue())
        {
            value = rule.GetValue();
            if (rule.Value.StartsWith("-"))
            {
                value = -value;
            }

            value = ApplyCap(rule, value, context, pendingStatisticNames);
            return true;
        }

        // Reference to another statistic
        if (pendingStatisticNames.Contains(rule.Value))
        {
            return false;
        }

        if (!context.Statistics.ContainsGroup(rule.Value))
        {
            return false;
        }

        value = context.Statistics.GetValue(rule.Value);
        value = ApplyCap(rule, value, context, pendingStatisticNames);
        return true;
    }

    /// <summary>
    /// Applies maximum cap constraint if specified on the rule.
    /// </summary>
    private int ApplyCap(
        RegistrationStatisticRule rule,
        int value,
        StatisticsProcessorContext context,
        HashSet<string> pendingStatisticNames)
    {
        if (!rule.MaximumValue.HasValue || rule.MaximumValue.Value <= 0)
        {
            return value;
        }

        // If max is a reference, try to resolve it
        // For now, just use the MaximumValue as-is since it's an int
        return Math.Min(value, rule.MaximumValue.Value);
    }

    private void ProcessReferenceValueRules(List<RegistrationStatisticRule> referenceValueRules, StatisticsProcessorContext context)
    {
        var iteration = 0;

        while (referenceValueRules.Count > 0 && iteration < MaxReferenceResolutionIterations)
        {
            iteration++;
            var processed = new List<RegistrationStatisticRule>();

            foreach (var rule in referenceValueRules)
            {
                if (TryResolveReference(rule, context))
                {
                    processed.Add(rule);
                }
            }

            foreach (var processedRule in processed)
            {
                referenceValueRules.Remove(processedRule);
            }

            if (processed.Count == 0)
            {
                break;
            }
        }
    }

    private bool TryResolveReference(RegistrationStatisticRule rule, StatisticsProcessorContext context)
    {
        var referencedStatName = rule.Value;

        if (!context.Statistics.ContainsGroup(referencedStatName))
        {
            return false;
        }

        var referencedValue = context.Statistics.GetValue(referencedStatName);
        var group = context.Statistics.GetGroup(rule.Name);
        var source = rule.AssociatedStatisticRuleId.Value.ToString();
        var displayName = context.RegistrationLookup.GetValueOrDefault(rule.ParentRegistrationId) ?? rule.Name;

        group.AddValue(source, referencedValue, displayName, rule.AssociatedStatisticRuleId.Value);
        return true;
    }

    private void ApplyStackingBonuses(List<(RegistrationStatisticRule Rule, int Value)> directValueRules, StatisticsProcessorContext context)
    {
        var stackingGroups = directValueRules
            .Where(r => !string.IsNullOrWhiteSpace(r.Rule.StackingBonus))
            .GroupBy(r => new { r.Rule.Name, r.Rule.StackingBonus });

        foreach (var stackingGroup in stackingGroups)
        {
            ApplyStackingGroup(stackingGroup, context);
        }
    }

    private void ApplyStackingGroup(IGrouping<dynamic, (RegistrationStatisticRule Rule, int Value)> stackingGroup, StatisticsProcessorContext context)
    {
        var highestValue = stackingGroup.Max(r => r.Value);
        var (rule, _) = stackingGroup.First(r => r.Value == highestValue);

        var group = context.Statistics.GetGroup(stackingGroup.Key.Name);
        var source = $"{stackingGroup.Key.StackingBonus}:{rule.AssociatedStatisticRuleId.Value}";
        var displayName = context.RegistrationLookup.GetValueOrDefault(rule.ParentRegistrationId) ?? rule.Name;

        group.AddValue(source, highestValue, $"{displayName} ({stackingGroup.Key.StackingBonus})", rule.AssociatedStatisticRuleId.Value);
    }

    private void ApplyMinMaxConstraints(List<RegistrationStatisticRule> validStatistics, StatisticsProcessorContext context)
    {
        foreach (var stat in validStatistics)
        {
            if (!context.Statistics.ContainsGroup(stat.Name))
            {
                continue;
            }

            var group = context.Statistics.GetGroup(stat.Name);
            var currentSum = group.Sum();
            var displayName = context.RegistrationLookup.GetValueOrDefault(stat.ParentRegistrationId) ?? stat.Name;

            ApplyMaximumConstraint(stat, group, currentSum, displayName);
            ApplyMinimumConstraint(stat, group, currentSum, displayName);
        }
    }

    private static void ApplyMaximumConstraint(RegistrationStatisticRule stat, StatisticValuesGroup group, int currentSum, string displayName)
    {
        if (stat.MaximumValue.HasValue && stat.MaximumValue > 0 && currentSum > stat.MaximumValue)
        {
            var excess = currentSum - stat.MaximumValue.Value;
            group.AddValue($"cap_max:{stat.AssociatedStatisticRuleId.Value}", -excess, $"{displayName} (Max Cap)", stat.AssociatedStatisticRuleId.Value);
        }
    }

    private static void ApplyMinimumConstraint(RegistrationStatisticRule stat, StatisticValuesGroup group, int currentSum, string displayName)
    {
        if (stat.MinimumValue.HasValue && stat.MinimumValue > 0 && currentSum < stat.MinimumValue)
        {
            var deficit = stat.MinimumValue.Value - currentSum;
            group.AddValue($"cap_min:{stat.AssociatedStatisticRuleId.Value}", deficit, $"{displayName} (Min Floor)", stat.AssociatedStatisticRuleId.Value);
        }
    }

    private void FinalizeStatistics(StatisticsProcessorContext context)
    {
        foreach (var group in context.Statistics)
        {
            group.MarkAsFinalized();
        }
    }
}
