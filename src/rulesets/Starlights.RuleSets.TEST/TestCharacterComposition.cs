using Starlights.Modules.Characters.Domain.Building;

namespace Starlights.RuleSets.TEST;

/// <summary>A synthetic immutable part with no D&D state or dependency.</summary>
public sealed record TestCharacterComposition : ICharacterComposition
{
    /// <summary>Creates the deterministic test state.</summary>
    public TestCharacterComposition(string marker)
    {
        if (marker != "initialized-by-test")
            throw new ArgumentException("The test marker is invalid.", nameof(marker));
        Marker = marker;
    }

    /// <summary>Gets the value used to verify durable reconstitution.</summary>
    public string Marker { get; }
}
