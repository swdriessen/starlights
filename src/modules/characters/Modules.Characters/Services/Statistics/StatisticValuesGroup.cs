using System.Runtime.InteropServices;

namespace Starlights.Modules.Characters.Services.Statistics;

public class StatisticValuesGroup
{
    private readonly Dictionary<string, StatisticValue> _values;

    public StatisticValuesGroup(string groupName)
    {
        GroupName = groupName;
        _values = [];
    }

    /// <summary>
    /// The 'key' statistic name of the group.
    /// </summary>
    public string GroupName { get; }

    /// <summary>
    /// Gets or sets the display name associated with the object.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Indicate that all provided rules have been handled and that this group can be used to get the total sum without missing out on pending additions.
    /// </summary>
    public bool IsFinalized { get; private set; }

    /// <summary>
    /// Adds a statistic value to the group, aggregating if the source already exists.
    /// </summary>
    public void AddValue(string source, int value, string? displayName = null, Guid? ruleId = null)
    {
        var statisticValue = new StatisticValue(source, value, displayName, ruleId);
        AddValue(statisticValue);
    }

    /// <summary>
    /// Adds a statistic value to the group, aggregating if the source already exists.
    /// </summary>
    public void AddValue(StatisticValue statisticValue)
    {
        ref var existing = ref CollectionsMarshal.GetValueRefOrAddDefault(_values, statisticValue.Source, out var exists);
        if (exists)
        {
            existing = existing with { Value = existing.Value + statisticValue.Value };
        }
        else
        {
            existing = statisticValue;
        }
    }

    /// <summary>
    /// Checks if a value from the specified source exists in this group.
    /// </summary>
    public bool ContainsValue(string source)
    {
        return _values.ContainsKey(source);
    }

    /// <summary>
    /// Gets all statistic values that contribute to this group.
    /// </summary>
    public IReadOnlyCollection<StatisticValue> GetValues()
    {
        return _values.Values.ToList().AsReadOnly();
    }

    /// <summary>
    /// Gets the breakdown of values as a dictionary for backwards compatibility.
    /// </summary>
    public Dictionary<string, int> GetValuesAsDictionary()
    {
        return _values.ToDictionary(x => x.Key, x => x.Value.Value);
    }

    /// <summary>
    /// Calculates the sum of all values in this group.
    /// </summary>
    public int Sum()
    {
        return _values.Sum(x => x.Value.Value);
    }

    /// <summary>
    /// Gets a summary string of the group's values.
    /// </summary>
    public string GetSummary(bool includeValues = true)
    {
        if (_values.Count == 0)
        {
            return string.Empty;
        }

        if (includeValues)
        {
            return string.Join(", ", _values.Values.Select(v =>
            {
                var name = v.DisplayName ?? v.Source;
                return $"{name} ({(v.Value >= 0 ? "+" : "")}{v.Value})";
            }));
        }

        return string.Join(", ", _values.Values.Select(v => v.DisplayName ?? v.Source));
    }

    public override string ToString()
    {
        return $"{GroupName} [{Sum()}]";
    }

    /// <summary>
    /// Merges another group's values into this group.
    /// </summary>
    public void Merge(StatisticValuesGroup? group)
    {
        if (group is null)
        {
            return;
        }

        foreach (var value in group.GetValues())
        {
            AddValue(value);
        }
    }

    /// <summary>
    /// Marks this group as finalized, indicating all rules have been processed.
    /// </summary>
    public void MarkAsFinalized()
    {
        IsFinalized = true;
    }
}
