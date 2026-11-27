# Ruleset renames need an explicit compatibility choice

## What happened

The DND5E5 refactor requested cohesive folders and contextual class names while
also asking to preserve public, serialized and runtime contracts. Repository C#
guidance and the refactoring skill direct public type renames toward obsolete
compatibility facades, so the implementation added 28 `Dnd*` facade types under
`Compatibility/`. The user later clarified that they do not want those classes.

## Impact and evidence

The implementation preserved the old public types but added a compatibility
layer the user did not want. This is a genuine contract choice: changing a
public CLR type name can break existing source and compiled consumers, whereas
moving files while retaining namespace and type names can improve organization
without adding facades. Persisted JSON shape is a separate boundary; the current
`CharacterComposition` can be tested against the old `{"Build":{"Stage":"awaiting-choices"}}`
payload without retaining the old CLR type.

The [C# guidance](../../docs/guidance/CSHARP-GUIDANCE.md) says to retain an
`[Obsolete]` facade for a public rename. The
[refactoring skill](../../.agents/skills/csharp-refactoring/SKILL.md) first
allows an explicitly authorized breaking change, but its later guidance-driven
rename section states the facade rule without that exception. The original
request asked to preserve public contracts, and the public
[ICharacterSheetCalculation](../../src/modules/characters/Modules.Characters.Endpoints/Building/ICharacterSheetCalculation.cs)
signature also names the legacy build types.

## Root cause

There was no explicit decision point separating two incompatible goals:
preserve the public CLR API with facades, or accept a breaking rename without
facades. The skill's later, more specific guidance-driven rule also obscured
its earlier breaking-change opt-out. The user's no-facade preference arrived
after implementation.

## Candidate improvements

1. Before renaming a public type, ask whether to preserve the old API with
   facades or authorize a breaking rename with no facades. If the answer is no
   facades, migrate all in-repository callers, including public interfaces in
   scope, and state that external callers must update. If no choice is given,
   keep the old type name and do only file/folder moves that preserve its CLR
   identity.
2. Align the C# guidance and refactoring skill: make the compatibility-facade
   rule conditional on preserving the old public API, and make the explicit
   no-facade authorization apply consistently to guidance-driven renames.
   Keep serialized-data checks separate: retain or migrate the wire shape
   through the canonical type and test old payloads, rather than assuming a
   CLR compatibility class is required.
3. When the user chooses no facades, make the completion check explicit:
   confirm no obsolete compatibility types were added for the renamed API and
   that all in-repository bindings and required old serialized payloads are
   covered.
