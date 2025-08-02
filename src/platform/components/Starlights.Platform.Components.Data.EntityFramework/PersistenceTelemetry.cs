using System.Diagnostics;

namespace Starlights.Platform.Components.Data.EntityFramework;

internal static class PersistenceTelemetry
{
    /// <summary>
    /// The name of the ActivitySource used for telemetry in the Entity Framework component.
    /// </summary>
    public static readonly string ActivitySourceName = "Starlights.Platform.Components.Data.EntityFramework";

    /// <summary>
    /// The ActivitySource instance used for telemetry in the Entity Framework component.
    /// </summary>
    public static ActivitySource ActivitySource { get; } = new(ActivitySourceName);
}