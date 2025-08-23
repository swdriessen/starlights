namespace Starlights.Platform.SourceGenerators.Tests;

[TestClass]
public class EntityIdGenerationTests
{
    [TestMethod]
    public void TestEntityId_ShouldBeGenerated()
    {
        // Arrange & Act
        var testEntityId = TestEntityId.New();

        // Assert
        Assert.AreNotEqual(default, testEntityId);
        Assert.AreNotEqual(Guid.Empty, testEntityId.Value);
    }

    [TestMethod]
    public void TestEntityId_ShouldHaveImplicitGuidConversion()
    {
        // Arrange
        var testEntityId = TestEntityId.New();

        // Act
        Guid guidValue = testEntityId; // Implicit conversion

        // Assert
        Assert.AreEqual(testEntityId.Value, guidValue);
    }

    [TestMethod]
    public void TestEntityId_NewInstancesShouldBeUnique()
    {
        // Arrange & Act
        var id1 = TestEntityId.New();
        var id2 = TestEntityId.New();

        // Assert
        Assert.AreNotEqual(id1, id2);
        Assert.AreNotEqual(id1.Value, id2.Value);
    }
}