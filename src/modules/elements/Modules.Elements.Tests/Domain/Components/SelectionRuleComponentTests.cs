using AwesomeAssertions;
using Starlights.Modules.Elements.Domain;
using Starlights.Modules.Elements.Domain.Components;

namespace Starlights.Modules.Elements.Tests.Domain.Components;

[TestClass]
public class SelectionRuleComponentTests
{
    [TestMethod]
    public void Constructor_ValidParameters_SetsProperties()
    {
        var owning = ElementId.New();
        var component = new SelectionRuleComponent(owning, "  type  ", "  Name  ", 2);
        component.OwningElement.Should().Be(owning);
        component.ElementType.Should().Be("type");
        component.Name.Should().Be("Name");
        component.LevelRequirement.Should().Be(2);
        component.Quantity.Should().Be(1);
        component.IsOptional.Should().BeFalse();
        component.HasRequirements.Should().BeTrue();
    }

    [TestMethod]
    public void UpdateLevelRequirement_Negative_Throws()
    {
        var component = new SelectionRuleComponent(ElementId.New(), "type", "name", 0);
        var act = () => component.UpdateLevelRequirement(-1);
        act.Should().Throw<ArgumentException>()
            .WithMessage("LevelRequirement cannot be negative. (Parameter 'levelRequirement')");
    }

    [TestMethod]
    public void UpdateQuantity_Invalid_Throws()
    {
        var component = new SelectionRuleComponent(ElementId.New(), "type", "name", 0);
        var act = () => component.UpdateQuantity(0);
        act.Should().Throw<ArgumentException>()
            .WithMessage("Quantity must be at least 1. (Parameter 'quantity')");
    }

    [TestMethod]
    public void UpdateQuantity_Valid_SetsPropertyAndIsMultiSelection()
    {
        var component = new SelectionRuleComponent(ElementId.New(), "type", "name", 0);
        component.UpdateQuantity(3);
        component.Quantity.Should().Be(3);
        component.IsMultiSelection.Should().BeTrue();
    }

    [TestMethod]
    public void UpdateOptional_SetsFlag()
    {
        var component = new SelectionRuleComponent(ElementId.New(), "type", "name", 0);
        component.UpdateIsOptional(true);
        component.IsOptional.Should().BeTrue();
    }

    [TestMethod]
    public void UpdateSupports_Empty_ClearsValues()
    {
        var component = new SelectionRuleComponent(ElementId.New(), "type", "name", 0);
        component.UpdateSupports("support1");
        component.Supports.Should().Be("support1");
        component.UpdateSupports("  ");
        component.Supports.Should().BeNull();
    }

    [TestMethod]
    public void UpdateRangeSupports_Trims()
    {
        var component = new SelectionRuleComponent(ElementId.New(), "type", "name", 0);
        component.UpdateRangeSupports("  a,b  ");
        component.RangeSupports.Should().Be("a,b");
    }

    [TestMethod]
    public void UpdateRequirements_SetsHasRequirements()
    {
        var component = new SelectionRuleComponent(ElementId.New(), "type", "name", 0);
        component.HasRequirements.Should().BeFalse();
        component.UpdateRequirements("  some  ");
        component.Requirements.Should().Be("some");
        component.HasRequirements.Should().BeTrue();
    }
}
