namespace Starlights.RuleSets.DND5E5.Building;

/// <summary>
/// Records the initial D&D build stage, without invented player decisions.
/// </summary>
public sealed record BuildPart
{

    /// <summary>
    /// Creates supported initial state; later slices introduce actual decisions.
    /// </summary>
    public BuildPart(string stage)
    {
        if (stage != "awaiting-choices")
        {
            throw new ArgumentException("Unsupported DND5E5 build stage.", nameof(stage));
        }

        Stage = stage;
    }

    /// <summary>
    /// Gets the stage preceding player decisions and sheet evaluation.
    /// </summary>
    public string Stage
    {
        get;
    }
}
