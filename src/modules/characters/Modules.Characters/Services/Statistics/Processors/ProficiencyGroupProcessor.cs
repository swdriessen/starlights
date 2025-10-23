using Microsoft.Extensions.Logging;

namespace Starlights.Modules.Characters.Services.Statistics.Processors;

/// <summary>
/// Processes proficiency-related statistic groups early in the calculation pipeline.
/// </summary>
internal sealed class ProficiencyGroupProcessor : IStatisticGroupProcessor
{
    private readonly ILogger<ProficiencyGroupProcessor> _logger;

    public ProficiencyGroupProcessor(ILogger<ProficiencyGroupProcessor> logger)
    {
        _logger = logger;
    }

    public int Order => 1;

    public void Process(Dictionary<string, StatisticRuleGroup> pendingGroups, StatisticsProcessorContext context, Func<StatisticRuleGroup, StatisticsProcessorContext, StatisticValuesGroup?> processGroupNode)
    {
        const string groupKey = "proficiency";
        const string groupDisplayName = "Proficiency Bonus"; // either from element or initializer

        context.Statistics.WithGroup("proficiency", group =>
        {
            group.DisplayName = groupDisplayName;
        });

        if (pendingGroups.TryGetValue(groupKey, out var proficiencyNode))
        {
            var result = processGroupNode(proficiencyNode, context);
            if (result is StatisticValuesGroup group && group.IsCompleted)
            {
                pendingGroups.Remove(group.GroupName);
            }
        }

        var proficiencyGroup = context.Statistics.GetGroup(groupKey);

        context.Statistics.WithGroupVariants(proficiencyGroup.GroupName, proficiencyGroup.DisplayName);

        // Validate no remaining proficiency-related rules
        if (pendingGroups.Any(r => r.Key.StartsWith(groupKey)))
        {
            _logger.LogError("[{Processor}] Pending statistic rules contain {GroupName}-related rules that should have been processed already.",
                nameof(ProficiencyGroupProcessor), groupKey);
        }
    }
}