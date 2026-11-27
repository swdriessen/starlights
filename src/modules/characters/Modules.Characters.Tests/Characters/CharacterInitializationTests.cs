using AwesomeAssertions;
using Starlights.Modules.Characters.Building;
using Starlights.Modules.Characters.Domain.Building;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.RuleSets.DND5E5.Building;
using Starlights.RuleSets.DND5E5.Rules;
using Starlights.RuleSets.TEST;

namespace Starlights.Modules.Characters.Tests.Characters;

[TestClass]
public sealed class CharacterInitializationTests
{
    private static readonly CharacterContentBinding Content = new("test", "1.0.0", "test:character");

    [TestMethod]
    [DataRow(null, "The Nameless One")]
    [DataRow("", "The Nameless One")]
    [DataRow("  ", "The Nameless One")]
    [DataRow("Mira", "Mira")]
    public void Create_InitializesAnIncompleteOwnedCharacter(string? name, string expected)
    {
        var ruleset = new Ruleset();
        var owner = Guid.NewGuid();
        var build = new CharacterBuild(ruleset, Content, ruleset.Initialize(Content));
        var character = Character.Create(owner, name, build);

        character.Name.Should().Be(expected);
        character.OwnerId.Should().Be(owner);
        character.RulesetId.Should().Be("DND5E5");
        character.Components.Should().BeEmpty();
        character.Build!.Composition.Should().BeOfType<CharacterComposition>()
            .Which.Build.Stage.Should().Be("awaiting-choices");
    }

    [TestMethod]
    public void Create_BoundsNamesAndDoesNotUseNamesAsIdentity()
    {
        var ruleset = new TestRuleset();
        var build = new CharacterBuild(ruleset, Content, ruleset.Initialize(Content));
        var owner = Guid.NewGuid();
        Character.Create(owner, new string('x', 100), build).Name.Should().HaveLength(100);
        var invalid = () => Character.Create(owner, new string('x', 101), build);
        invalid.Should().Throw<ArgumentException>();
        Character.Create(owner, "Mira", build).Id.Should().NotBe(Character.Create(owner, "Mira", build).Id);
        var ownerless = () => Character.Create(Guid.Empty, "Mira", build);
        ownerless.Should().Throw<ArgumentException>();
    }

    [TestMethod]
    public void Composition_RejectsMissingAndIncompatibleStateWithoutChangingAnExistingBuild()
    {
        var ruleset = new Ruleset();
        var composition = new CharacterComposition(new BuildPart("awaiting-choices"));
        var character = Character.Create(Guid.NewGuid(), "Mira", new(ruleset, Content, composition));
        var incompatible = () => new CharacterBuild(ruleset, Content, new TestCharacterComposition("initialized-by-test"));
        incompatible.Should().Throw<InvalidOperationException>();
        var missing = () => new CharacterComposition(null!);
        missing.Should().Throw<ArgumentException>();
        character.Build!.Composition.Should().BeSameAs(composition);
        character.Components.Should().BeEmpty();
    }

    [TestMethod]
    [DataRow("DND5E5")]
    [DataRow("TEST")]
    [DataRow("both")]
    [DataRow("reversed")]
    public void Rulesets_WorkAloneAndInEitherRegistrationOrder(string configuration)
    {
        ICharacterRuleset[] modules = configuration switch
        {
            "DND5E5" => [new Ruleset()],
            "TEST" => [new TestRuleset()],
            "both" => [new Ruleset(), new TestRuleset()],
            _ => [new TestRuleset(), new Ruleset()]
        };
        var registry = new CharacterRulesets(modules);
        foreach (var module in modules)
        {
            var resolved = registry.GetRequired(module.Id.ToLowerInvariant());
            resolved.Should().BeSameAs(module);
            var build = new CharacterBuild(resolved, Content, resolved.Initialize(Content));
            build.RulesetId.Should().Be(module.Id);
            build.Composition.GetType().Should().Be(module.CompositionType);
        }
    }

    [TestMethod]
    public void Rulesets_RejectMissingDuplicateAndCaseOnlyRegistrations()
    {
        var missing = () => new CharacterRulesets([]).GetRequired("TEST");
        missing.Should().Throw<InvalidOperationException>();
        var duplicate = () => new CharacterRulesets([new TestRuleset(), new TestRuleset()]);
        duplicate.Should().Throw<InvalidOperationException>();
        var casing = () => new CharacterRulesets([new TestRuleset(), new LowercaseTestRuleset()]);
        casing.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void Catalog_LoadsAndPinsContentInsteadOfRebindingToANewerPack()
    {
        var registry = new CharacterRulesets([new Ruleset()]);
        var catalog = new CharacterContentCatalog();
        catalog.AddEmbeddedPack(typeof(Ruleset).Assembly, "Starlights.RuleSets.DND5E5.Content.srd-5.2.1.json", registry);
        var option = catalog.Options.Should().ContainSingle().Subject;
        option.Content.Should().Be(new CharacterContentBinding("srd-5.2.1", "0.1.0", "srd521:character:default"));
        option.Definition.GetProperty("rules").GetArrayLength().Should().Be(0);
        var unavailable = () => catalog.GetRequired("DND5E5", new("srd-5.2.1", "0.2.0", "srd521:character:default"));
        unavailable.Should().Throw<InvalidOperationException>();
        var duplicate = () => catalog.AddEmbeddedPack(typeof(Ruleset).Assembly, "Starlights.RuleSets.DND5E5.Content.srd-5.2.1.json", registry);
        duplicate.Should().Throw<InvalidOperationException>();
        catalog.Options.Should().ContainSingle();
    }

    [TestMethod]
    [DataRow("latest")]
    [DataRow("1.0")]
    [DataRow("01.0.0")]
    public void ContentBinding_RejectsNonPinnedVersions(string version)
    {
        var invalid = () => new CharacterContentBinding("test", version, "test:character");
        invalid.Should().Throw<ArgumentException>();
    }

    private sealed class LowercaseTestRuleset : ICharacterRuleset
    {
        public string Id => "test";
        public Type CompositionType => typeof(TestCharacterComposition);
        public ICharacterComposition Initialize(CharacterContentBinding content) => new TestCharacterComposition("initialized-by-test");
        public void Validate(ICharacterComposition composition) => throw new NotSupportedException();
    }
}
