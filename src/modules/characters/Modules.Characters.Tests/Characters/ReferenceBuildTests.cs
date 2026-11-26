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
