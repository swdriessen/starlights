using System.Reflection;

namespace Starlights.Platform.SourceGenerators.Tests;

[TestClass]
public class GeneratedCodeInspectionTests
{
    [TestMethod]
    public void TestEntityId_ShouldBeGeneratedWithCorrectStructure()
    {
        // Arrange
        var testEntityIdType = typeof(TestEntityId);

        // Assert - check it's a value type (struct)
        Assert.IsTrue(testEntityIdType.IsValueType);

        // Assert - check it has a Value property
        var valueProperty = testEntityIdType.GetProperty("Value");
        Assert.IsNotNull(valueProperty);
        Assert.AreEqual(typeof(Guid), valueProperty.PropertyType);

        // Assert - check it has a New() method
        var newMethod = testEntityIdType.GetMethod("New", BindingFlags.Public | BindingFlags.Static);
        Assert.IsNotNull(newMethod);
        Assert.AreEqual(testEntityIdType, newMethod.ReturnType);

        // Assert - check it has implicit operator
        var implicitOperator = testEntityIdType.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "op_Implicit" && m.ReturnType == typeof(Guid));
        Assert.IsNotNull(implicitOperator);
    }

    [TestMethod]
    public void TestEntityId_ShouldHaveDebuggerDisplayAttribute()
    {
        // Arrange
        var testEntityIdType = typeof(TestEntityId);

        // Act
        var debuggerDisplayAttribute = testEntityIdType.GetCustomAttribute<System.Diagnostics.DebuggerDisplayAttribute>();

        // Assert
        Assert.IsNotNull(debuggerDisplayAttribute);
        Assert.AreEqual("{Value}", debuggerDisplayAttribute.Value);
    }
}