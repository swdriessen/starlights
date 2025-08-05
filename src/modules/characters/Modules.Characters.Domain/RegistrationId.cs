using System.Diagnostics;

namespace Starlights.Modules.Characters.Domain;

/// <summary>
/// Represents a unique identifier for Registration entities.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct RegistrationId(Guid Value)
{
    /// <summary>
    /// Creates a new RegistrationId with a Version 7 GUID.
    /// </summary>
    public static RegistrationId New() => new(Guid.CreateVersion7());

    /// <summary>
    /// Implicitly converts RegistrationId to Guid.
    /// </summary>
    public static implicit operator Guid(RegistrationId id) => id.Value;
}

/// <summary>
/// Represents a unique identifier for Element entities in the context of the Characters module.
/// </summary>
[DebuggerDisplay("{Value}")]
public readonly record struct ElementId(Guid Value)
{
    /// <summary>
    /// Creates a new ElementId with a Version 7 GUID.
    /// </summary>
    //public static ElementId New() => new(Guid.CreateVersion7()); // TODO: check - in this case i don't want to create new element id's, maybe only in tests... so lets not put it here

    /// <summary>
    /// Implicitly converts ElementId to Guid.
    /// </summary>
    public static implicit operator Guid(ElementId id) => id.Value;
}