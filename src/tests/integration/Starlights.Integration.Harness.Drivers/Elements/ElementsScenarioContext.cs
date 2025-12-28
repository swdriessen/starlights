namespace Starlights.Integration.Drivers.Elements;

public sealed class ElementsScenarioContext
{
    public Guid LastCreated { get; private set; }

    public Dictionary<string, Guid> CreatedMap = [];

    public void ElementCreated(string name, Guid id)
    {
        CreatedMap[name] = id;
        LastCreated = id;
    }
}