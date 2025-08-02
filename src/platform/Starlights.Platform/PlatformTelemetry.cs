using System.Diagnostics;

namespace Starlights.Platform;

/// <summary>
/// Provides a single ActivitySource for all Starlights platform components.
/// </summary>
public sealed class PlatformTelemetry
{
    /// <summary>
    /// The name of the ActivitySource for all platform components.
    /// </summary>
    public const string ActivitySourceName = "Starlights.Platform";

    /// <summary>
    /// The ActivitySource instance for all platform components.
    /// </summary>
    public ActivitySource ActivitySource { get; } = new(ActivitySourceName);
}