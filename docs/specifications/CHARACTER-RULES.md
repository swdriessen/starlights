# Character rules and computation

These requirements carry forward applicable Aurora Next agreements into the new application. They describe intended behavior, not a claim that the existing prototypes implement every case. See [SPECIFICATION.md](./SPECIFICATION.md) for product scope, [CONTENT-AND-COMPUTATION.md](./CONTENT-AND-COMPUTATION.md) for shared authoring and evaluation contracts, and [BEHAVIOR.md](./BEHAVIOR.md) for examples.

For Element occurrences, Requirements, ability inputs, score composition and checks, use [the shared computation contracts](./CONTENT-AND-COMPUTATION.md). They apply to the mechanics below as well.

## Class advancement

- In D&D, Character Level is the sum of active Class Levels and is 0 before a class is chosen. Ordered class entries identify the first class and preserve the advancement order needed for class-specific decisions and hit-point gains.
- Rules granted through a class entry retain that entry's class context through their Grant ancestry. Character-wide progression uses Character Level; class-specific progression uses the applicable Class Level.
- A class change revalidates prerequisites, starting-class benefits and dependent Choices. Removing the first class promotes the next remaining class; it does not recreate starting possessions.

## Spells

- Spell definitions compose validated properties for level, school, casting time, duration, range, verbal/somatic/material components and their applicable material requirements, ritual and concentration. Use the shared composable property contracts rather than hardcoding these fields into every Ruleset's generic Element.
- Learning, preparation and access are distinct. Record learning and preparation under their applicable source and preserve source-specific casting permissions and modifications.
- A source can be a class, feature, species or particular Owned Item. Each access and casting method declares its permissions and applicable modifications; neither a shared spell identity nor possession of a slot transfers permissions between sources.
- Learning and replacement retain their source and applicable class advancement. Current preparation is loadout intent rather than a history of every preparation change or a resource expenditure.
- A replacement identifies the earlier learned entry it replaces and is checked at the applicable advancement. Undoing it restores that entry; a later replacement whose target is unavailable remains invalid rather than silently targeting another entry.
- Acquiring a spell for a spellbook is distinct from preparing it and from owning a physical spellbook. Executing casting, spending resources and physical spellbook lifecycle are separate concerns.
- Spell access can expose source-specific capacities, recovery information, casting time, components and range or effect-distance modifications. Such metadata does not execute a rest, consume a resource or grant permission through another source.
- Present the applicable casting ability, spell attack bonus, save DC, preparation capacity and spell-slot capacities under the supported Ruleset's rules. Preserve distinctions between shared multiclass Spellcasting slots, Pact Magic and other source-specific permissions or capacities. Computing and displaying these results is initial builder scope; expending slots and performing recovery are not. Exact typed property and progression contracts must be specified with independently verified SRD cases before those mechanics are considered implemented.

## Movement, senses and defenses

- Content defines the movement modes, senses, damage types and defenses used by a supported Ruleset. Supported mechanics determine their typed values, ranges, eligibility and combination policies. Do not infer a resistance from a feature name or require every Ruleset to use the same senses.
- Present established movement modes and speeds, senses and applicable ranges, damage resistances/vulnerabilities/immunities and condition-related defenses on the Character Sheet where relevant. Keep their contributing origins inspectable.
- Preserve conditional qualifications, such as a defense applying only against a particular kind of effect. Show a qualified reminder rather than presenting a situational defense as unconditional or simulating an incoming attack.
- Definitions that reuse existing supported mechanics can be added through content without editing a hardcoded list in the generic core. New semantics still require a trusted implementation and an explicit combination policy; merely adding a property or description does not implement them.

## Equipment and possessions

- Possessions, equipment allocations and attunement intent are distinct. Ownership alone does not grant benefits that require an item to be equipped or attuned.
- Allocations refer to owned item identities and quantities. If an item is removed or its quantity decreases, preserve affected allocation intent and explain the issue rather than silently selecting a replacement or trimming allocations.
- A homogeneous stack can allocate quantities to different roles, but independently stateful or attuned items require separate physical identities. Allocation does not acquire or consume possessions.
- Use named armor and shield roles, Ruleset-extensible hand positions and slotless other equipment. A shield reserves a permitted hand; a two-handed use occupies two permitted positions. Additional hand-like positions declare their permitted uses rather than assuming every limb can use every item.
- Equipment and attunement are independent intents. Content declares each benefit's eligibility and any exclusive activation limits; a valid nonstacking bonus follows its combination policy rather than becoming an equipment conflict.
- Preserve and report conflicting equipment or attunement intent rather than silently choosing which item wins. Apply only independently eligible benefits.
- The Ruleset supplies an attunement maximum, which supported effects and numeric adjustments can change. A required bond has a nonnegative cost, defaulting to one; cost zero still requires eligibility and a bond. If eligible positive-cost bonds exceed capacity, none of those positive-cost bonds wins by ordering; independently eligible zero-cost bonds remain valid.
- Evaluate complete eligible armor-class alternatives separately, then apply their compatible bonuses. Do not combine parts of incompatible alternatives or choose an invalid fallback to hide a conflict.
- Explain the winning and eligible losing armor-class alternatives and their compatible bonuses. Equal totals are equivalent rather than requiring an arbitrary mechanical winner.
- An armor drawback does not automatically remove its armor-class benefit; apply the consequences declared by the applicable rules.
- Attack profiles retain their particular item or feature origin, relevant ability, proficiency, damage, range and applicable properties. An enhancement to one weapon does not automatically enhance another weapon or unarmed attacks.
- Separate melee and thrown uses where applicable; hand use determines baseline versatile damage. Select the best eligible ability by default while allowing another permitted choice. An unwielded inventory weapon is not presented as an attack-ready profile.
- Present supported situational attack or damage alternatives separately with their conditions; do not add them to unconditional values or imply that an attack has been executed.
- Ammunition-dependent profiles check compatible eligible ammunition without consuming it. Mechanically different alternatives require a player selection; mechanically equivalent eligible alternatives may be selected automatically. An unavailable ammunition source leaves the configuration unavailable with a reason.
- Loading a one-handed ammunition weapon requires a free permitted hand position; a two-handed weapon uses its assigned pair rather than requiring a third hand. Loading limits are reminders, not action or ammunition expenditure tracking.
- Additional typed damage components and explicit component replacements retain their own sources and damage types. Situational alternatives are separate previews rather than automatically combined, selected or executed attacks.
- Ordered Item Decorator applications modify an owned item while preserving the base item and each application's identity and provenance. Distinct item copies can have different modifications and state.
- Compose decorators left to right with explicit supported operations and compatibility checks against the original base and the current intermediate item. Decorators are nonrepeatable by default; declared exclusive families apply within one composition. Invalid new composition edits fail atomically, unlike preserved invalid equipment intent.
- Reordering preserves surviving Decorator Application identities; replacing an application creates a new application identity while preserving the Owned Item identity. Naming operations can change the displayed name without erasing earlier effects. The default composed rarity is the highest declared rarity.
- If existing composition becomes unresolvable after content changes, retain the recorded applications and report the issue without activating bare-base mechanics as a fallback. A homogeneous stateless stack shares its composition; splitting and merging stacks are deferred.
- Attunement is one bond per owned item. Where multiple applied definitions require it, their requirements must hold and the item's cost is the highest applicable declared cost, not the sum.
- Manual numeric adjustments preserve the rules-derived result. A user-set value replaces the final value; otherwise an optional user bonus is added. An adjustment does not make an otherwise ineligible item or attack valid.
- Adjustments use stable Character or item/profile targets. Reject invalid new targets and invalid values; retain an adjustment whose previously valid target disappears as dormant intent rather than attaching it to a replacement. Manual damage-expression editing remains deferred.
- Starting-equipment receipts preserve the original acquired items and currency separately from the Choice. A standard correction is previewed and explicitly confirmed; if the original items, their original composition or the required currency are no longer available, it is blocked rather than silently reconstructing them or reversing unrelated acquisitions.
- Future carrying-state mechanics must distinguish ownership, carrying and equipment: an owned item can be carried without being equipped or remain owned while not carried. Detailed location, containers, carrying quantities and their commands are deferred; absence of equipment does not establish a storage location.
