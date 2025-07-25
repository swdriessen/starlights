using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Starlights.Platform.Hosting;
using Starlights.Platform.Hosting.Abstractions;

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
        var emptyModule = app.Services.GetService<EmptyModule>();
        emptyModule.Should().NotBeNull();
    }

    [TestMethod]
    public void AddStarlightsPlatform_DisableModuleDiscovery()
    {
        // Arrange
        var builder = Host.CreateApplicationBuilder();

        // Act
        builder.AddStarlightsPlatform(options => options.IsModuleDiscoveryEnabled = false);

        // Assert
        var app = builder.Build();
        var emptyModule = app.Services.GetService<EmptyModule>();
        emptyModule.Should().BeNull();
    }

    [TestMethod]
    public void AddStarlightsPlatform_ShouldConfigureServicesOfModules()
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

    internal sealed class EmptyModule : IPlatformModule
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<EmptyService>();
        }

        internal class EmptyService
        {
            // This class is just a placeholder to ensure that the module can be resolved and its services can be configured.
        }
    }
}
