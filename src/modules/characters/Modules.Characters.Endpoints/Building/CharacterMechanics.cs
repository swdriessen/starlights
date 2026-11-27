using Starlights.Modules.Characters.Building;
using Starlights.Modules.Characters.Domain.Building;
using Starlights.RuleSets.DND5E5.Building;

namespace Starlights.Modules.Characters.Endpoints.Building;

internal sealed class CharacterMechanics
{
    private readonly Dictionary<CharacterContentBinding, BuildRules> _rules = [];
    public CharacterMechanics(CharacterContentCatalog catalog)
    {
        foreach (var option in catalog.Options.Where(option => option.RulesetId == "DND5E5" && option.Content.PackVersion == "0.2.0"))
            _rules.Add(option.Content, new(new(catalog.GetPack(option.Content), option.Content)));
    }
    public bool Supports(CharacterContentBinding content) => _rules.ContainsKey(content);
    public BuildRules Required(CharacterContentBinding content)
        => _rules.GetValueOrDefault(content) ?? throw new CharacterRequestException("This pinned Character does not support build actions.");
}
