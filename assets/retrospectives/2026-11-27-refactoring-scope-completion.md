# Guidance-based refactoring ended after one operation

## What happened

The request was to refactor the project according to updated C# guidance, with
the DND5E5 project tagged. The assistant treated that tag as a scope boundary
without confirming it, selected one operation, and ended after extracting a
private method in `DndBuildRules`. It announced an extraction of `View` helpers
but instead extracted package-item resolution from `ClaimEquipment`. It did not
account for the remaining project files or guidance findings.

## Impact and evidence

The change did not establish project-wide compliance. The
[C# guidance](../../docs/guidance/CSHARP-GUIDANCE.md) includes Ruleset
organization, class naming, braces and documentation, while the
[inventory](../learnings/2026-11-27-csharp-structure-inventory.md) records potential
project-wide findings. The
[refactoring skill](../../.agents/skills/csharp-refactoring/SKILL.md) specifies
one operation per *step*, but does not state when a broad multi-operation task
is complete; treating one step as the whole task was an execution error, not a
skill requirement.

The assistant built the Ruleset project and ran 242 Character unit tests, but
did not run the solution build or full .NET suite required by
[AGENTS.md](../../AGENTS.md), or report those omissions. Its first filtered test
invocation used incorrect native MTP arguments; it recovered by running the
test project unfiltered.

## Candidate improvements

1. For broad refactoring requests, add a skill completion criterion: determine
   the intended scope, account for each in-scope file and applicable guidance,
   execute safe operations one at a time, and report completed, unchanged and
   blocked work. Ask when a tagged file and the requested scope are ambiguous.
2. Before reporting completion, compare actual edits and validation against
   the planned work and repository verification instructions. Explicitly
   report any unfinished scope or unrun checks.
3. After resolving existing formatter workspace-loading limitations and style
   debt, add deterministic CI checks for agreed mechanical style rules. The
   [CI workflow](../../.github/workflows/development-build.yml) already builds
   and tests, but does not run the formatter checks. Keep responsibility and
   domain-boundary decisions as judgment-based review.
