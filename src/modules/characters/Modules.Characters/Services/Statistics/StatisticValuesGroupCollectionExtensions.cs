namespace Starlights.Modules.Characters.Services.Statistics;



public static class StatisticValuesGroupCollectionExtensions
{
    public static StatisticValuesGroup WithGroup(this StatisticValuesGroupCollection collection, string groupName, Action<StatisticValuesGroup>? configureGroup = null)
    {
        var group = collection.GetGroup(groupName);
        configureGroup?.Invoke(group);

        return group;
    }

    public static StatisticValue WithInternalValue(this StatisticValuesGroup group, int numericValue)
    {
        var value = new StatisticValue("Internal", numericValue);
        group.AddValue(value);
        return value;
    }

    public static StatisticValue WithValue(this StatisticValuesGroup group, int numericValue, string originName = "Internal", string? displayName = null)
    {
        var value = new StatisticValue(originName, numericValue, originName);
        group.AddValue(value);
        return value;
    }

    public static StatisticValuesGroupCollection WithGroupVariants(this StatisticValuesGroupCollection collection, string groupName)
    {
        var origingGroup = collection.GetGroup(groupName);

        collection.WithGroup($"{origingGroup.GroupName}:half", g => g.WithInternalValue(origingGroup.Sum() / 2));
        collection.WithGroup($"{origingGroup.GroupName}:half:up", g => g.WithInternalValue((int)Math.Ceiling(origingGroup.Sum() / 2.0)));
        collection.WithGroup($"{origingGroup.GroupName}:half:down", g => g.WithInternalValue((int)Math.Floor(origingGroup.Sum() / 2.0)));

        return collection;
    }
}