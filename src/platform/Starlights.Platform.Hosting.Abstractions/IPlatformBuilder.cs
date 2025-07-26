using Microsoft.Extensions.DependencyInjection;

namespace Starlights.Platform.Hosting.Abstractions;

/// <summary>
/// Defines a builder for configuring the platform.
/// </summary>
public interface IPlatformBuilder
{
    /// <summary>
    /// Gets the service collection for the platform builder.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets the options for configuring the platform builder.
    /// </summary>
    PlatformBuilderOptions Options { get; }
}
