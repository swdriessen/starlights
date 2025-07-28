namespace Starlights.Modules.Elements.Integration.Abstractions.Models;

/// <summary>
/// The DTO model for an Element in the system. Used for data transfer between modules and APIs.
/// </summary>
public record ElementModel(string Name, string Type, string Source, Guid Identifier);
