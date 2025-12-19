using System.Diagnostics;
using OpenTelemetry;

namespace Starlights.Integration;

/// <summary>
/// Custom activity processor that outputs simplified telemetry information to console.
/// Shows only activity display name and tags for cleaner integration test output.
/// </summary>
internal sealed class CustomConsoleActivityProcessor : BaseProcessor<Activity>
{
    public override void OnStart(Activity activity)
    {
        if (activity.Source.Name.StartsWith("Starlights"))
        {
            var tags = activity.TagObjects.Any()
                ? string.Join(", ", activity.TagObjects.Select(t => $"{t.Key}={t.Value}"))
                : "no tags";

            Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss:fff} ACT] >>>> {activity.DisplayName} | [{tags}]");
        }
    }

    public override void OnEnd(Activity activity)
    {
        if (activity.Source.Name.StartsWith("Starlights"))
        {
            var tags = activity.TagObjects.Any()
                ? string.Join(", ", activity.TagObjects.Select(t => $"{t.Key}={t.Value}"))
                : "no tags";

            Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss:fff} ACT] <<<< {activity.DisplayName} | [{tags}]");
        }
    }
}