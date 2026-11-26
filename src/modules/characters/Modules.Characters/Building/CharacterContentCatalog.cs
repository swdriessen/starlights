using System.Reflection;
using System.Text.Json;
using Starlights.Modules.Characters.Domain.Building;

namespace Starlights.Modules.Characters.Building;

/// <summary>Loads the bounded creation subset of explicitly bundled versioned content.</summary>
public sealed class CharacterContentCatalog
{
    private readonly List<CharacterCreationOption> _options = [];
    private readonly Dictionary<(string Id, string Version), JsonElement> _packs = [];

    /// <summary>Gets the validated creation options without mutable catalog access.</summary>
    public IReadOnlyList<CharacterCreationOption> Options => _options
        .GroupBy(option => (option.RulesetId, option.Content.PackId, option.Content.CharacterElementId))
        .Select(group => group.OrderByDescending(option => Version.Parse(option.Content.PackVersion)).First()).ToArray();

    /// <summary>Reads the exact pinned pack; callers never substitute a current version.</summary>
    public JsonElement GetPack(CharacterContentBinding binding) => _packs[(binding.PackId, binding.PackVersion)];

    /// <summary>Loads an embedded pack, surfacing missing content rather than inventing defaults.</summary>
    public void AddEmbeddedPack(Assembly assembly, string resourceName, CharacterRulesets rulesets)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("A bundled Character content resource is missing.");
        using var reader = new StreamReader(stream);
        AddPack(reader.ReadToEnd(), rulesets);
    }

    /// <summary>Validates pack identities and Character bindings while retaining unevaluated declarations.</summary>
    public void AddPack(string json, CharacterRulesets rulesets)
    {
        using var document = JsonDocument.Parse(json, new JsonDocumentOptions { AllowDuplicateProperties = false });
        var root = document.RootElement;
        if (Text(root, "format") != "starlights-content/1")
            throw new InvalidOperationException("Unsupported Character content format.");
        var packId = Text(root, "id");
        var version = Text(root, "version");
        var packName = Text(root, "name");
        _ = Text(root, "licence");
        _ = Text(root, "attribution");
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var additions = new List<CharacterCreationOption>();
        foreach (var element in root.GetProperty("elements").EnumerateArray())
        {
            var id = Text(element, "id");
            if (!ids.Add(id))
                throw new InvalidOperationException("Content contains duplicate Element identities.");
            var ruleset = rulesets.GetRequired(Text(element, "ruleset"));
            if (Text(element, "type") != "character")
                continue;
            if (element.GetProperty("rules").ValueKind != JsonValueKind.Array)
                throw new InvalidOperationException("Character Rules must be an array.");
            var binding = new CharacterContentBinding(packId, version, id);
            additions.Add(new(ruleset.Id, Text(element, "name"), packName, binding, element.Clone()));
        }
        if (additions.Count == 0 || additions.Any(option => _options.Any(existing => existing.Content == option.Content)))
            throw new InvalidOperationException("Bundled content has no Character Element or duplicates an existing binding.");
        _options.AddRange(additions);
        _packs.Add((packId, version), root.Clone());
    }

    /// <summary>Requires an existing exact binding, never a newer version.</summary>
    public CharacterCreationOption GetRequired(string rulesetId, CharacterContentBinding binding)
        => _options.SingleOrDefault(option => option.RulesetId == rulesetId && option.Content == binding)
            ?? throw new InvalidOperationException("The Character's pinned content is unavailable.");

    private static string Text(JsonElement element, string property)
        => element.GetProperty(property).GetString() is { } value && !string.IsNullOrWhiteSpace(value)
            ? value : throw new InvalidOperationException("Required content metadata is missing.");
}
