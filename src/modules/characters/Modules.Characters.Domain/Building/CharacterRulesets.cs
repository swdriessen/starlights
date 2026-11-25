namespace Starlights.Modules.Characters.Domain.Building;

/// <summary>Resolves explicitly registered Rulesets without registration-order precedence.</summary>
public sealed class CharacterRulesets
{
    private readonly Dictionary<string, ICharacterRuleset> _rulesets = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>Rejects duplicate identifiers and invalid composition contracts.</summary>
    public CharacterRulesets(IEnumerable<ICharacterRuleset> rulesets)
    {
        foreach (var ruleset in rulesets)
        {
            if (string.IsNullOrWhiteSpace(ruleset.Id) || ruleset.Id != ruleset.Id.ToUpperInvariant()
                || !typeof(ICharacterComposition).IsAssignableFrom(ruleset.CompositionType)
                || !_rulesets.TryAdd(ruleset.Id, ruleset))
                throw new InvalidOperationException("Ruleset registration is invalid or ambiguous.");
        }
    }

    /// <summary>Finds a supported selection; no implicit default or fallback is applied.</summary>
    public bool Contains(string id) => _rulesets.ContainsKey(id);

    /// <summary>Resolves a required implementation or surfaces missing configuration.</summary>
    public ICharacterRuleset GetRequired(string id)
        => _rulesets.TryGetValue(id, out var ruleset) ? ruleset
            : throw new InvalidOperationException("The selected Ruleset implementation is unavailable.");
}
