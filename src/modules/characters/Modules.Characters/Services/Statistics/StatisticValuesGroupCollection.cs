namespace Starlights.Modules.Characters.Services.Statistics;

public class StatisticValuesGroupCollection : List<StatisticValuesGroup>
{
    public bool ContainsGroup(string groupName)
    {
        if (groupName == null)
        {
            return false;
        }

        return groupName.StartsWith("-") ? this.Any(x => x.GroupName.Equals(groupName[1..])) : this.Any(x => x.GroupName.Equals(groupName));
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

        return int.MinValue;
    }
}