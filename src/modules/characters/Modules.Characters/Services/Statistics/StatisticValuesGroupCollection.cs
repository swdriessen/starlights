using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Starlights.Modules.Characters.Services.Statistics;

/// <summary>
/// Manages a collection of statistic value groups with efficient lookup and aggregation capabilities.
/// </summary>
public sealed class StatisticValuesGroupCollection : IEnumerable<StatisticValuesGroup>
{
    private readonly Dictionary<string, StatisticValuesGroup> _groups = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Gets the number of groups in the collection.
    /// </summary>
    public int Count => _groups.Count;

    /// <summary>
    /// Determines whether a group with the specified name exists in the collection.
    /// Group names starting with '-' have the prefix removed before checking.
    /// </summary>
    /// <param name="groupName">The name of the group to check. If null or empty, returns false.</param>
    /// <returns>true if the group exists; otherwise, false.</returns>
    public bool ContainsGroup(string? groupName)
    {
        if (string.IsNullOrEmpty(groupName))
        {
            return false;
        }

        var normalizedName = NormalizeGroupName(groupName);
        return _groups.ContainsKey(normalizedName);
    }

    /// <summary>
    /// Adds or merges a group into the collection. If a group with the same name exists,
    /// the values from the provided group are merged into the existing group.
    /// </summary>
    /// <param name="group">The group to add or merge.</param>
    /// <exception cref="ArgumentNullException">Thrown when group is null.</exception>
    public void AddGroup(StatisticValuesGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        var normalizedName = NormalizeGroupName(group.GroupName);

        if (_groups.TryGetValue(normalizedName, out var existingGroup))
        {
            // Merge values into existing group
            foreach (var value in group.GetStatisticValues())
            {
                existingGroup.AddValue(value);
            }
        }
        else
        {
            _groups.Add(normalizedName, group);
        }
    }

    /// <summary>
    /// Gets the group with the specified name. If the group doesn't exist and createNonExisting is true,
    /// creates a new group and adds it to the collection.
    /// </summary>
    /// <param name="groupName">The name of the group to retrieve. Group names starting with '-' have the prefix removed.</param>
    /// <param name="createNonExisting">If true, creates and adds a new group if one doesn't exist.</param>
    /// <returns>The requested group or a newly created group.</returns>
    /// <exception cref="ArgumentException">Thrown when groupName is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the group is not found and createNonExisting is false.</exception>
    public StatisticValuesGroup GetGroup(string groupName, bool createNonExisting = true)
    {
        if (string.IsNullOrEmpty(groupName))
        {
            throw new ArgumentException("Group name cannot be null or empty.", nameof(groupName));
        }

        var normalizedName = NormalizeGroupName(groupName);

        if (_groups.TryGetValue(normalizedName, out var existingGroup))
        {
            return existingGroup;
        }

        if (createNonExisting)
        {
            var group = new StatisticValuesGroup(normalizedName);
            _groups.Add(normalizedName, group);
            return group;
        }

        throw new InvalidOperationException($"Statistic group '{groupName}' was not found and createNonExisting was set to false.");
    }

    /// <summary>
    /// Attempts to get the group with the specified name without creating it if it doesn't exist.
    /// </summary>
    /// <param name="groupName">The name of the group to retrieve. Group names starting with '-' have the prefix removed.</param>
    /// <param name="group">When this method returns, contains the group associated with the specified name, if found; otherwise, null.</param>
    /// <returns>true if the group was found; otherwise, false.</returns>
    public bool TryGetGroup(string groupName, [NotNullWhen(true)] out StatisticValuesGroup? group)
    {
        if (string.IsNullOrEmpty(groupName))
        {
            group = null;
            return false;
        }

        var normalizedName = NormalizeGroupName(groupName);
        return _groups.TryGetValue(normalizedName, out group);
    }

    /// <summary>
    /// Gets all groups in the collection.
    /// </summary>
    /// <returns>A read-only collection of all statistic value groups.</returns>
    public IReadOnlyCollection<StatisticValuesGroup> GetGroups()
    {
        return _groups.Values;
    }

    /// <summary>
    /// Gets the sum of all statistic values for the specified group.
    /// </summary>
    /// <param name="groupName">The name of the group.</param>
    /// <returns>The sum of values for the specified group.</returns>
    /// <exception cref="ArgumentException">Thrown when groupName is null or empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the specified group does not exist.</exception>
    public int GetValue(string groupName)
    {
        if (string.IsNullOrEmpty(groupName))
        {
            throw new ArgumentException("Group name cannot be null or empty.", nameof(groupName));
        }

        var normalizedName = NormalizeGroupName(groupName);

        if (_groups.TryGetValue(normalizedName, out var group))
        {
            return group.Sum();
        }

        throw new InvalidOperationException($"Statistic group '{groupName}' was not found. This indicates an unresolved dependency or calculation error.");
    }

    /// <summary>
    /// Removes a group from the collection.
    /// </summary>
    /// <param name="group">The group to remove.</param>
    /// <returns>true if the group was successfully removed; otherwise, false.</returns>
    public bool Remove(StatisticValuesGroup group)
    {
        ArgumentNullException.ThrowIfNull(group);

        var normalizedName = NormalizeGroupName(group.GroupName);
        return _groups.Remove(normalizedName);
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection of groups.
    /// </summary>
    public IEnumerator<StatisticValuesGroup> GetEnumerator()
    {
        return _groups.Values.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection of groups.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Normalizes a group name by removing the leading '-' prefix if present.
    /// </summary>
    /// <param name="groupName">The group name to normalize.</param>
    /// <returns>The normalized group name.</returns>
    private static string NormalizeGroupName(string groupName)
    {
        return groupName.StartsWith('-') ? groupName[1..] : groupName;
    }
}