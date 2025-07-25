using FluentAssertions;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Hosting;

namespace Starlights.Platform.Tests;

[TestClass]
public sealed class PlatformTests
{
    [TestMethod]
    public void AddStarlightsPlatform()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act
        builder.AddStarlightsPlatform();

        // Assert
        var app = builder.Build();
        app.Should().NotBeNull();
    }
}
