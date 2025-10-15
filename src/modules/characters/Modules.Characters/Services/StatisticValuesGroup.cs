namespace Starlights.Modules.Characters.Services;

/// <summary>
/// Represents a single statistic value contribution with its source information.
/// </summary>
/// <param name="Source">The identifier of the source (e.g., rule ID or calculated source)</param>
/// <param name="Value">The numeric value contributed</param>
/// <param name="DisplayName">Optional display name for the source</param>
/// <param name="RuleId">Optional ID of the originating registration statistic rule</param>
public record StatisticValue(string Source, int Value, string? DisplayName = null, Guid? RuleId = null);

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
    /// Indicate that all provided rules have been handled and that this group can be used to get the total sum without missing out on pending additions.
    /// </summary>
    public bool IsFinalized { get; set; }

    public void AddValue(string source, int value, string? displayName = null, Guid? ruleId = null)
    {
        if (_values.ContainsKey(source))
        {
            var existing = _values[source];
            _values[source] = existing with { Value = existing.Value + value };
        }
        else
        {
            _values.Add(source, new StatisticValue(source, value, displayName, ruleId));
        }
    }

    public void AddValue(StatisticValue statisticValue)
    {
        if (_values.ContainsKey(statisticValue.Source))
        {
            var existing = _values[statisticValue.Source];
            _values[statisticValue.Source] = existing with { Value = existing.Value + statisticValue.Value };
        }
        else
        {
            _values.Add(statisticValue.Source, statisticValue);
        }
    }

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

    public int Sum()
    {
        return _values.Sum(x => x.Value.Value);
    }

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

    public void Merge(StatisticValuesGroup group)
    {
        if (group == null)
        {
            return;
        }

        if (group.Sum() > 0)
        {
            foreach (var value in group.GetValues())
            {
                AddValue(value);
            }
        }
    }

    public void Finalized()
    {
        IsFinalized = true;
    }
}

public class StatisticValuesGroupCollection : List<StatisticValuesGroup>
{
    public bool ContainsGroup(string groupName)
    {
        if (groupName == null)
        {
            return false;
        }

        if (groupName.StartsWith("-"))
        {
            return this.Any(x => x.GroupName.Equals(groupName[1..]));
        }

        return this.Any(x => x.GroupName.Equals(groupName));
    }

    public void AddGroup(StatisticValuesGroup group)
    {
        if (ContainsGroup(group.GroupName))
        {
            var existingGroup = GetGroup(group.GroupName);

            //add values
            foreach (var value in group.GetValues())
            {
                existingGroup.AddValue(value);
            }
        }
        else
        {
            Add(group);
        }
    }

    /// <summary>
    /// Gets the group with the specified group name, if there is none it returns a new one with the name and adds it to this collection
    /// </summary>
    public StatisticValuesGroup GetGroup(string groupName, bool createNonExisting = true)
    {
        if (groupName.StartsWith("-"))
        {
            groupName = groupName[1..];
        }

        if (ContainsGroup(groupName))
        {
            return this.Single(x => x.GroupName.Equals(groupName));
        }

        if (createNonExisting)
        {
            var group = new StatisticValuesGroup(groupName);
            Add(group);
            return group;
        }

        return null!;
    }

    public int GetValue(string groupName)
    {
        if (ContainsGroup(groupName))
        {
            return GetGroup(groupName).Sum();
        }

        return 0;
    }
}