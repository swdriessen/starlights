namespace Starlights.Modules.Elements.Integration.Abstractions;

public record ElementModel(string Name, string Type, string Source, Guid Identifier);

public record AbilityComponentModel(string Abbreviation);