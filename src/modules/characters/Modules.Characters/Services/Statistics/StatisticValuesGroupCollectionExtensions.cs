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
    public static StatisticValuesGroup WithDisplayName(this StatisticValuesGroup group, string displayName)
    {
        group.DisplayName = displayName;
        return group;
    }

    public static StatisticValue WithValue(this StatisticValuesGroup group, int numericValue, string originName = "Internal", string? displayName = null)
    {
        var value = new StatisticValue(originName, numericValue, originName);
        group.AddValue(value);
        return value;
    }

    public static StatisticValuesGroupCollection WithGroupVariants(this StatisticValuesGroupCollection collection, string groupName, string? displayName = null)
    {
        var originGroup = collection.GetGroup(groupName);

        var h = collection.WithGroup($"{originGroup.GroupName}:half", g => g.WithValue(originGroup.Sum() / 2));
        var hup = collection.WithGroup($"{originGroup.GroupName}:half:up", g => g.WithInternalValue((int)Math.Ceiling(originGroup.Sum() / 2.0)));
        var hdown = collection.WithGroup($"{originGroup.GroupName}:half:down", g => g.WithInternalValue((int)Math.Floor(originGroup.Sum() / 2.0)));

        if (displayName is not null)
        {
            h.WithDisplayName(displayName);
            hup.WithDisplayName(displayName);
            hdown.WithDisplayName(displayName);
        }
        else if (originGroup.DisplayName is not null)
        {
            h.WithDisplayName(originGroup.DisplayName);
            hup.WithDisplayName(originGroup.DisplayName);
            hdown.WithDisplayName(originGroup.DisplayName);
        }

        return collection;
    }
}