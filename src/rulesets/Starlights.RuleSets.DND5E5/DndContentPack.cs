using System.Globalization;
using System.Text.Json;
using Starlights.Modules.Characters.Domain.Building;

namespace Starlights.RuleSets.DND5E5;

/// <summary>Parses and validates the reference build's explicit, non-executable content subset.</summary>
public sealed class DndContentPack
{
    /// <summary>Loads an exact content pack; unsupported declarations fail rather than being ignored.</summary>
    public DndContentPack(JsonElement root, CharacterContentBinding content)
    {
        Fields(root, "format", "id", "name", "version", "licence", "attribution", "elements");
        if (Text(root, "format") != "starlights-content/1" || Text(root, "id") != content.PackId
            || Text(root, "version") != content.PackVersion) throw Invalid();
        Content = content;
        var elements = new Dictionary<string, DndContentElement>(StringComparer.Ordinal);
        foreach (var raw in root.GetProperty("elements").EnumerateArray())
        {
            Fields(raw, "id", "name", "type", "ruleset", "description", "rules", "ability", "properties", "sheet");
            if (Text(raw, "ruleset") != "DND5E5") throw Invalid();
            var properties = new List<IDndElementProperty>();
            if (raw.TryGetProperty("ability", out var ability))
            {
                Fields(ability, "prefix", "modifier", "defaultInput");
                properties.Add(new DndAbilityProperty(Text(ability, "prefix"), Text(ability, "modifier")));
            }
            if (raw.TryGetProperty("properties", out var parts))
                foreach (var part in parts.EnumerateArray()) properties.Add(ParseProperty(part));
            if (properties.GroupBy(property => property.GetType()).Any(group => group.Count() > 1)) throw Invalid();
            var rules = raw.GetProperty("rules").EnumerateArray().Select(ParseRule).ToArray();
            if (rules.Select(rule => rule.Id).Distinct(StringComparer.Ordinal).Count() != rules.Length) throw Invalid();
            var entries = new List<DndSheetEntry>();
            var values = new Dictionary<string, DndSheetValue>(StringComparer.Ordinal);
            if (raw.TryGetProperty("sheet", out var sheet))
            {
                Fields(sheet, "entries", "values");
                if (sheet.TryGetProperty("values", out var declaredValues))
                {
                    foreach (var value in declaredValues.EnumerateObject())
                    {
                        if (!System.Text.RegularExpressions.Regex.IsMatch(value.Name, "^[a-z][a-z0-9-]*$")) throw Invalid();
                        var binding = value.Value;
                        if (binding.ValueKind == JsonValueKind.Object)
                        {
                            var kind = Text(binding, "kind");
                            if (kind == "text") { Fields(binding, "kind", "value"); values.Add(value.Name, new(kind, Text(binding, "value"))); }
                            else if (kind == "element-name") { Fields(binding, "kind", "target"); values.Add(value.Name, new(kind, Text(binding, "target"))); }
                            else throw Invalid();
                        }
                        else
                        {
                            var expression = binding.ValueKind == JsonValueKind.Number ? binding.GetInt32().ToString(CultureInfo.InvariantCulture) : binding.GetString() ?? throw Invalid();
                            _ = DndNumericExpression.Parse(expression);
                            values.Add(value.Name, new("numeric", expression));
                        }
                    }
                }
                var entryIds = new HashSet<string>(StringComparer.Ordinal);
                foreach (var entry in sheet.GetProperty("entries").EnumerateArray())
                {
                    Fields(entry, "id", "name", "description", "usage");
                    if (!entryIds.Add(Text(entry, "id"))) throw Invalid();
                    entries.Add(new(Optional(entry, "name") ?? Text(raw, "name"), Optional(entry, "description"), Optional(entry, "usage"), Text(entry, "id")));
                }
            }
            var element = new DndContentElement(Text(raw, "id"), Text(raw, "name"), Text(raw, "type"),
                Optional(raw, "description"), Array.AsReadOnly(rules), properties.AsReadOnly(), entries.AsReadOnly(),
                new System.Collections.ObjectModel.ReadOnlyDictionary<string, DndSheetValue>(values));
            ValidateText(element.Description, null);
            foreach (var entry in entries)
            {
                ValidateText(entry.Name, values);
                ValidateText(entry.Description, values);
                ValidateText(entry.Usage, values);
            }
            if (!elements.TryAdd(element.Id, element)) throw Invalid();
        }
        if (!elements.TryGetValue(content.CharacterElementId, out var character) || character.Type != "character") throw Invalid();
        foreach (var element in elements.Values)
        {
            foreach (var rule in element.Rules)
            {
                if (rule.Kind == "grant" && !elements.ContainsKey(rule.Target!)) throw Invalid();
                if (rule.OptionIds.Any(id => !elements.ContainsKey(id))) throw Invalid();
                if (rule.Kind == "selection" && !elements.Values.Any(candidate => Matches(rule, candidate))) throw Invalid();
            }
            if (element.Property<DndPackageProperty>() is { } package)
                foreach (var item in package.Items)
                    if (item.Target is not null && !elements.ContainsKey(item.Target)) throw Invalid();
            if (element.Property<DndWeaponMasteryProperty>() is { } mastery
                && (!elements.TryGetValue(mastery.WeaponId, out var weaponDefinition) || weaponDefinition.Property<DndWeaponProperty>() is null)) throw Invalid();
            foreach (var value in element.SheetValues.Values.Where(value => value.Kind == "element-name"))
                if (!elements.ContainsKey(value.Value)) throw Invalid();
        }
        var targets = elements.Values.SelectMany(element => element.Rules).Where(rule => rule.Kind == "statistic").Select(rule => rule.Target!)
            .Concat(elements.Values.SelectMany(element => element.Properties.OfType<DndAbilityProperty>()
                .SelectMany(ability => new[] { $"{ability.Prefix}.score", $"{ability.Prefix}.modifier" }))).ToHashSet(StringComparer.Ordinal);
        foreach (var element in elements.Values)
        {
            var expressions = element.Rules.Where(rule => rule.Kind == "statistic").Select(rule => rule.Value!)
                .Concat(element.Properties.OfType<DndAbilityProperty>().Select(ability => ability.Modifier))
                .Concat(element.SheetValues.Values.Where(value => value.Kind == "numeric").Select(value => value.Value));
            foreach (var expression in expressions)
                if (DndNumericExpression.References(expression).Any(reference => !targets.Contains(reference))) throw Invalid();
            if (element.Property<DndWeaponProperty>() is { } weapon && !targets.Contains($"{weapon.Ability}.modifier")) throw Invalid();
        }
        // Reject recursive grant/option graphs at load time, including cycles hidden in unanswered options.
        void Visit(string id, HashSet<string> ancestry)
        {
            if (!ancestry.Add(id)) throw Invalid();
            foreach (var rule in elements[id].Rules)
            {
                if (rule.Kind == "grant") Visit(rule.Target!, new(ancestry, StringComparer.Ordinal));
                if (rule.Kind == "selection")
                    foreach (var option in elements.Values.Where(candidate => Matches(rule, candidate)))
                        Visit(option.Id, new(ancestry, StringComparer.Ordinal));
            }
        }
        Visit(character.Id, []);
        Elements = new System.Collections.ObjectModel.ReadOnlyDictionary<string, DndContentElement>(elements);
    }

    /// <summary>Gets the exact supported binding.</summary>
    public CharacterContentBinding Content { get; }
    /// <summary>Gets validated definitions by stable identity.</summary>
    public IReadOnlyDictionary<string, DndContentElement> Elements { get; }
    /// <summary>Matches explicitly declared identities or type labels, never display names.</summary>
    public static bool Matches(DndContentRule selection, DndContentElement element)
        => selection.OptionIds.Contains(element.Id, StringComparer.Ordinal) || selection.OptionType == element.Type;

    private static DndContentRule ParseRule(JsonElement raw)
    {
        Fields(raw, "id", "kind", "target", "options", "count", "operation", "value", "requirement");
        var kind = Text(raw, "kind");
        if (kind is not ("grant" or "selection" or "statistic")) throw Invalid();
        string? target = null, optionType = null, value = null;
        string[] optionIds = [];
        var count = raw.TryGetProperty("count", out var countValue) ? countValue.GetInt32() : 1;
        if (count is < 1 or > 20) throw Invalid();
        if (kind == "grant") target = Text(raw, "target");
        if (kind == "selection")
        {
            var options = raw.GetProperty("options");
            Fields(options, "type", "ids");
            optionType = Optional(options, "type");
            if (options.TryGetProperty("ids", out var ids)) optionIds = ids.EnumerateArray().Select(id => id.GetString() ?? throw Invalid()).ToArray();
            if ((optionType is null) == (optionIds.Length == 0)) throw Invalid();
        }
        if (kind == "statistic")
        {
            var subject = raw.GetProperty("target");
            Fields(subject, "subject", "statistic");
            if (Text(subject, "subject") != "character" || Text(raw, "operation") != "increase") throw Invalid();
            target = Text(subject, "statistic");
            var number = raw.GetProperty("value");
            value = number.ValueKind == JsonValueKind.Number
                ? number.GetInt32().ToString(CultureInfo.InvariantCulture) : number.GetString() ?? throw Invalid();
            _ = DndNumericExpression.Parse(value);
        }
        var armor = false;
        if (raw.TryGetProperty("requirement", out var requirement))
        {
            Fields(requirement, "subject", "fact", "is");
            if (kind != "statistic" || Text(requirement, "subject") != "character"
                || Text(requirement, "fact") != "wearing-armor" || !requirement.GetProperty("is").GetBoolean()) throw Invalid();
            armor = true;
        }
        return new(Text(raw, "id"), kind, target, optionType, Array.AsReadOnly(optionIds), count, value, armor);
    }

    private static IDndElementProperty ParseProperty(JsonElement raw)
    {
        var kind = Text(raw, "kind");
        switch (kind)
        {
            case "armor":
                Fields(raw, "kind", "armorClass");
                var ac = raw.GetProperty("armorClass").GetInt32();
                if (ac is < 1 or > 30) throw Invalid();
                return new DndArmorProperty(ac);
            case "weapon":
                Fields(raw, "kind", "ability", "damage", "damageType", "mastery");
                var damage = Text(raw, "damage");
                if (!System.Text.RegularExpressions.Regex.IsMatch(damage, @"^[1-9][0-9]?d(?:4|6|8|10|12)$")) throw Invalid();
                return new DndWeaponProperty(Text(raw, "ability"), damage, Text(raw, "damageType"), Text(raw, "mastery"));
            case "weapon-mastery":
                Fields(raw, "kind", "weapon");
                return new DndWeaponMasteryProperty(Text(raw, "weapon"));
            case "standard-array":
                Fields(raw, "kind", "values");
                var values = raw.GetProperty("values").EnumerateArray().Select(value => value.GetInt32()).ToArray();
                if (values.Length != 6 || values.Any(value => value is < 3 or > 20)) throw Invalid();
                return new DndStandardArrayProperty(Array.AsReadOnly(values));
            case "starting-package":
                Fields(raw, "kind", "gold", "items");
                var gold = raw.GetProperty("gold").GetInt32();
                if (gold < 0) throw Invalid();
                var items = raw.GetProperty("items").EnumerateArray().Select(item =>
                {
                    Fields(item, "target", "targetSelection", "quantity");
                    var target = Optional(item, "target");
                    var selection = Optional(item, "targetSelection");
                    var quantity = item.GetProperty("quantity").GetInt32();
                    if ((target is null) == (selection is null) || quantity < 1) throw Invalid();
                    return new DndPackageItem(target, selection, quantity);
                }).ToArray();
                return new DndPackageProperty(gold, Array.AsReadOnly(items));
            default: throw Invalid();
        }
    }

    private static void Fields(JsonElement raw, params string[] names)
    {
        if (raw.ValueKind != JsonValueKind.Object || raw.EnumerateObject().Any(property => !names.Contains(property.Name, StringComparer.Ordinal)))
            throw Invalid();
    }
    private static void ValidateText(string? text, IReadOnlyDictionary<string, DndSheetValue>? values)
    {
        if (text is null) return;
        if (System.Text.RegularExpressions.Regex.IsMatch(text, @"<[^>]*>|!\[|(?m)^\s*:::|javascript\s*:|data\s*:", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            throw Invalid();
        var placeholders = System.Text.RegularExpressions.Regex.Matches(text, @"\{\{([a-z][a-z0-9-]*)\}\}");
        if (placeholders.Any(match => values is null || !values.ContainsKey(match.Groups[1].Value))
            || System.Text.RegularExpressions.Regex.Replace(text, @"\{\{([a-z][a-z0-9-]*)\}\}", "").Contains("{{", StringComparison.Ordinal))
            throw Invalid();
    }
    private static string Text(JsonElement raw, string field) => Optional(raw, field) ?? throw Invalid();
    private static string? Optional(JsonElement raw, string field)
        => raw.TryGetProperty(field, out var value) && value.GetString() is { Length: > 0 } text ? text : null;
    private static InvalidOperationException Invalid() => new("The D&D mechanics content contains an unsupported or invalid declaration.");
}
