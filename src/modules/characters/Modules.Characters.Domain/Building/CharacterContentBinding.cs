using System.Text.RegularExpressions;

namespace Starlights.Modules.Characters.Domain.Building;

/// <summary>Identifies the exact published content that starts a Character's build.</summary>
public sealed partial record CharacterContentBinding
{
    /// <summary>Creates a binding without floating versions or missing identities.</summary>
    public CharacterContentBinding(string packId, string packVersion, string characterElementId)
    {
        if (string.IsNullOrWhiteSpace(packId) || string.IsNullOrWhiteSpace(characterElementId))
            throw new ArgumentException("Content identities are required.");
        if (!SemanticVersion().IsMatch(packVersion))
            throw new ArgumentException("An exact semantic pack version is required.");
        PackId = packId;
        PackVersion = packVersion;
        CharacterElementId = characterElementId;
    }

    /// <summary>Gets the published pack identity.</summary>
    public string PackId { get; }
    /// <summary>Gets its pinned semantic version.</summary>
    public string PackVersion { get; }
    /// <summary>Gets the selected content-defined Character Element.</summary>
    public string CharacterElementId { get; }

    [GeneratedRegex(@"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?:-(?:0|[1-9]\d*|\d*[A-Za-z-][0-9A-Za-z-]*)(?:\.(?:0|[1-9]\d*|\d*[A-Za-z-][0-9A-Za-z-]*))*)?(?:\+[0-9A-Za-z-]+(?:\.[0-9A-Za-z-]+)*)?$", RegexOptions.CultureInvariant)]
    private static partial Regex SemanticVersion();
}
