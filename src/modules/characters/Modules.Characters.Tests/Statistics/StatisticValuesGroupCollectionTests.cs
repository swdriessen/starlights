using FluentAssertions;
using Starlights.Modules.Characters.Services.Statistics;

namespace Starlights.Modules.Characters.Tests.Statistics;

[TestClass]
public sealed class StatisticValuesGroupCollectionTests
{
    #region Basic Operations

    [TestMethod]
    public void Constructor_ShouldCreateEmptyCollection()
    {
        // Arrange & Act
        var collection = new StatisticValuesGroupCollection();

        // Assert
        collection.Count.Should().Be(0);
    }

    [TestMethod]
    public void GetGroup_WithCreateNonExisting_ShouldCreateAndAddGroup()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var group = collection.GetGroup("test-group");

        // Assert
        group.Should().NotBeNull();
        group.GroupName.Should().Be("test-group");
        collection.Count.Should().Be(1);
        collection.ContainsGroup("test-group").Should().BeTrue();
    }

    [TestMethod]
    public void GetGroup_WithoutCreateNonExisting_ShouldThrowWhenNotFound()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var act = () => collection.GetGroup("nonexistent", createNonExisting: false);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*nonexistent*not found*");
    }

    [TestMethod]
    public void GetGroup_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var act = () => collection.GetGroup(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
 .WithParameterName("groupName");
    }

    [TestMethod]
    public void ContainsGroup_WithExistingGroup_ShouldReturnTrue()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        collection.GetGroup("test-group");

        // Act
        var result = collection.ContainsGroup("test-group");

        // Assert
        result.Should().BeTrue();
    }

    [TestMethod]
    public void ContainsGroup_WithNonExistingGroup_ShouldReturnFalse()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var result = collection.ContainsGroup("nonexistent");

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void ContainsGroup_WithNullOrEmptyName_ShouldReturnFalse()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act & Assert
        collection.ContainsGroup(null).Should().BeFalse();
        collection.ContainsGroup(string.Empty).Should().BeFalse();
    }

    #endregion

    #region Name Normalization

    [TestMethod]
    public void GetGroup_WithDashPrefix_ShouldNormalizeName()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var group = collection.GetGroup("-test-group");

        // Assert
        group.Should().NotBeNull();
        group.GroupName.Should().Be("test-group");
    }

    [TestMethod]
    public void ContainsGroup_WithDashPrefix_ShouldMatchNormalizedName()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        collection.GetGroup("test-group");

        // Act & Assert
        collection.ContainsGroup("-test-group").Should().BeTrue();
    }

    [TestMethod]
    public void ContainsGroup_IsCaseInsensitive()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        collection.GetGroup("Test-Group");

        // Act & Assert
        collection.ContainsGroup("test-group").Should().BeTrue();
        collection.ContainsGroup("TEST-GROUP").Should().BeTrue();
        collection.ContainsGroup("TeSt-GrOuP").Should().BeTrue();
    }

    [TestMethod]
    public void GetGroup_IsCaseInsensitive_ShouldReturnSameGroup()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        var original = collection.GetGroup("Test-Group");

        // Act
        var lowercase = collection.GetGroup("test-group");
        var uppercase = collection.GetGroup("TEST-GROUP");

        // Assert
        lowercase.Should().BeSameAs(original);
        uppercase.Should().BeSameAs(original);
        collection.Count.Should().Be(1);
    }

    #endregion

    #region AddGroup

    [TestMethod]
    public void AddGroup_WithNewGroup_ShouldAddToCollection()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        var group = new StatisticValuesGroup("test-group");

        // Act
        collection.AddGroup(group);

        // Assert
        collection.Count.Should().Be(1);
        collection.ContainsGroup("test-group").Should().BeTrue();
    }

    [TestMethod]
    public void AddGroup_WithExistingGroup_ShouldMergeValues()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        var existing = collection.GetGroup("test-group");
        existing.AddValue("source1", 5);

        var newGroup = new StatisticValuesGroup("test-group");
        newGroup.AddValue("source2", 3);

        // Act
        collection.AddGroup(newGroup);

        // Assert
        collection.Count.Should().Be(1);
        var merged = collection.GetGroup("test-group");
        merged.Sum().Should().Be(8);
        merged.GetStatisticValues().Should().HaveCount(2);
    }

    [TestMethod]
    public void AddGroup_WithNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var act = () => collection.AddGroup(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region GetValue

    [TestMethod]
    public void GetValue_WithExistingGroup_ShouldReturnSum()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        var group = collection.GetGroup("test-group");
        group.AddValue("source1", 5);
        group.AddValue("source2", 3);

        // Act
        var result = collection.GetValue("test-group");

        // Assert
        result.Should().Be(8);
    }

    [TestMethod]
    public void GetValue_WithNonExistingGroup_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var act = () => collection.GetValue("nonexistent");

        // Assert
        act.Should().Throw<InvalidOperationException>()
     .WithMessage("*nonexistent*not found*");
    }

    [TestMethod]
    public void GetValue_WithEmptyName_ShouldThrowArgumentException()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var act = () => collection.GetValue(string.Empty);

        // Assert
        act.Should().Throw<ArgumentException>()
.WithParameterName("groupName");
    }

    [TestMethod]
    public void GetValue_WithDashPrefix_ShouldNormalizeAndReturnValue()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        var group = collection.GetGroup("test-group");
        group.AddValue("source", 10);

        // Act
        var result = collection.GetValue("-test-group");

        // Assert
        result.Should().Be(10);
    }

    #endregion

    #region Remove

    [TestMethod]
    public void Remove_WithExistingGroup_ShouldRemoveAndReturnTrue()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        var group = collection.GetGroup("test-group");

        // Act
        var result = collection.Remove(group);

        // Assert
        result.Should().BeTrue();
        collection.Count.Should().Be(0);
        collection.ContainsGroup("test-group").Should().BeFalse();
    }

    [TestMethod]
    public void Remove_WithNonExistingGroup_ShouldReturnFalse()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        var group = new StatisticValuesGroup("nonexistent");

        // Act
        var result = collection.Remove(group);

        // Assert
        result.Should().BeFalse();
    }

    [TestMethod]
    public void Remove_WithNull_ShouldThrowArgumentNullException()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var act = () => collection.Remove(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    #endregion

    #region GetGroups

    [TestMethod]
    public void GetGroups_ShouldReturnAllGroups()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        collection.GetGroup("group1");
        collection.GetGroup("group2");
        collection.GetGroup("group3");

        // Act
        var groups = collection.GetGroups();

        // Assert
        groups.Should().HaveCount(3);
        groups.Should().Contain(g => g.GroupName == "group1");
        groups.Should().Contain(g => g.GroupName == "group2");
        groups.Should().Contain(g => g.GroupName == "group3");
    }

    [TestMethod]
    public void GetGroups_WithEmptyCollection_ShouldReturnEmpty()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var groups = collection.GetGroups();

        // Assert
        groups.Should().BeEmpty();
    }

    [TestMethod]
    public void GetGroups_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        collection.GetGroup("test");

        // Act
        var groups = collection.GetGroups();

        // Assert
        groups.Should().BeAssignableTo<IReadOnlyCollection<StatisticValuesGroup>>();
    }

    [TestMethod]
    public void GetGroups_CanBeUsedWithLinq()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        collection.GetGroup("completed-1").Complete();
        collection.GetGroup("incomplete-1");
        collection.GetGroup("completed-2").Complete();

        // Act
        var completed = collection.GetGroups().Where(g => g.IsCompleted).ToList();

        // Assert
        completed.Should().HaveCount(2);
        completed.Should().Contain(g => g.GroupName == "completed-1");
        completed.Should().Contain(g => g.GroupName == "completed-2");
    }

    #endregion

    #region IEnumerable Support

    [TestMethod]
    public void Enumeration_ShouldIterateAllGroups()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        collection.GetGroup("group1");
        collection.GetGroup("group2");
        collection.GetGroup("group3");

        // Act
        var groups = collection.ToList();

        // Assert
        groups.Should().HaveCount(3);
        groups.Should().Contain(g => g.GroupName == "group1");
        groups.Should().Contain(g => g.GroupName == "group2");
        groups.Should().Contain(g => g.GroupName == "group3");
    }

    [TestMethod]
    public void Foreach_ShouldIterateAllGroups()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        collection.GetGroup("group1");
        collection.GetGroup("group2");

        var count = 0;
        var names = new List<string>();

        // Act
        foreach (var group in collection)
        {
            count++;
            names.Add(group.GroupName);
        }

        // Assert
        count.Should().Be(2);
        names.Should().Contain("group1");
        names.Should().Contain("group2");
    }

    [TestMethod]
    public void Linq_ShouldWorkDirectly()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        collection.GetGroup("completed-1").Complete();
        collection.GetGroup("incomplete-1");
        collection.GetGroup("completed-2").Complete();

        // Act
        var completed = collection.Where(g => g.IsCompleted).ToList();

        // Assert
        completed.Should().HaveCount(2);
        completed.Should().Contain(g => g.GroupName == "completed-1");
        completed.Should().Contain(g => g.GroupName == "completed-2");
    }

    #endregion

    #region Performance & Edge Cases

    [TestMethod]
    public void MultipleOperations_ShouldMaintainConsistency()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var group1 = collection.GetGroup("strength");
        group1.AddValue("base", 10);

        var group2 = collection.GetGroup("STRENGTH"); // case insensitive
        group2.AddValue("bonus", 2);

        var group3 = collection.GetGroup("-strength"); // dash prefix
        group3.AddValue("temp", 1);

        // Assert
        collection.Count.Should().Be(1);
        var final = collection.GetGroup("strength");
        final.GetStatisticValues().Should().HaveCount(3);
        final.Sum().Should().Be(13);
    }

    [TestMethod]
    public void LargeNumberOfGroups_ShouldHandleEfficiently()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        for (int i = 0; i < 1000; i++)
        {
            var group = collection.GetGroup($"group-{i}");
            group.AddValue("test", i);
        }

        // Assert
        collection.Count.Should().Be(1000);
        collection.GetValue("group-500").Should().Be(500);
        collection.ContainsGroup("group-999").Should().BeTrue();
    }

    #endregion

    #region TryGetGroup

    [TestMethod]
    public void TryGetGroup_WithExistingGroup_ShouldReturnTrueAndGroup()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        var expected = collection.GetGroup("test-group");

        // Act
        var result = collection.TryGetGroup("test-group", out var group);

        // Assert
        result.Should().BeTrue();
        group.Should().NotBeNull();
        group.Should().BeSameAs(expected);
        group!.GroupName.Should().Be("test-group");
    }

    [TestMethod]
    public void TryGetGroup_WithNonExistingGroup_ShouldReturnFalseAndNull()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var result = collection.TryGetGroup("nonexistent", out var group);

        // Assert
        result.Should().BeFalse();
        group.Should().BeNull();
    }

    [TestMethod]
    public void TryGetGroup_WithNullOrEmptyName_ShouldReturnFalseAndNull()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act & Assert
        collection.TryGetGroup(null!, out var group1).Should().BeFalse();
        group1.Should().BeNull();

        collection.TryGetGroup(string.Empty, out var group2).Should().BeFalse();
        group2.Should().BeNull();
    }

    [TestMethod]
    public void TryGetGroup_WithDashPrefix_ShouldNormalizeAndFind()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        var expected = collection.GetGroup("test-group");

        // Act
        var result = collection.TryGetGroup("-test-group", out var group);

        // Assert
        result.Should().BeTrue();
        group.Should().BeSameAs(expected);
    }

    [TestMethod]
    public void TryGetGroup_IsCaseInsensitive()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();
        var expected = collection.GetGroup("Test-Group");

        // Act & Assert
        collection.TryGetGroup("test-group", out var lowercase).Should().BeTrue();
        lowercase.Should().BeSameAs(expected);

        collection.TryGetGroup("TEST-GROUP", out var uppercase).Should().BeTrue();
        uppercase.Should().BeSameAs(expected);

        collection.TryGetGroup("TeSt-GrOuP", out var mixed).Should().BeTrue();
        mixed.Should().BeSameAs(expected);
    }

    [TestMethod]
    public void TryGetGroup_DoesNotCreateGroup()
    {
        // Arrange
        var collection = new StatisticValuesGroupCollection();

        // Act
        var result = collection.TryGetGroup("nonexistent", out _);

        // Assert
        result.Should().BeFalse();
        collection.Count.Should().Be(0);
        collection.ContainsGroup("nonexistent").Should().BeFalse();
    }

    #endregion
}
