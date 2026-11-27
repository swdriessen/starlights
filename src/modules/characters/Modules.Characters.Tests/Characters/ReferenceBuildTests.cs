using System.Text.Json;
using AwesomeAssertions;
using Starlights.Modules.Characters.Domain.Building;
using Starlights.Modules.Characters.Domain.Characters;
using Starlights.RuleSets.DND5E5;

namespace Starlights.Modules.Characters.Tests.Characters;

[TestClass]
public sealed class ReferenceBuildTests
{
    private static readonly CharacterContentBinding Content = new("srd-5.2.1", "0.2.0", "srd521:character:default");

    [TestMethod]
    public void RejectedNamedOperations_LeaveTheAggregateUnchanged_AndFactsCannotBeMutated()
    {
        var rules = Rules();
        var character = Character.Create(Guid.NewGuid(), "Original", new(rules, Content, rules.Initialize(Content)));
        var original = character.Build;
        var invalid = () => character.RecordChoice(rules, "not-offered", ["srd521:class:fighter"]);
        invalid.Should().Throw<ArgumentException>();
        character.Build.Should().BeSameAs(original);
        var badArray = () => character.AssignAbilityScores(rules, new Dictionary<string,int> { ["strength"] = 15, ["dexterity"] = 15 });
        badArray.Should().Throw<ArgumentException>();
        character.Build.Should().BeSameAs(original);
        var badEquipment = () => character.Equip(rules, "unowned", null);
        badEquipment.Should().Throw<ArgumentException>();
        character.Build.Should().BeSameAs(original);
        var inputs = new Dictionary<string,int> { ["strength"] = 15 };
        character.AssignAbilityScores(rules, inputs);
        inputs["strength"] = 8;
        DndBuildRules.Facts(character.Build!.Composition).Inputs["strength"].Should().Be(15);
    }

    [TestMethod]
    public void Calculation_ReadsAuthoredContent_NotReferenceConstants()
    {
        var json = Json().Replace("\"value\":30", "\"value\":35", StringComparison.Ordinal);
        using var document = JsonDocument.Parse(json);
        var rules = new DndBuildRules(new(document.RootElement, Content));
        var character = Character.Create(Guid.NewGuid(), "Changed content", new(rules, Content, rules.Initialize(Content)));
        character.RecordChoice(rules, Content.CharacterElementId + "/species", ["srd521:species:human"]);
        rules.View(character.Build!.Composition).Statistics.Single(statistic => statistic.Id == "speed").Value.Should().Be(35);
    }

    [TestMethod]
    public void NumericExpressions_AreBounded_AndUnresolvedInputsDoNotBecomeZero()
    {
        DndNumericExpression.Parse("floor((17 - 10) / 2)").Evaluate(_ => null).Should().Be(3);
        DndNumericExpression.Parse("missing.score + 2").Evaluate(_ => null).Should().BeNull();
        DndNumericExpression.Parse("5 / 0").Evaluate(_ => null).Should().BeNull();
        var script = () => DndNumericExpression.Parse("System.IO.File.Delete('secret')");
        script.Should().Throw<InvalidOperationException>();
    }

    [TestMethod]
    public void Content_RejectsUnknownRequiredContractsAndMissingReferences()
    {
        foreach (var json in new[]
        {
            Json().Replace("\"kind\":\"armor\"", "\"kind\":\"unsupported\"", StringComparison.Ordinal),
            Json().Replace("\"target\":\"srd521:ability:strength\"", "\"target\":\"missing:ability:strength\"", StringComparison.Ordinal),
            Json().Replace("\"kind\":\"grant\"", "\"kind\":\"unsupported\"", StringComparison.Ordinal),
            Json().Replace("\"value\":30", "\"value\":\"unknown.statistic\"", StringComparison.Ordinal),
            Json().Replace("Gain Heroic Inspiration after a Long Rest.", "<script>alert(1)</script>", StringComparison.Ordinal),
            Json().Replace("Gain Heroic Inspiration after a Long Rest.", "![external](https://example.invalid/image)", StringComparison.Ordinal),
            Json().Replace("Gain Heroic Inspiration after a Long Rest.", "::: sidebar Unsupported", StringComparison.Ordinal)
        })
        {
            using var document = JsonDocument.Parse(json);
            var parse = () => new DndContentPack(document.RootElement, Content);
            parse.Should().Throw<InvalidOperationException>();
        }
    }

    [TestMethod]
    public void LaterGrant_RetainsButDoesNotApplyANowIneligibleSelectionAnswer()
    {
        var rules = Rules();
        var character = Character.Create(Guid.NewGuid(), "Retained intent", new(rules, Content, rules.Initialize(Content)));
        character.RecordChoice(rules, Content.CharacterElementId + "/species", ["srd521:species:human"]);
        var featSelection = Content.CharacterElementId + "/species/srd521:species:human/versatile";
        character.RecordChoice(rules, featSelection, ["srd521:origin-feat:savage-attacker"]);
        character.RecordChoice(rules, Content.CharacterElementId + "/background", ["srd521:background:soldier"]);
        var view = rules.View(character.Build!.Composition);
        var retained = view.Selections.Single(selection => selection.Id == featSelection);
        retained.Answers.Should().Equal("srd521:origin-feat:savage-attacker");
        retained.Options.Should().NotContain(option => option.Id == "srd521:origin-feat:savage-attacker");
        view.Entries.Should().ContainSingle(entry => entry.Name == "Savage Attacker");
        view.BuildComplete.Should().BeFalse();
    }

    private static DndBuildRules Rules()
    {
        using var document = JsonDocument.Parse(Json());
        return new(new(document.RootElement, Content));
    }

    [TestMethod]
    public void AbilityGeneration_InitializesAssignmentsOnlyAtCreationOrMethodReplacement()
    {
        var rules = Rules();
        var character = Character.Create(Guid.NewGuid(), "Initial assignments", new(rules, Content, rules.Initialize(Content)));
        var initial = rules.View(character.Build!.Composition);
        initial.Abilities.Select(ability => ability.Id).Should().Equal("strength", "dexterity", "constitution", "intelligence", "wisdom", "charisma");
        initial.Abilities.Select(ability => ability.Input).Should().Equal(15, 14, 13, 12, 10, 8);
        initial.Abilities.Should().OnlyContain(ability => !ability.IsFallback && ability.Issue == null);
        initial.BuildComplete.Should().BeFalse();
        character.AssignAbilityScores(rules, new Dictionary<string, int> { ["strength"] = 8 });
        character.ChangeAbilityGeneration(rules, "standard-array", false);
        DndBuildRules.Facts(character.Build.Composition).Inputs.Should().BeEquivalentTo(new Dictionary<string, int> { ["strength"] = 8 });
        character.ChangeAbilityGeneration(rules, "point-buy", false);
        var pointBuy = rules.View(character.Build.Composition);
        pointBuy.Abilities.Select(ability => ability.Input).Should().Equal(8, 8, 8, 8, 8, 8);
        pointBuy.PointsSpent.Should().Be(0);
        character.ChangeAbilityGeneration(rules, "physical-dice", false);
        DndBuildRules.Facts(character.Build.Composition).Inputs.Should().BeEmpty();
        character.ChangeAbilityGeneration(rules, "standard-array", false);
        rules.View(character.Build.Composition).Abilities.Select(ability => ability.Input).Should().Equal(15, 14, 13, 12, 10, 8);
        var legacy = new DndCharacterComposition(new("awaiting-choices"), DndBuildFacts.Empty);
        rules.View(legacy).Abilities.Should().OnlyContain(ability => ability.Input == null && ability.IsFallback);
        rules.ChangeAbilityGeneration(legacy, "standard-array", false).Should().BeSameAs(legacy);
    }

    [TestMethod]
    [DataRow(8, 0, -1)]
    [DataRow(9, 1, -1)]
    [DataRow(10, 2, 0)]
    [DataRow(11, 3, 0)]
    [DataRow(12, 4, 1)]
    [DataRow(13, 5, 1)]
    [DataRow(14, 7, 2)]
    [DataRow(15, 9, 2)]
    public void PointBuy_UsesSrdCostsAndModifiers(int input, int cost, int modifier)
    {
        var rules = Rules();
        var character = Character.Create(Guid.NewGuid(), "Point buy", new(rules, Content, rules.Initialize(Content)));
        character.ChangeAbilityGeneration(rules, "point-buy", false);
        character.AssignAbilityScores(rules, new Dictionary<string, int> { ["strength"] = input });
        var view = rules.View(character.Build!.Composition);
        view.PointsSpent.Should().Be(cost);
        view.Abilities.Single(ability => ability.Id == "strength").Modifier.Should().Be(modifier);
    }

    [TestMethod]
    public void RolledAllocation_RespectsDuplicateMultiplicity_AndRejectedGenerationIsAtomic()
    {
        var rules = Rules();
        DndAbilityRoll[] rolls = [new([1, 1, 1, 1]), new([6, 6, 6, 6]), new([1, 6, 6, 6]),
            new([2, 3, 4, 5]), new([2, 2, 2, 2]), new([3, 3, 3, 3])];
        var facts = new DndBuildFacts(new Dictionary<string, IReadOnlyList<string>>(), new Dictionary<string, int>(), [],
            null, null, new("application-rolls", rolls));
        var character = Character.Create(Guid.NewGuid(), "Rolls", new(rules, Content, new DndCharacterComposition(new("awaiting-choices"), facts)));
        character.AssignAbilityScores(rules, new Dictionary<string, int> { ["strength"] = 18, ["dexterity"] = 18, ["constitution"] = 3,
            ["intelligence"] = 12, ["wisdom"] = 6, ["charisma"] = 9 });
        rules.View(character.Build!.Composition).Abilities.Select(ability => ability.Modifier).Should().Equal(4, 4, -4, 1, -2, -1);
        var before = character.Build;
        var excessive = () => character.AssignAbilityScores(rules, new Dictionary<string, int>
            { ["strength"] = 18, ["dexterity"] = 18, ["constitution"] = 18 });
        excessive.Should().Throw<ArgumentException>();
        character.Build.Should().BeSameAs(before);
        var badMethod = () => character.ChangeAbilityGeneration(rules, "unknown", false);
        badMethod.Should().Throw<ArgumentException>();
        var invalidReroll = () => character.ChangeAbilityGeneration(rules, "point-buy", true);
        invalidReroll.Should().Throw<ArgumentException>();
        character.Build.Should().BeSameAs(before);
        character.ChangeAbilityGeneration(rules, "physical-dice", false);
        character.ChangeAbilityGeneration(rules, "application-rolls", false);
        rules.View(character.Build!.Composition).Abilities.Select(ability => ability.Input).Should().Equal(3, 18, 18, 12, 6, 9);
        DndBuildRules.Facts(character.Build.Composition).AbilityGeneration.Rolls.Should().BeEquivalentTo(rolls);
        character.ChangeAbilityGeneration(rules, "application-rolls", true);
        var rerolled = DndBuildRules.Facts(character.Build.Composition);
        rules.View(character.Build.Composition).Abilities.Select(ability => ability.Input)
            .Should().Equal(rerolled.AbilityGeneration.Rolls.Select(roll => (int?)roll.Total));
    }

    [TestMethod]
    public void AbilityFallback_IdentifiesAbsentDefaultsZeroAndInvalidRecordedInputs()
    {
        var json = Json().Replace("\"prefix\":\"strength\"", "\"prefix\":\"strength\",\"defaultInput\":8", StringComparison.Ordinal);
        using var document = JsonDocument.Parse(json);
        var rules = new DndBuildRules(new(document.RootElement, Content));
        var view = rules.View(new DndCharacterComposition(new("awaiting-choices"), DndBuildFacts.Empty));
        var strength = view.Abilities.Single(ability => ability.Id == "strength");
        strength.Input.Should().BeNull();
        strength.Score.Should().Be(8);
        strength.Modifier.Should().Be(-1);
        strength.IsFallback.Should().BeTrue();
        strength.Issue.Should().Contain("missing");
        strength.Sources.Should().Contain(source => source.Contains("declared-default", StringComparison.Ordinal));
        view.Abilities.Single(ability => ability.Id == "dexterity").Score.Should().Be(0);
        view.BuildComplete.Should().BeFalse();
        view.SheetReady.Should().BeTrue();
        var invalidFacts = new DndBuildFacts(new Dictionary<string, IReadOnlyList<string>>(),
            new Dictionary<string, int> { ["strength"] = 0 }, [], null, null);
        var invalid = rules.View(new DndCharacterComposition(new("awaiting-choices"), invalidFacts)).Abilities.Single(ability => ability.Id == "strength");
        invalid.Input.Should().Be(0);
        invalid.IsFallback.Should().BeFalse();
        invalid.Score.Should().BeNull();
        invalid.Issue.Should().NotBeNull();
    }

    [TestMethod]
    public void AbilityMaximum_LimitsEffectiveScoreAndConsumersWithoutChangingInputOrIncreases()
    {
        var rules = Rules();
        var character = Character.Create(Guid.NewGuid(), "Limits", new(rules, Content, rules.Initialize(Content)));
        character.ChangeAbilityGeneration(rules, "physical-dice", false);
        character.AssignAbilityScores(rules, new Dictionary<string, int> { ["strength"] = 18, ["constitution"] = 18 });
        character.RecordChoice(rules, Content.CharacterElementId + "/background", ["srd521:background:soldier"]);
        character.RecordChoice(rules, Content.CharacterElementId + "/background/srd521:background:soldier/ability-increases", ["srd521:soldier-increases:str-con"]);
        var strength = rules.View(character.Build!.Composition).Abilities.Single(ability => ability.Id == "strength");
        strength.Input.Should().Be(18);
        strength.OrdinaryScore.Should().Be(20);
        strength.Score.Should().Be(20);
        strength.Modifier.Should().Be(5);
        var limitedJson = Json().Replace("\"prefix\":\"strength\"", "\"prefix\":\"strength\",\"maximum\":19", StringComparison.Ordinal);
        using var document = JsonDocument.Parse(limitedJson);
        var limitedRules = new DndBuildRules(new(document.RootElement, Content));
        var limitedView = limitedRules.View(character.Build.Composition);
        var limited = limitedView.Abilities.Single(ability => ability.Id == "strength");
        limited.Input.Should().Be(18);
        limited.OrdinaryScore.Should().Be(20);
        limited.Score.Should().Be(19);
        limited.Modifier.Should().Be(4);
        limited.Sources.Should().Contain(source => source.Contains("str-con", StringComparison.Ordinal));
        limited.Sources.Should().Contain("DND5E5/maximum/19");
        limitedView.Statistics.Single(statistic => statistic.Id == "athletics").Value.Should().Be(4);
    }

    [TestMethod]
    public void UnansweredSelections_DoNotInventProficiencyOrMakeResolvedInputsUnavailable()
    {
        var rules = Rules();
        var character = Character.Create(Guid.NewGuid(), "Incomplete", new(rules, Content, rules.Initialize(Content)));
        character.AssignAbilityScores(rules, new Dictionary<string,int>
        { ["strength"] = 15, ["dexterity"] = 14, ["constitution"] = 13, ["intelligence"] = 8, ["wisdom"] = 10, ["charisma"] = 12 });
        var view = rules.View(character.Build!.Composition);
        view.BuildComplete.Should().BeFalse();
        view.SheetReady.Should().BeTrue();
        view.Statistics.Single(statistic => statistic.Id == "perception").Value.Should().Be(0);
        view.Statistics.Single(statistic => statistic.Id == "passive-perception").Value.Should().Be(10);
    }
    private static string Json()
    {
        using var stream = typeof(DndRuleset).Assembly.GetManifestResourceStream("Starlights.RuleSets.DND5E5.Content.srd-5.2.1-reference.json")!;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
