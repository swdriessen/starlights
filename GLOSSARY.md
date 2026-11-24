# Character Building

The language used to describe game content and a player's character-building decisions.

## Game content

**Ruleset**:
The rules governing characters in a particular game or version of a game.

**Element**:
A building block of game content, such as a class, feature, spell or item. Different kinds of Elements can carry different information.

**Element Occurrence**:
An Element applied to a Character through a particular granting origin, with its own benefits and build decisions.

**Element Origin**:
The particular Grant or decision through which an Element Occurrence entered a Character. It is distinct from the publication Source.

**Rule**:
A declarative instruction carried by an Element that affects a Character.

**Requirement**:
A condition that normally must hold for a Rule, option or benefit to apply.

**Requirement Override**:
An explicit exception allowing a particular Character to use an option or benefit despite an unmet Requirement.

**Grant**:
A Rule that gives a Character something, such as an Element or a feature.

**Selection Rule**:
A Rule that offers a player a Selection from Options or a permitted numeric range.

**Character Element**:
An Element that declares a Ruleset and starts a Character's build through Grants and Selections.

**Rule Element**:
An Element that carries reusable game Rules, such as an optional multiclassing rule. It is distinct from an individual Rule.

**Content Pack**:
A distributable bundle of Elements and associated assets for one Ruleset, ranging from a small homebrew addition to the content of a book. A published pack version has an identity shared by its users, distinct from their individual access to it.

**Content Pack Dependency**:
A Content Pack's declared requirement for another pack and compatible versions.

**Content Set**:
The selection of Content Packs used by a Character.

**Source**:
A publication or other identified origin of game content. A Content Pack can contain content from more than one Source.

**Content Provider**:
A local or remote place from which the application obtains Content Packs. It is distinct from the publication Source of their content.

**Replace**:
A declared content operation that supersedes an existing Element definition.

**Append**:
A declared content operation that adds to an existing Element without replacing the whole definition.

## Characters and decisions

**Character**:
A player's character, with recorded choices and possessions whose capabilities follow the applicable Ruleset and content. A Character can be incomplete.

**Owner**:
The account that manages a saved Character or private uploaded asset. A Character's Owner need not be the person who plays it.
_Avoid_: Player (when referring to ownership)

**Character Sheet**:
A view of a Character's build, possessions and capabilities.

**Sheet Entry**:
A named reminder or concise description shown on a Character Sheet. One Element Occurrence can supply several Sheet Entries; their visibility does not change its mechanical benefits.

**Build Completion**:
Whether a Character's required building decisions are valid. Optional details and unclaimed starting equipment do not determine it.

**Sheet Readiness**:
Whether the Character Sheet's required calculations are resolved, distinct from Build Completion.

**Character Option**:
An optional rule offered for a Character that the player can enable or disable.

**Selection**:
A place on a Character where a Rule offers a decision to be answered by a Choice.
_Avoid_: Slot

**Option**:
A candidate offered by a Selection, whether currently eligible or not. An Option is distinct from the Choice recording the player's answer.

**Choice**:
A recorded character-building or loadout decision, such as a selected feat or an ability score. It is the player's answer to an offered decision, distinct from the capabilities or possessions that result.

**Dormant Choice**:
A recorded Choice whose decision is currently unavailable. It is retained without contributing to the Character and may apply again when the decision becomes available, subject to revalidation.
_Avoid_: Orphaned choice, inactive choice

**Invalid Choice**:
A recorded answer that fails the applicable rules of an available decision. It remains player intent but is not a valid basis for the resulting capability unless an applicable exception permits it.

**Class Level**:
In D&D character building, the number of levels a Character has in one class.

**Character Level**:
In D&D character building, the sum of a Character's Class Levels.

**Primary Class**:
In D&D character building, the first class of a Character, which determines starting-class benefits.

**Ability**:
A fundamental Character attribute, such as Strength, represented by an Element.

**Ability Score Input**:
The score supplied for an Ability by the player or an ability-generation method, before applicable increases and limits.

**Effective Ability Score**:
The Ability score used by a Character's mechanics after applicable increases and limits.

**Manual Adjustment**:
A recorded user bonus or user-set value for a numeric Character mechanic, distinct from the rules-derived value.

## Spells and equipment

**Spell Access**:
A source-specific way a Character can cast a Spell, including its permitted methods and applicable modifications.

**Spell Learning**:
A Character's acquisition of a learned Spell through a particular source and, when required, class advancement.

**Spell Preparation**:
A Character's chosen set of Spells readied through a particular source under that source's rules.

**Owned Item**:
A particular item or homogeneous stack possessed by a Character, distinct from the Element defining its base kind.

**Base Item**:
An item's underlying kind before its Item Decorators are applied.

**Item Decorator**:
A reusable modification to an item's name, properties or capabilities.

**Decorator Application**:
A particular, independently identified use of an Item Decorator on an Owned Item.

**Equipment Loadout**:
A Character's arrangement of particular Owned Items intended to be worn, wielded or attuned, distinct from possession and eligibility.
_Avoid_: Inventory, loadout (unqualified)

**Equipment Allocation**:
A designation of how many units of an Owned Item are used in a particular role in an Equipment Loadout.

**Attunement**:
A Character's bond with a particular Owned Item when required by its rules, distinct from wearing or wielding it.

**Starting Equipment Receipt**:
A record of a starting package a Character has claimed and the possessions and currency it supplied, distinct from the corresponding Choice.
