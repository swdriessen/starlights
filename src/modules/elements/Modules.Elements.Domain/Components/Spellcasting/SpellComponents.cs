using System.Diagnostics;

namespace Starlights.Modules.Elements.Domain.Components.Spellcasting;

[DebuggerDisplay("V = {HasSomatic}, S = {HasVerbal}, M = {HasMaterial}, Material = {MaterialComponent}")]
public readonly record struct SpellComponents
{
    /// <summary>
    /// Indicates whether the spell has a somatic component.
    /// </summary>
    public bool HasSomatic { get; init; }

    /// <summary>
    /// Indicates whether the spell has a verbal component.
    /// </summary>
    public bool HasVerbal { get; init; }

    /// <summary>
    /// Indicates whether the spell has a material component.
    /// </summary>
    public bool HasMaterial { get; init; }

    /// <summary>
    /// Gets the material component description, if any.
    /// </summary>
    public string? MaterialComponent { get; init => field = value?.Trim(); }
}
