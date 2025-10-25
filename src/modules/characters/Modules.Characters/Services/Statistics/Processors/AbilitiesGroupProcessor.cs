using Starlights.Modules.Characters.Domain.Abilities;

namespace Starlights.Modules.Characters.Services.Statistics.Processors;

internal sealed class AbilitiesGroupProcessor : IStatisticGroupProcessor
{
    public int Order => 5;

    public void Process(Dictionary<string, StatisticRuleGroup> pendingGroups, StatisticsProcessorContext context, Func<StatisticRuleGroup, StatisticsProcessorContext, StatisticValuesGroup?> processGroupNode)
    {
        var component = context.Character.GetRequiredComponent<AbilitiesComponent>();

        // ensure ability score related groups are processed to completion
        foreach (var score in component.AbilityScores)
        {
            var groupKey = score.Name.ToSlug();

            if (pendingGroups.TryGetValue(groupKey, out var pendingGroup))
            {
                var result = processGroupNode(pendingGroup, context);
                if (result is StatisticValuesGroup group && group.IsCompleted)
                {
                    continue;
                }

                context.AddError($"Failed to process ability score group '{groupKey}' - if this group has dependencies, those need to be processed first (recursively)");
                throw new InvalidOperationException($"Failed to process ability score group '{groupKey}' - if this group has dependencies, those need to be processed first (recursively)");
            }

            if (pendingGroups.TryGetValue($"{groupKey}:max", out var pendingMaximumGroup))
            {
                var result = processGroupNode(pendingMaximumGroup, context);
                if (result is StatisticValuesGroup group && group.IsCompleted)
                {
                    continue;
                }

                context.AddError($"Failed to process ability score group '{groupKey}:max' - if this group has dependencies, those need to be processed first (recursively)");
                throw new InvalidOperationException($"Failed to process ability score group '{groupKey}:max' - if this group has dependencies, those need to be processed first (recursively)");
            }
        }


        // update ability scores based on processed groups
        foreach (var score in component.AbilityScores)
        {
            var groupKey = score.Name.ToSlug();

            var newMaximumScore = 20; // default maximum

            if (context.Statistics.TryGetGroup($"{groupKey}:max", out var maximumGroup))
            {
                if (maximumGroup.IsCompleted)
                {
                    newMaximumScore = maximumGroup.Sum();
                }
                else
                {
                    context.AddError($"Ability score maximum group '{groupKey}:max' is not completed");
                }
            }

            // TODO: maximum score on entity defaults to 20 in ruleset, but can be increased via statistic rules

            // in case statistics were removed, make sure to still update the additional score (to zero)
            var newAdditionalScore = 0;

            if (context.Statistics.TryGetGroup(groupKey, out var abilityGroup))
            {
                if (abilityGroup.IsCompleted)
                {
                    newAdditionalScore = abilityGroup.Sum();
                }
                else
                {
                    context.AddError($"Ability score group '{groupKey}' is not completed");
                    // errors will be logged, but we still want to update as much as possible without failing completely
                }
            }

            component.UpdateAbilityAdditionalScore(score.Id, newAdditionalScore);


            var scoreGroup = context.Statistics.WithGroup($"{groupKey}:score", group =>
            {
                group.WithValue(score.CalculatedScore, $"{score.Name}");
                group.Complete();
            });

            var modifierGroup = context.Statistics.WithGroup($"{groupKey}:modifier", group =>
            {
                group.WithValue(score.CalculatedModifier, $"{score.Name} Modifier");
                group.Complete();
            });

            context.Statistics.WithGroupVariants(scoreGroup.GroupName, score.Name);
            context.Statistics.WithGroupVariants(modifierGroup.GroupName, score.Name);
        }

    }
}