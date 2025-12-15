using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Starlights.Platform.Hosting;
using Starlights.Platform.Tests.Helpers;

namespace Starlights.Platform.Tests;

[TestClass]
public sealed class PlatformTests
{
    private readonly Mock<IEmptyVerificationService> _mockService = new();

    [TestMethod]
    public void AddStarlightsPlatform()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act
        builder.AddStarlightsPlatform();

        // Assert
        var app = builder.Build();
        var emptyModule = app.Services.GetService<EmptyModule>();
        emptyModule.Should().NotBeNull();
    }

    [TestMethod]
    public void AddStarlightsPlatform_DisableModuleDiscovery()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act
        builder.AddStarlightsPlatform(options => options.IsDiscoveryEnabled = false);

        // Assert
        var app = builder.Build();
        var emptyModule = app.Services.GetService<EmptyModule>();
        emptyModule.Should().BeNull();
    }

    [TestMethod]
    public void AddStarlightsPlatform_ShouldConfigurePlatformModule()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act
        builder.AddStarlightsPlatform();

        // Assert
        var app = builder.Build();
        var emptyService = app.Services.GetService<EmptyModule.EmptyService>();
        emptyService.Should().NotBeNull("the empty service should be registered by the empty module");
    }

    [TestMethod]
    public void AddStarlightsPlatform_ShouldInvokePlatformServicesExtension()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton(_mockService.Object);

        // Act
        builder.AddStarlightsPlatform();

        // Assert
        _mockService.Verify(mock => mock.TestConfigureServices(It.IsAny<EmptyServiceExtension>()),
            Times.Exactly(3), $"the {nameof(EmptyServiceExtension)} should be invoked during platform initialization");
    }

    [TestMethod]
    public void AddStarlightsPlatform_ShouldInvokePlatformApplicationExtension()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton(_mockService.Object);
        builder.AddStarlightsPlatform();
        var app = builder.Build();

        // Act
        app.UseStarlightsPlatform();

        // Assert
        _mockService.Verify(mock => mock.TestConfigureApplication(It.IsAny<EmptyApplicationExtension>()),
            Times.Exactly(3), $"the {nameof(EmptyApplicationExtension)} should be invoked during platform initialization");
    }

    [TestMethod]
    public void AddStarlightsPlatform_ShouldInvokePlatformServicesExtensionInOrder()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton(_mockService.Object);

        // Act
        builder.AddStarlightsPlatform();

        // Assert
        _mockService.Invocations.Select(i => i.Arguments[0].GetType().Name)
            .Should().ContainInOrder(nameof(BeforeEmptyServiceExtension), nameof(EmptyServiceExtension), nameof(AfterEmptyServiceExtension));
    }

    [TestMethod]
    public void AddStarlightsPlatform_ShouldInvokePlatformApplicationExtensionInOrder()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddSingleton(_mockService.Object);
        builder.AddStarlightsPlatform();
        var app = builder.Build();

        // Act
        app.UseStarlightsPlatform();

        // Assert
        _mockService.Invocations.Select(i => i.Arguments[0].GetType().Name)
            .Should().ContainInOrder(nameof(BeforeEmptyApplicationExtension), nameof(EmptyApplicationExtension), nameof(AfterEmptyApplicationExtension));
    }
}
