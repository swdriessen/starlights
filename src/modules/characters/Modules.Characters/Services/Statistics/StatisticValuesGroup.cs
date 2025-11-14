using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Starlights.Modules.Characters.Services.Statistics;

[DebuggerDisplay("Key = {GroupName} ({DisplayName}) Sum = {Sum()} IsCompleted = {IsCompleted}")]
public class StatisticValuesGroup
{
    private readonly Dictionary<string, StatisticValue> _statisticValues;

    public StatisticValuesGroup(string groupName)
    {
        GroupName = groupName;
        _statisticValues = [];
    }

    /// <summary>
    /// The 'key' statistic name of the group.
    /// </summary>
    public string GroupName { get; }

    /// <summary>
    /// Gets or sets the display name associated with the object. This is a more user-friendly name compared to the GroupName.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Indicate that all provided rules have been handled and that this group can be used to get the total sum without missing out on pending additions.
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// Adds a statistic value to the group, aggregating if the source already exists.
    /// </summary>
    public void AddValue(string source, int value, string? displayName = null, Guid? ruleId = null)
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Cannot add values to a completed StatisticValuesGroup.");
        }

        var statisticValue = new StatisticValue(source, value, displayName, ruleId);
        AddValue(statisticValue);
    }

    /// <summary>
    /// Adds a statistic value to the group, aggregating if the source already exists.
    /// </summary>
    public void AddValue(StatisticValue statisticValue)
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Cannot add values to a completed StatisticValuesGroup.");
        }

        ref var existing = ref CollectionsMarshal.GetValueRefOrAddDefault(_statisticValues, statisticValue.Source, out var exists);
        if (exists && existing is not null)
        {
            existing = existing with { Value = existing.Value + statisticValue.Value };
        }
        else
        {
            existing = statisticValue;
        }
    }

    /// <summary>
    /// Gets all statistic values that contribute to this group.
    /// </summary>
    public IReadOnlyCollection<StatisticValue> GetStatisticValues()
    {
        return _statisticValues.Values.ToList().AsReadOnly();
    }

    /// <summary>
    /// Determines whether any statistic values are present.
    /// </summary>
    /// <returns>true if one or more statistic values exist; otherwise, false.</returns>
    public bool HasStatisticValues()
    {
        return _statisticValues.Count > 0;
    }

    /// <summary>
    /// Calculates the sum of all statistic values in the collection.
    /// </summary>
    /// <returns>The total sum of the values contained in the collection. Returns 0 if the collection is empty.</returns>
    public int Sum()
    {
        return _statisticValues.Values.Sum(x => x.Value);
    }

    /// <summary>
    /// Generates a summary string of the current statistics, optionally including their values.
    /// </summary>
    /// <param name="includeValues">true to include the values of each statistic in the summary; otherwise, false to include only the names.</param>
    /// <returns>A comma-separated string containing the names of the statistics, with values included if specified; or an empty
    /// string if there are no statistics.</returns>
    public string GetSummary(bool includeValues = true)
    {
        if (_statisticValues.Count == 0)
        {
            return string.Empty;
        }

        if (includeValues)
        {
            return string.Join(", ", _statisticValues.Values.Select(v =>
            {
                var name = v.DisplayName ?? v.Source;
                return $"{name} ({(v.Value >= 0 ? "+" : "")}{v.Value})";
            }));
        }

        return string.Join(", ", _statisticValues.Values.Select(v => v.DisplayName ?? v.Source));
    }

    /// <summary>
    /// Merges the statistic values from the specified group into the current group.
    /// </summary>
    /// <remarks>This method adds each statistic value from the provided group to the current group. If the
    /// group contains no values, or if the group is null, the method performs no operation.</remarks>
    /// <param name="group">The group whose statistic values are to be merged. If null, no action is taken.</param>
    public void Merge(StatisticValuesGroup? group)
    {
        if (group is null)
        {
            return;
        }

        foreach (var value in group.GetStatisticValues())
        {
            AddValue(value);
        }
    }

    /// <summary>
    /// Marks the operation as completed, indicating all rules have been processed.
    /// </summary>
    /// <remarks>Once this method is called, the operation is considered complete and cannot be marked as
    /// incomplete again. Subsequent calls have no additional effect.</remarks>
    public void Complete()
    {
        IsCompleted = true;
    }

    public override string ToString()
    {
        return $"{GroupName} [{Sum()}]";
    }
}
