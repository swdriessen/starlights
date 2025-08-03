using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Starlights.Platform.Components.Data.EntityFramework;

internal static class PersistenceInstrumentation
{
    /// <summary>
    /// The name of the ActivitySource used for telemetry in the Entity Framework component.
    /// </summary>
    public static readonly string ActivitySourceName = "Starlights.Platform.Persistence";

    /// <summary>
    /// The ActivitySource instance used for telemetry in the Entity Framework component.
    /// </summary>
    public static ActivitySource ActivitySource { get; } = new(ActivitySourceName);

    /// <summary>
    /// Starts a new Activity with the specified name and ActivityKind.Internal.
    /// </summary>
    /// <remarks>
    /// Shorthand for ActivitySource.StartActivity(name, ActivityKind.Internal))
    /// </remarks>
    public static Activity? StartActivity([CallerMemberName] string name = "", Action<Activity>? configureAction = null)
    {
        var activity = ActivitySource.StartActivity(name, ActivityKind.Internal);

        if (activity is not null)
        {
            configureAction?.Invoke(activity);
        }

        return activity;
    }
}