# Representative content JSON baseline

Use this example when designing the initial JSON loader, validator or authored content. It adapts the approved Aurora Next representative document's field shapes to a semantic pack version. `example-content/1` is an illustrative format marker, not the selected production identifier; that identifier remains to be chosen. The example is an original homebrew fixture, not SRD content.

The initial contract uses the shown Element, Rule, target, numeric value, ability and Requirement shapes. It is not a complete schema for all supported content. Expand the schema through tested slices, and apply the requirements in [CONTENT-AND-COMPUTATION.md](./CONTENT-AND-COMPUTATION.md).

```json
{
  "format": "example-content/1",
  "id": "proof",
  "name": "Representative Character (Homebrew)",
  "version": "0.1.0",
  "licence": "CC-BY-4.0",
  "attribution": "Original homebrew example for Aurora Builder redesign review; not SRD content.",
  "elements": [
    {
      "id": "proof:character:representative",
      "name": "Representative Character",
      "ruleset": "DND5E5",
      "rules": [
        { "id": "strength", "kind": "grant", "target": "proof:ability:strength" },
        { "id": "dexterity", "kind": "grant", "target": "proof:ability:dexterity" },
        { "id": "foundation", "kind": "grant", "target": "proof:feature:foundation" },
        { "id": "training", "kind": "selection", "options": { "type": "training" } },
        {
          "id": "earned",
          "kind": "grant",
          "target": "proof:feature:strength-trained",
          "requirement": {
            "subject": "character",
            "statistic": "strength.score",
            "atLeast": 14
          }
        }
      ]
    },
    {
      "id": "proof:ability:strength",
      "name": "Strength",
      "ruleset": "DND5E5",
      "ability": {
        "prefix": "strength",
        "defaultInput": 10,
        "modifier": "floor((strength.score - 10) / 2)"
      },
      "rules": [
        {
          "id": "maximum",
          "kind": "statistic",
          "target": { "subject": "character", "statistic": "strength.maximum" },
          "operation": "increase",
          "value": 20
        }
      ]
    },
    {
      "id": "proof:ability:dexterity",
      "name": "Dexterity",
      "ruleset": "DND5E5",
      "ability": {
        "prefix": "dexterity",
        "defaultInput": 10,
        "modifier": "floor((dexterity.score - 10) / 2)"
      },
      "rules": [
        {
          "id": "maximum",
          "kind": "statistic",
          "target": { "subject": "character", "statistic": "dexterity.maximum" },
          "operation": "increase",
          "value": 20
        }
      ]
    },
    {
      "id": "proof:feature:foundation",
      "name": "Foundation",
      "ruleset": "DND5E5",
      "description": "Your training increases your Strength by **1**.\n\n::: sidebar Training note\nThis is an original homebrew example, not SRD text.\n:::",
      "sheet": {
        "values": {
          "strength-bonus": 1,
          "current-strength": "strength.score",
          "feature-name": {
            "kind": "element-name",
            "target": "proof:feature:foundation"
          },
          "activation-label": { "kind": "text", "value": "No action" },
          "availability-label": { "kind": "text", "value": "Continuous" },
          "recovery-label": { "kind": "text", "value": "Not required" }
        },
        "entries": [
          {
            "id": "strength-increase",
            "name": "{{feature-name}} Training",
            "description": "Strength increase: +{{strength-bonus}}.",
            "action": "{{activation-label}}",
            "usage": "{{availability-label}}",
            "recovery": "{{recovery-label}}"
          },
          {
            "id": "strength-reminder",
            "name": "Strength Reminder",
            "description": "Current Strength: {{current-strength}}.",
            "variants": [
              {
                "id": "trained",
                "requirement": {
                  "subject": "character",
                  "statistic": "strength.score",
                  "atLeast": 14
                },
                "name": "Trained Strength",
                "description": "**Trained.** Current Strength: {{current-strength}}."
              },
              {
                "id": "omit-reminder-text",
                "requirement": {
                  "subject": "character",
                  "statistic": "strength.score",
                  "atLeast": 19
                },
                "descriptionHidden": true
              },
              {
                "id": "omit-reminder-entry",
                "requirement": {
                  "subject": "character",
                  "statistic": "strength.score",
                  "atLeast": 20
                },
                "hidden": true
              }
            ]
          }
        ]
      },
      "rules": [
        {
          "id": "increase",
          "kind": "statistic",
          "target": { "subject": "character", "statistic": "strength.score" },
          "operation": "increase",
          "value": 1
        }
      ]
    },
    {
      "id": "proof:item:strength-bracelet",
      "name": "Strength Bracelet",
      "ruleset": "DND5E5",
      "rules": [
        {
          "id": "setting",
          "kind": "statistic",
          "target": { "subject": "character", "statistic": "strength.score" },
          "operation": "setting",
          "value": 19,
          "requirement": {
            "all": [
              { "subject": "originating-item", "fact": "equipped", "is": true },
              { "subject": "originating-item", "fact": "attuned", "is": true }
            ]
          }
        }
      ]
    },
    {
      "id": "proof:training:strength",
      "name": "Strength Training",
      "ruleset": "DND5E5",
      "rules": [
        {
          "id": "increase",
          "kind": "statistic",
          "target": { "subject": "character", "statistic": "strength.score" },
          "operation": "increase",
          "value": 2
        }
      ]
    },
    {
      "id": "proof:training:dexterity",
      "name": "Dexterity Training",
      "ruleset": "DND5E5",
      "rules": [
        {
          "id": "increase",
          "kind": "statistic",
          "target": { "subject": "character", "statistic": "dexterity.score" },
          "operation": "increase",
          "value": 2
        }
      ]
    },
    {
      "id": "proof:feature:strength-trained",
      "name": "Strength-Trained Feature",
      "ruleset": "DND5E5",
      "rules": [
        {
          "id": "increase",
          "kind": "statistic",
          "target": { "subject": "character", "statistic": "dexterity.score" },
          "operation": "increase",
          "value": 1
        }
      ]
    }
  ]
}
```

## Interpretation

- The validated Element ID declares the type segment, such as `ability` or `training`. Rule IDs are stable within the Element; `kind` identifies a supported contract, not an evaluation pass.
- An Ability declares its prefix, default input and inline modifier formula. Player inputs and item state belong to the Character, not this content.
- A numeric literal is a JSON number; a computed numeric value is a bounded formula string. Requirements are structured conditions. The example uses Character-statistic `atLeast`, originating-item boolean `is` and `all`.
- The bracelet's conditions apply to the particular originating item; its target remains Character Strength. Its setting value participates in the Ruleset's shared highest-wins setting group.
- The training Selection defaults to one answer. A positive fixed count and Element repeatability are separate supported concepts.
- No author-supplied phases, priorities, dependency lists or setter-group labels are needed.
- Foundation illustrates static full-description Markdown, a sidebar and multiple sheet entries. Its `sheet.values` are presentation values, not additional Strength increases. Double-brace placeholders in every authored sheet-facing text field read their occurrence's evaluated numeric or text values.
- A bare value string such as `strength.score` is a numeric expression. Text uses an explicit binding: `kind: text` is a literal, while `kind: element-name` reads the named definition's display name. The first entry therefore renders as **Foundation Training**, with action **No action**, usage **Continuous** and recovery **Not required**. These are reminders; they do not activate the permanent Strength increase or execute gameplay.
- Sheet variants override explicitly supplied fields in authored order. Their order never schedules mechanics. The strength thresholds here illustrate presentation behavior; class- and Character-level conditions must follow the context contracts in [CONTENT-AND-COMPUTATION.md](./CONTENT-AND-COMPUTATION.md#descriptions-and-sheet-presentation). Their serialized bindings remain open rather than being implied by this score-based example.
- The example does not settle multi-pack resolution, replacement and append syntax, saved-character representation, every Requirement shape or broader item and spell contracts.
- Composable typed properties extend the supported model without requiring every Element to carry every game's fields. This example does not yet select a general property-envelope format or define serialized Choice-label and originating-item bindings.

## Expected results

For these cases, record Strength 12 and Dexterity 14 as Character inputs, select the representative Character Element and apply no other effects or manual adjustments:

| Training answer | Bracelet state | Strength | Dexterity | Strength-Trained Feature |
| --- | --- | --- | --- | --- |
| Strength Training | Not owned | 15 | 15 | Applied |
| Dexterity Training | Not owned | 13 | 16 | Not applied |
| Dexterity Training | Owned, equipped and attuned | 19 | 17 | Applied |

Foundation contributes +1 Strength in all three cases. The bracelet's setting is not an additional +19 and does not receive ordinary increases a second time.

All three cases show the separate **Foundation Training** entry with `Strength increase: +1.` The second entry is:

| Effective Strength | Reminder label | Reminder description |
| --- | --- | --- |
| 15 | Trained Strength | **Trained.** Current Strength: 15. |
| 13 | Strength Reminder | Current Strength: 13. |
| 19 | Trained Strength | Hidden; the label remains visible |

At Strength 20 the reminder entry is hidden entirely, while Foundation Training remains visible and the Strength increase still applies. These visibility rules are original fixture behavior, not game rules. The full description remains the authored static text in every case.
