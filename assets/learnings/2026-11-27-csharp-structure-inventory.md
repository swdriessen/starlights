# C# structure and style inventory

Date: 2026-11-27

This is a review inventory, not authorization to refactor. Findings are
candidates for separately requested work; counts are a snapshot, not permanent
standards.

## Scope and method

- Surveyed the paths of all 970 Git-tracked C# files: 330 in Characters, 353
  Elements, 29 File Management, 27 Identity, 30 Rulesets, 61 Platform, 127
  integration and acceptance tests, 6 Aspire, 6 source-generator files and
  1 application file. Migration files are included in those counts.
- Inspected representative endpoint, workflow, Ruleset and multi-class files.
  Directory and size counts are navigation signals, not proof of poor design.
  The structural review is not an exhaustive manual review of every file.
- Ran `dotnet format style Starlights.slnx --no-restore --verify-no-changes`
  with `--severity info --diagnostics IDE0011 IDE0022`, and separately with
  `--severity warn`. The formatter reported findings without editing source.
  Counts below are unique reported file, line, column and diagnostic
  combinations, not a guarantee of every possible violation. The formatter
  also reports import-order changes even with the diagnostic filter.
- Workspace loading reported an obsolete Aspire AppHost SDK, a source-generator
  project reference without a matching metadata reference, and an unsupported
  frontend `.esproj`. Treat the diagnostic totals as incomplete until those
  load limitations are assessed.

## Structural candidates

1. **Character build endpoints and workflow.**
   [Building](../../src/modules/characters/Modules.Characters.Endpoints/Building/)
   contains 34 tracked C# files. [CharacterBuildEndpoints](../../src/modules/characters/Modules.Characters.Endpoints/Building/CharacterBuildEndpoints.cs)
   maps seven operations in one method, alongside notification setup.
   [CharacterBuildWorkflows](../../src/modules/characters/Modules.Characters.Endpoints/Building/CharacterBuildWorkflows.cs)
   combines edit dispatch, validation, revision and operation handling,
   persistence, calculation, notification and response mapping. Review
   operation-owned slices and cohesive shared mechanisms while preserving
   ownership, idempotency, accepted revisions, cancellation boundaries,
   notifications and calculation outcomes. Also review
   [CharacterEndpoints](../../src/modules/characters/Modules.Characters.Endpoints/CharacterEndpoints.cs)
   and [CharacterWorkflows](../../src/modules/characters/Modules.Characters.Endpoints/Building/CharacterWorkflows.cs),
   which combine multiple operations.
2. **Ruleset organization and naming.**
   [DND5E5](../../src/rulesets/Starlights.RuleSets.DND5E5/) has 28 C# files at
   its project root, all with `Dnd` or `IDnd` file-name prefixes.
   [DndBuildRules](../../src/rulesets/Starlights.RuleSets.DND5E5/DndBuildRules.cs)
   combines choice, ability and equipment operations with build evaluation;
   examine cohesive domain boundaries before moving or renaming types. Check
   callers and serialization or other observable names before any rename.
   The [TEST Ruleset](../../src/rulesets/Starlights.RuleSets.TEST/TestRuleset.cs)
   also has a Ruleset-prefixed class; assess whether a distinctive name is
   actually needed at its use sites.
3. **Other multi-operation endpoints.**
   [FileEndpoints](../../src/modules/file-management/Modules.FileManagement/Features/FileEndpoints.cs)
   maps five operations and [IdentityEndpoints](../../src/modules/identity/Modules.Identity/Features/IdentityEndpoints.cs)
   maps four. Their feature folders already contain some related contracts;
   review operation ownership without duplicating genuinely shared behavior.
4. **Multiple classes in a file.**
   [GetStatisticsResponse.cs](../../src/modules/characters/Modules.Characters.Endpoints/Generation/Statistics/GetStatistics/GetStatisticsResponse.cs)
   contains three differently named public classes, and
   [PlatformException.cs](../../src/platform/Starlights.Platform.Hosting/PlatformException.cs)
   contains three differently named exception classes. A text-based scan also
   found files with nested test helpers; review their ownership and the agreed
   generic, partial and generated-code exceptions before splitting anything.
5. **Large files worth targeted review.**
   [ElementsModuleInitializer](../../src/modules/elements/Modules.Elements/Services/ElementsModuleInitializer.cs)
   has 469 nonblank lines and initializes several content categories.
   [StatisticsCalculatorTests](../../src/modules/characters/Modules.Characters.Tests/Statistics/StatisticsCalculatorTests.cs)
   has 945 nonblank lines, and
   [CharacterEndpointsTests](../../src/modules/characters/Modules.Characters.Integration.Tests/CharacterEndpointsTests.cs)
   has 595. Size alone is not a reason to split; inspect responsibilities and
   test journeys before choosing boundaries. Generated migration snapshots are
   not ordinary feature-slicing candidates.

## Formatter findings

| Diagnostic | Reported locations | Meaning |
| --- | ---: | --- |
| IDE0011 | 183 | Braces suggested; 91 in DND5E5 and 19 in Character endpoint Building. |
| IDE0022 | 78 | Block method bodies required at warning severity; 13 in DND5E5 and 9 in Character endpoint Building. |
| IDE0130 | 153 | Namespace does not match folder. Review public and serialized names before moves or renames. |
| IDE0161 | 41 | File-scoped namespace preference; all 41 reported locations are in migration files. |
| IDE1006 | 9 | Private-field underscore naming errors reported by the formatter. |
| IDE0061 | 3 | Block local-function bodies. |
| IDE0021 | 2 | Block constructor bodies. |
| IDE0074 | 2 | Compound assignment. |
| IDE0040 | 1 | Explicit accessibility modifier. |
| IMPORTS | 17 | Import ordering; reported separately from the requested style diagnostics. |

The method-body warning is configured now; the agreed eventual target is an
error after the existing debt is addressed. IDE0011 remains a suggestion in
`.editorconfig`; the agreed future target is an error after inventory and
cleanup. Raising either to an error does not by itself establish build
enforcement; confirm the actual analyzer and build configuration before relying
on it as a build gate.

## Validation and next decisions

`dotnet build Starlights.slnx --no-restore --verbosity quiet` succeeded with
22 warnings and no errors. It did not demonstrate that all formatter style
diagnostics run during builds. The formatter checks exited nonzero because
changes would be required. No application code was changed, and tests were not
run for this documentation and configuration change.

Choose and request any refactoring separately. For each chosen area, establish
the behavior and external-contract boundaries, then verify the affected paths.
Do not treat generated migration output as hand-maintained feature code.
