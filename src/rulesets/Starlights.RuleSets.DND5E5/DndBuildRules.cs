using Starlights.Modules.Characters.Domain.Building;

namespace Starlights.RuleSets.DND5E5;

/// <summary>Direct reference-build policies and a rebuildable evaluation over player facts and typed JSON content.</summary>
public sealed class DndBuildRules(DndContentPack pack) : ICharacterAbilityGenerationRules
{
    /// <summary>Reads the pinned, validated definition for a retained possession.</summary>
    public DndContentElement Element(string id) => pack.Elements[id];
    /// <inheritdoc />
    public string Id => "DND5E5";
    /// <inheritdoc />
    public string RulesetId => Id;
    /// <inheritdoc />
    public CharacterContentBinding Content => pack.Content;
    /// <inheritdoc />
    public Type CompositionType => typeof(DndCharacterComposition);
    /// <inheritdoc />
    public ICharacterComposition Initialize(CharacterContentBinding content) => InitializeAbilityAssignments(Compose(DndBuildFacts.Empty));
    /// <inheritdoc />
    public void Validate(ICharacterComposition composition)
    {
        new DndRuleset().Validate(composition);
        var facts = Facts(composition);
        if (facts.Receipts.Select(receipt => receipt.SelectionId).Distinct(StringComparer.Ordinal).Count() != facts.Receipts.Count
            || facts.Receipts.SelectMany(receipt => receipt.Items).Any(item => item.Quantity < 1 || !pack.Elements.ContainsKey(item.ElementId)))
            throw new InvalidOperationException("Invalid D&D possessions.");
    }
    /// <inheritdoc />
    public ICharacterComposition RecordChoice(ICharacterComposition composition, string selectionId, IReadOnlyList<string> answers)
    {
        var facts = Facts(composition);
        var current = View(composition);
        var selection = current.Selections.SingleOrDefault(candidate => candidate.Id == selectionId)
            ?? throw new ArgumentException("The Selection is unavailable.");
        // Qualification for replacement excludes this decision's old answers, not another granting origin.
        var without = facts.Choices.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
        without.Remove(selectionId);
        var replacement = Compose(new(without, facts.Inputs, facts.Receipts, facts.ArmorId, facts.WeaponId, facts.AbilityGeneration));
        var options = View(replacement).Selections.Single(candidate => candidate.Id == selectionId).Options;
        if (answers.Count > selection.Count || answers.Distinct(StringComparer.Ordinal).Count() != answers.Count
            || answers.Any(answer => !options.Any(option => option.Id == answer)))
            throw new ArgumentException("The Selection answers are invalid.");
        without[selectionId] = answers;
        return Compose(new(without, facts.Inputs, facts.Receipts, facts.ArmorId, facts.WeaponId, facts.AbilityGeneration));
    }
    /// <inheritdoc />
    public ICharacterComposition AssignAbilities(ICharacterComposition composition, IReadOnlyDictionary<string, int> inputs)
    {
        var facts = Facts(composition);
        var abilities = View(composition).Abilities.Select(ability => ability.Id).ToArray();
        var issue = InputIssue(facts.AbilityGeneration, inputs, abilities);
        if (issue is not null) throw new ArgumentException(issue);
        return Compose(new(facts.Choices, inputs, facts.Receipts, facts.ArmorId, facts.WeaponId, facts.AbilityGeneration));
    }
    /// <inheritdoc />
    public ICharacterComposition ChangeAbilityGeneration(ICharacterComposition composition, string method, bool reroll)
    {
        var facts = Facts(composition);
        if (method is not ("standard-array" or "point-buy" or "application-rolls" or "physical-dice")
            || reroll && (method != "application-rolls" || facts.AbilityGeneration.Method != method))
            throw new ArgumentException("Select a supported method; reroll only the active application-roll method.");
        if (method == facts.AbilityGeneration.Method && !reroll) return composition;
        var rolls = method == "application-rolls" && (reroll || facts.AbilityGeneration.Rolls.Count == 0)
            ? DndAbilityGeneration.Roll() : facts.AbilityGeneration.Rolls;
        return InitializeAbilityAssignments(Compose(new(facts.Choices, new Dictionary<string, int>(), facts.Receipts, facts.ArmorId, facts.WeaponId, new(method, rolls))));
    }

    private ICharacterComposition InitializeAbilityAssignments(ICharacterComposition composition)
    {
        var generation = Facts(composition).AbilityGeneration;
        if (generation.Method == "physical-dice") return composition;
        var view = View(composition);
        IReadOnlyList<int> values = generation.Method == "point-buy"
            ? Enumerable.Repeat(8, view.Abilities.Count).ToArray() : view.AbilityInputOptions;
        if (values.Count != view.Abilities.Count)
            throw new InvalidOperationException("The generation results must match the number of offered abilities.");
        return AssignAbilities(composition, view.Abilities.Select((ability, index) => (ability.Id, Value: values[index]))
            .ToDictionary(pair => pair.Id, pair => pair.Value, StringComparer.Ordinal));
    }

    private string? InputIssue(DndAbilityGeneration generation, IReadOnlyDictionary<string, int> inputs, IReadOnlyList<string> abilities)
    {
        if (inputs.Keys.Any(key => !abilities.Contains(key, StringComparer.Ordinal)))
            return "Assign scores only to offered abilities.";
        if (generation.Method == "point-buy")
        {
            if (inputs.Values.Any(value => value is < 8 or > 15)) return "Point-buy scores must be from 8 through 15.";
            if (inputs.Values.Sum(DndAbilityGeneration.PointCost) > 27) return "Point-buy assignments cannot exceed the 27-point budget.";
        }
        else if (generation.Method == "physical-dice")
        {
            if (inputs.Values.Any(value => value is < 3 or > 18)) return "Physical dice totals must be from 3 through 18.";
        }
        else
        {
            var available = generation.Method == "application-rolls" ? generation.Rolls.Select(roll => roll.Total).ToArray()
                : pack.Elements[Content.CharacterElementId].Property<DndStandardArrayProperty>()?.Values
                    ?? throw new InvalidOperationException("The ability method is unavailable.");
            if (inputs.Values.GroupBy(value => value).Any(group => group.Count() > available.Count(value => value == group.Key)))
                return "Assign each available result at most once; duplicate rolled totals may be used as often as rolled.";
        }
        return null;
    }
    /// <inheritdoc />
    public ICharacterComposition ClaimEquipment(ICharacterComposition composition, string selectionId)
    {
        var facts = Facts(composition);
        if (facts.Receipts.Any(receipt => receipt.SelectionId == selectionId)) return composition;
        var view = View(composition);
        var selection = view.Selections.SingleOrDefault(candidate => candidate.Id == selectionId);
        if (selection is not { IsPackage: true, Answers.Count: 1 }) throw new ArgumentException("Choose an available starting package before claiming it.");
        var option = pack.Elements[selection.Answers[0]];
        var package = option.Property<DndPackageProperty>() ?? throw new ArgumentException("The selected package is unavailable.");
        var items = package.Items.Select((item, index) =>
        {
            var target = item.Target;
            if (target is null)
            {
                var decision = view.Selections.SingleOrDefault(candidate => candidate.Id == item.TargetSelection);
                if (decision?.Answers.Count != 1) throw new ArgumentException("Complete the package's item choice before claiming it.");
                target = decision.Answers[0];
            }
            return new DndOwnedItem($"{selectionId}/item-{index}", target, item.Quantity);
        }).ToArray();
        var receipt = new DndEquipmentReceipt(selectionId, option.Id, package.Gold, items);
        return Compose(new(facts.Choices, facts.Inputs, [.. facts.Receipts, receipt], facts.ArmorId, facts.WeaponId, facts.AbilityGeneration));
    }
    /// <inheritdoc />
    public ICharacterComposition Equip(ICharacterComposition composition, string? armorId, string? weaponId)
    {
        var facts = Facts(composition);
        var items = facts.Receipts.SelectMany(receipt => receipt.Items).ToDictionary(item => item.Id, StringComparer.Ordinal);
        if (armorId is not null && (!items.TryGetValue(armorId, out var armor) || pack.Elements[armor.ElementId].Property<DndArmorProperty>() is null)
            || weaponId is not null && (!items.TryGetValue(weaponId, out var weapon) || pack.Elements[weapon.ElementId].Property<DndWeaponProperty>() is null))
            throw new ArgumentException("Equipment must refer to owned items with the appropriate equipment property.");
        return Compose(new(facts.Choices, facts.Inputs, facts.Receipts, armorId, weaponId, facts.AbilityGeneration));
    }

    /// <summary>Independently derives the current decisions, numeric values and source evidence.</summary>
    public DndBuildView View(ICharacterComposition composition)
    {
        var facts = Facts(composition);
        var occurrences = new List<(string Path, DndContentElement Element, string? Decision)>();
        var decisions = new List<(string Id, DndContentElement Element, DndContentRule Rule)>();
        var excluded = new HashSet<string>(StringComparer.Ordinal);
        void Apply(string id, string path, int depth, string? decision = null)
        {
            if (excluded.Contains(path)) return;
            if (depth > 30 || occurrences.Count > 500) throw new InvalidOperationException("The content graph exceeds supported limits.");
            var element = pack.Elements[id];
            occurrences.Add((path, element, decision));
            foreach (var rule in element.Rules)
            {
                var origin = $"{path}/{rule.Id}";
                if (rule.Kind == "grant") Apply(rule.Target!, origin, depth + 1);
                if (rule.Kind == "selection")
                {
                    decisions.Add((origin, element, rule));
                    if (facts.Choices.TryGetValue(origin, out var answers))
                        foreach (var answer in answers.Where(answer => pack.Elements.TryGetValue(answer, out var candidate) && DndContentPack.Matches(rule, candidate)))
                            Apply(answer, $"{origin}/{answer}", depth + 1, origin);
                }
            }
        }
        Apply(Content.CharacterElementId, Content.CharacterElementId, 0);
        foreach (var occurrence in occurrences.Where(occurrence => occurrence.Decision is not null))
            if (occurrences.Any(other => other.Element.Id == occurrence.Element.Id && other.Path != occurrence.Path))
                excluded.Add(occurrence.Path);
        if (excluded.Count > 0)
        {
            occurrences.Clear();
            decisions.Clear();
            Apply(Content.CharacterElementId, Content.CharacterElementId, 0);
        }
        var selections = decisions.Select(decision =>
        {
            var answers = facts.Choices.GetValueOrDefault(decision.Id) ?? [];
            var options = pack.Elements.Values.Where(candidate => DndContentPack.Matches(decision.Rule, candidate)
                && !occurrences.Any(occurrence => occurrence.Element.Id == candidate.Id
                    && !occurrence.Path.StartsWith(decision.Id + "/", StringComparison.Ordinal))).ToArray();
            return new DndSelection(decision.Id, $"{decision.Element.Name}: {decision.Rule.Id.Replace('-', ' ')}",
                decision.Rule.Count, answers, Array.AsReadOnly(options), options.Any(option => option.Property<DndPackageProperty>() is not null));
        }).ToArray();
        var contributions = new Dictionary<string, List<(string Value, string Source)>>(StringComparer.Ordinal);
        void Add(string target, string value, string source)
        {
            if (!contributions.TryGetValue(target, out var existing)) contributions[target] = existing = [];
            existing.Add((value, source));
        }
        var abilityDefinitions = occurrences.Where(occurrence => occurrence.Element.Property<DndAbilityProperty>() is not null).ToArray();
        var inputIssue = InputIssue(facts.AbilityGeneration, facts.Inputs,
            abilityDefinitions.Select(occurrence => occurrence.Element.Property<DndAbilityProperty>()!.Prefix).ToArray());
        var abilityLimits = abilityDefinitions.ToDictionary(occurrence => occurrence.Element.Property<DndAbilityProperty>()!.Prefix + ".score",
            occurrence => occurrence.Element.Property<DndAbilityProperty>()!.Maximum, StringComparer.Ordinal);
        foreach (var occurrence in abilityDefinitions)
        {
            var ability = occurrence.Element.Property<DndAbilityProperty>()!;
            var assigned = facts.Inputs.TryGetValue(ability.Prefix, out var input);
            Add($"{ability.Prefix}.score", inputIssue is not null ? "invalid.input"
                : (assigned ? input : ability.DefaultInput ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture),
                assigned ? $"{occurrence.Path}/input/{facts.AbilityGeneration.Method}"
                    : $"{occurrence.Path}/fallback/{(ability.DefaultInput.HasValue ? "declared-default" : "zero")}");
            Add($"{ability.Prefix}.modifier", ability.Modifier, $"{occurrence.Path}/modifier");
        }
        foreach (var occurrence in occurrences)
            foreach (var rule in occurrence.Element.Rules.Where(rule => rule.Kind == "statistic" && (!rule.RequiresArmor || facts.ArmorId is not null)))
                Add(rule.Target!, rule.Value!, $"{occurrence.Path}/{rule.Id}");
        var armorItem = facts.Receipts.SelectMany(receipt => receipt.Items).SingleOrDefault(item => item.Id == facts.ArmorId);
        if (armorItem is not null)
        {
            var armor = pack.Elements[armorItem.ElementId].Property<DndArmorProperty>()!;
            Add("armor-class", armor.ArmorClass.ToString(System.Globalization.CultureInfo.InvariantCulture), $"{armorItem.Id}/armor");
        }
        else Add("armor-class", "10 + dexterity.modifier", "unarmored");
        var evaluated = new Dictionary<string, int?>(StringComparer.Ordinal);
        var ordinaryScores = new Dictionary<string, int?>(StringComparer.Ordinal);
        var visiting = new HashSet<string>(StringComparer.Ordinal);
        int? Calculate(string id)
        {
            if (evaluated.TryGetValue(id, out var value)) return value;
            if (!contributions.TryGetValue(id, out var sources) || !visiting.Add(id)) return null;
            int? total = 0;
            foreach (var source in sources) total += DndNumericExpression.Parse(source.Value).Evaluate(name => Calculate(name));
            visiting.Remove(id);
            if (abilityLimits.TryGetValue(id, out var maximum))
            {
                ordinaryScores[id] = total;
                total = total.HasValue ? Math.Min(total.Value, maximum) : null;
            }
            evaluated[id] = total;
            return total;
        }
        var statistics = contributions.Select(pair => new DndStatistic(pair.Key, pair.Key.Replace('.', ' ').Replace('-', ' '),
            Calculate(pair.Key), Array.AsReadOnly(pair.Value.Select(value => value.Source)
                .Concat(abilityLimits.TryGetValue(pair.Key, out var maximum) ? [$"DND5E5/maximum/{maximum}"] : Array.Empty<string>()).ToArray()))).ToArray();
        var abilities = abilityDefinitions.Select(occurrence =>
        {
            var ability = occurrence.Element.Property<DndAbilityProperty>()!;
            return new DndAbilityResult(ability.Prefix, occurrence.Element.Name, facts.Inputs.TryGetValue(ability.Prefix, out var input) ? input : null,
                Calculate($"{ability.Prefix}.score"), Calculate($"{ability.Prefix}.modifier"),
                ordinaryScores.GetValueOrDefault($"{ability.Prefix}.score"), ability.Maximum, !facts.Inputs.ContainsKey(ability.Prefix),
                inputIssue ?? (facts.Inputs.ContainsKey(ability.Prefix) ? null : "Required ability assignment is missing; fallback is not an answer."),
                statistics.Single(statistic => statistic.Id == $"{ability.Prefix}.score").Sources);
        }).ToArray();
        var attacks = new List<DndAttack>();
        var weaponItem = facts.Receipts.SelectMany(receipt => receipt.Items).SingleOrDefault(item => item.Id == facts.WeaponId);
        if (weaponItem is not null)
        {
            var element = pack.Elements[weaponItem.ElementId];
            var weapon = element.Property<DndWeaponProperty>()!;
            var modifier = Calculate($"{weapon.Ability}.modifier");
            var proficiency = Calculate("weapon-proficiency");
            if (modifier.HasValue && proficiency.HasValue)
                attacks.Add(new(element.Name, modifier.Value + proficiency.Value,
                    $"{weapon.Damage}{(modifier.Value < 0 ? "" : "+")}{modifier.Value} {weapon.DamageType}",
                    occurrences.Any(occurrence => occurrence.Element.Property<DndWeaponMasteryProperty>()?.WeaponId == weaponItem.ElementId)
                        ? weapon.Mastery : null));
        }
        var presentationReady = true;
        var entries = occurrences.SelectMany(occurrence => occurrence.Element.Entries.Select(entry =>
        {
            string? Replace(string? template) => template is null ? null : System.Text.RegularExpressions.Regex.Replace(template, @"\{\{([a-z][a-z0-9-]*)\}\}", match =>
            {
                var value = occurrence.Element.SheetValues[match.Groups[1].Value];
                if (value.Kind == "text") return Escape(value.Value);
                if (value.Kind == "element-name") return Escape(pack.Elements[value.Value].Name);
                var number = DndNumericExpression.Parse(value.Value).Evaluate(reference => Calculate(reference));
                if (number.HasValue) return number.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                presentationReady = false;
                return "Unavailable";
            });
            return new DndSheetEntry(Replace(entry.Name)!, Replace(entry.Description), Replace(entry.Usage), $"{occurrence.Path}/sheet/{entry.Id}");
        })).ToArray();
        var complete = inputIssue is null && abilities.All(ability => ability.Input.HasValue) && selections.All(selection =>
            selection.Answers.Count == selection.Count && selection.Answers.All(answer => selection.Options.Any(option => option.Id == answer)));
        var ready = statistics.All(statistic => statistic.Value.HasValue) && presentationReady;
        IReadOnlyList<int> inputOptions = facts.AbilityGeneration.Method switch
        {
            "standard-array" => pack.Elements[Content.CharacterElementId].Property<DndStandardArrayProperty>()?.Values ?? [],
            "application-rolls" => facts.AbilityGeneration.Rolls.Select(roll => roll.Total).ToArray(),
            "point-buy" => Enumerable.Range(8, 8).ToArray(),
            "physical-dice" => Enumerable.Range(3, 16).ToArray(),
            _ => throw new InvalidOperationException("Unsupported ability generation method.")
        };
        var pointsSpent = facts.AbilityGeneration.Method == "point-buy" && inputIssue is null
            ? facts.Inputs.Values.Sum(DndAbilityGeneration.PointCost) : (int?)null;
        return new(Array.AsReadOnly(selections), Array.AsReadOnly(abilities), Array.AsReadOnly(statistics),
            attacks.AsReadOnly(), Array.AsReadOnly(entries), complete, ready, inputOptions, pointsSpent);
    }

    /// <summary>Reads protected facts from the supported composition.</summary>
    public static DndBuildFacts Facts(ICharacterComposition composition)
        => composition is DndCharacterComposition { Facts: { } facts } ? facts
            : throw new InvalidOperationException("This pinned Character does not support reference mechanics.");
    private static DndCharacterComposition Compose(DndBuildFacts facts) => new(new("awaiting-choices"), facts);
    private static string Escape(string text) => System.Text.RegularExpressions.Regex.Replace(text, @"([\\`*_{}\[\]()#+.!<>-])", @"\$1");
}
