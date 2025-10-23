using Starlights.Modules.Characters.Domain.Abilities;

namespace Starlights.Modules.Characters.Services.Statistics.Processors;

internal sealed class AbilitiesGroupProcessor : IStatisticGroupProcessor
{
    public int Order => 5;

    public void Process(Dictionary<string, StatisticRuleGroup> pendingGroups, StatisticsProcessorContext context, Func<StatisticRuleGroup, StatisticsProcessorContext, StatisticValuesGroup?> processGroupNode)
    {



        context.Character.UpdateComponent<AbilitiesComponent>((a, _) =>
        {
            foreach (var score in a.AbilityScores)
            {
                var abilityGroupKey = score.Name.ToSlug();

                // first process any pending groups for this ability score
                if (pendingGroups.TryGetValue(abilityGroupKey, out var abilityRuleGroup))
                {
                    var result = processGroupNode(abilityRuleGroup, context);
                    if (result is StatisticValuesGroup abilityValuesGroup && abilityValuesGroup.IsCompleted)
                    {
                        pendingGroups.Remove(abilityValuesGroup.GroupName);
                    }
                    else
                    {
                        // TODO: if this group has dependencies, those need to be processed first (recursively)
                        throw new InvalidOperationException($"Failed to process ability score group '{abilityGroupKey}' - if this group has dependencies, those need to be processed first (recursively)");
                    }
                }

                var group = context.Statistics.GetGroup(abilityGroupKey);

                score.UpdateAdditionalScore(group.Sum());
                // TODO: maximum score on entity defaults to 20 in ruleset, but can be increased via statistic rules

                var scoreGroup = context.Statistics.WithGroup($"{abilityGroupKey}:score", group =>
                {
                    group.WithValue(score.CalculatedScore, $"{score.Name}");
                    group.Complete();
                });

                var modifierGroup = context.Statistics.WithGroup($"{abilityGroupKey}:modifier", group =>
                {
                    group.WithValue(score.CalculatedModifier, $"{score.Name} Modifier");
                    group.Complete();
                });

                context.Statistics.WithGroupVariants(scoreGroup.GroupName, score.Name);
                context.Statistics.WithGroupVariants(modifierGroup.GroupName, score.Name);

                // make sure pendingRules do not container more ability score related rules as those are now processed
                // throw exception for now if they do
                if (pendingGroups.Any(r => r.Key.StartsWith(abilityGroupKey)))
                {
                    throw new InvalidOperationException("Pending statistic rules contain ability score related rules that should have been processed already");
                }
            }
        });

    }
}