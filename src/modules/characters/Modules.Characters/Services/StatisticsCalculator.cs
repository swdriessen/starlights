using Starlights.Modules.Characters.Domain.Abilities;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.Modules.Characters.Domain.Progression;
using Starlights.Modules.Characters.Domain.Registrations;

namespace Starlights.Modules.Characters.Services;

public class StatisticsCalculator
{
    public StatisticValuesGroupCollection Calculate(Character character, List<Registration> registrations)
    {
        var allStatistics = registrations.SelectMany(x => x.StatisticRules).ToList();

        // Get character level for level requirement filtering
        var progressionComponent = character.GetRequiredComponent<ProgressionComponent>();
        var characterLevel = progressionComponent.CharacterLevel;

        // Filter out rules that do not meet level requirements
        var validStatistics = allStatistics.Where(stat => stat.LevelRequirement <= characterLevel).ToList();

        // Initialize the statistics collection
        var statistics = new StatisticValuesGroupCollection();

        // Seed character level statistic first (needed for other calculations)
        statistics.GetGroup("character:level").AddValue("base", characterLevel, "Character Level");
        statistics.GetGroup("level").AddValue("base", characterLevel, "Character Level");

        // Get ability scores and seed ability-related statistics
        var abilitiesComponent = character.GetRequiredComponent<AbilitiesComponent>();
        foreach (var abilityScore in abilitiesComponent.AbilityScores)
        {
            var abilityName = abilityScore.Abbreviation.ToLowerInvariant();

            // Seed ability-related statistics
            statistics.GetGroup($"{abilityName}:score").AddValue("base", abilityScore.CalculatedScore, abilityScore.Name);
            statistics.GetGroup($"{abilityName}:modifier").AddValue("base", abilityScore.CalculatedModifier, $"{abilityScore.Name} Modifier");

            // Add halved variants (rounded down and up)
            var modifierHalfDown = (int)Math.Floor(abilityScore.CalculatedModifier / 2.0);
            var modifierHalfUp = (int)Math.Ceiling(abilityScore.CalculatedModifier / 2.0);
            statistics.GetGroup($"{abilityName}:modifier_half").AddValue("base", modifierHalfDown, $"{abilityScore.Name} Half Modifier");
            statistics.GetGroup($"{abilityName}:modifier_half_down").AddValue("base", modifierHalfDown, $"{abilityScore.Name} Half Modifier (Down)");
            statistics.GetGroup($"{abilityName}:modifier_half_up").AddValue("base", modifierHalfUp, $"{abilityScore.Name} Half Modifier (Up)");
        }

        // Create a lookup for registration names to use as display names
        var registrationLookup = registrations.ToDictionary(r => r.Id, r => r.AssociatedElementName);

        // Separate statistics into direct values and references
        var directValueRules = new List<(RegistrationStatisticRule Rule, int Value)>();
        var referenceValueRules = new List<RegistrationStatisticRule>();

        foreach (var stat in validStatistics)
        {
            if (stat.IsNumberValue())
            {
                var value = stat.GetValue();
                // Check if the original value starts with a minus sign for negatives
                if (stat.Value.StartsWith("-"))
                {
                    value = -value;
                }
                directValueRules.Add((stat, value));
            }
            else
            {
                referenceValueRules.Add(stat);
            }
        }

        // Process direct value rules first (no stacking bonus) - these include proficiency
        foreach (var (rule, value) in directValueRules.Where(r => string.IsNullOrWhiteSpace(r.Rule.StackingBonus)))
        {
            var group = statistics.GetGroup(rule.Name);
            var source = rule.AssociatedStatisticRuleId.Value.ToString();
            var displayName = registrationLookup.GetValueOrDefault(rule.ParentRegistrationId) ?? rule.Name;

            // Check if value starts with + or - to determine if it's additive
            if (rule.Value.StartsWith("+") || rule.Value.StartsWith("-"))
            {
                group.AddValue(source, value, displayName, rule.AssociatedStatisticRuleId.Value);
            }
            else
            {
                // Set value (replace existing)
                group.AddValue(source, value, displayName, rule.AssociatedStatisticRuleId.Value);
            }
        }

        // After processing direct values, add halved proficiency variants if proficiency exists
        if (statistics.ContainsGroup("proficiency"))
        {
            var proficiencyValue = statistics.GetValue("proficiency");
            var proficiencyHalfDown = (int)Math.Floor(proficiencyValue / 2.0);
            var proficiencyHalfUp = (int)Math.Ceiling(proficiencyValue / 2.0);
            statistics.GetGroup("proficiency:half").AddValue("calculated", proficiencyHalfDown, "Half Proficiency");
            statistics.GetGroup("proficiency:half_down").AddValue("calculated", proficiencyHalfDown, "Half Proficiency (Down)");
            statistics.GetGroup("proficiency:half_up").AddValue("calculated", proficiencyHalfUp, "Half Proficiency (Up)");
            statistics.GetGroup("proficiency:bonus").AddValue("calculated", proficiencyValue, "Proficiency Bonus");
        }

        // Process reference value rules (values that reference other statistics)
        var maxIterations = 10; // Prevent infinite loops
        var iteration = 0;

        while (referenceValueRules.Count > 0 && iteration < maxIterations)
        {
            iteration++;
            var processed = new List<RegistrationStatisticRule>();

            foreach (var rule in referenceValueRules)
            {
                var referencedStatName = rule.Value;

                // Try to resolve the referenced statistic
                if (statistics.ContainsGroup(referencedStatName))
                {
                    var referencedValue = statistics.GetValue(referencedStatName);
                    var group = statistics.GetGroup(rule.Name);
                    var source = rule.AssociatedStatisticRuleId.Value.ToString();
                    var displayName = registrationLookup.GetValueOrDefault(rule.ParentRegistrationId) ?? rule.Name;

                    group.AddValue(source, referencedValue, displayName, rule.AssociatedStatisticRuleId.Value);
                    processed.Add(rule);
                }
            }

            // Remove processed rules
            foreach (var processedRule in processed)
            {
                referenceValueRules.Remove(processedRule);
            }

            // If no progress was made, break to avoid infinite loop
            if (processed.Count == 0)
            {
                break;
            }
        }

        // Apply stacking bonuses (highest value wins per stacking type)
        var stackingGroups = directValueRules
            .Where(r => !string.IsNullOrWhiteSpace(r.Rule.StackingBonus))
            .GroupBy(r => new { StatName = r.Rule.Name, StackingType = r.Rule.StackingBonus });

        foreach (var stackingGroup in stackingGroups)
        {
            var highestValue = stackingGroup.Max(r => r.Value);
            var (Rule, Value) = stackingGroup.First(r => r.Value == highestValue);

            var group = statistics.GetGroup(stackingGroup.Key.StatName);
            var source = $"{stackingGroup.Key.StackingType}:{Rule.AssociatedStatisticRuleId.Value}";
            var displayName = registrationLookup.GetValueOrDefault(Rule.ParentRegistrationId) ?? Rule.Name;

            if (Rule.Value.StartsWith("+") || Rule.Value.StartsWith("-"))
            {
                group.AddValue(source, highestValue, $"{displayName} ({stackingGroup.Key.StackingType})", Rule.AssociatedStatisticRuleId.Value);
            }
            else
            {
                group.AddValue(source, highestValue, $"{displayName} ({stackingGroup.Key.StackingType})", Rule.AssociatedStatisticRuleId.Value);
            }
        }

        // Apply minimum and maximum values
        foreach (var stat in validStatistics)
        {
            if (statistics.ContainsGroup(stat.Name))
            {
                var group = statistics.GetGroup(stat.Name);
                var currentSum = group.Sum();
                var displayName = registrationLookup.GetValueOrDefault(stat.ParentRegistrationId) ?? stat.Name;

                // Apply maximum
                if (stat.MaximumValue.HasValue && stat.MaximumValue > 0 && currentSum > stat.MaximumValue)
                {
                    // Adjust by adding a capping value
                    var excess = currentSum - stat.MaximumValue.Value;
                    group.AddValue($"cap_max:{stat.AssociatedStatisticRuleId.Value}", -excess, $"{displayName} (Max Cap)", stat.AssociatedStatisticRuleId.Value);
                }

                // Apply minimum
                if (stat.MinimumValue.HasValue && stat.MinimumValue > 0 && currentSum < stat.MinimumValue)
                {
                    // Adjust by adding a floor value
                    var deficit = stat.MinimumValue.Value - currentSum;
                    group.AddValue($"cap_min:{stat.AssociatedStatisticRuleId.Value}", deficit, $"{displayName} (Min Floor)", stat.AssociatedStatisticRuleId.Value);
                }
            }
        }

        // Mark all groups as finalized
        foreach (var group in statistics)
        {
            group.Finalized();
        }

        return statistics;
    }
}
