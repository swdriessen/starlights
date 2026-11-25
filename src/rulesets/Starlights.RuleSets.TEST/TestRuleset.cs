using Starlights.Modules.Characters.Domain.Building;

namespace Starlights.RuleSets.TEST;

/// <summary>Proves shared workflows work without D&D registration.</summary>
public sealed class TestRuleset : ICharacterRuleset
{
    /// <inheritdoc />
    public string Id => "TEST";
    /// <inheritdoc />
    public Type CompositionType => typeof(TestCharacterComposition);
    /// <inheritdoc />
    public ICharacterComposition Initialize(CharacterContentBinding content)
        => new TestCharacterComposition("initialized-by-test");
    /// <inheritdoc />
    public void Validate(ICharacterComposition composition)
    {
        if (composition is not TestCharacterComposition { Marker: "initialized-by-test" })
            throw new InvalidOperationException("TEST requires its own immutable marker part.");
    }
}
