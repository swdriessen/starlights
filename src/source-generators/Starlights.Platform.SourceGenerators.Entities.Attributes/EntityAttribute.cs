namespace Starlights.Platform.SourceGenerators.Entities.Attributes;

/// <summary>
/// Marks a class as an entity for automatic EntityId generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class EntityAttribute : Attribute;