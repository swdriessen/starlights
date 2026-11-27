# Broad C# refactoring request stopped prematurely

## What happened

The user invoked `csharp-refactoring` to refactor the project according to
updated guidelines, with the DND5E5 project file in context. The assistant
immediately replied that the request was not a behavior-preserving refactor
because no specific operation or target was provided. It read no repository
files and made no changes.

## Impact and evidence

The response did not establish whether the requested work was actually
behavior-changing. [AGENTS.md](../../AGENTS.md) points C# work to
[CSHARP-GUIDANCE.md](../../docs/guidance/CSHARP-GUIDANCE.md), which explicitly
applies to behavior-preserving refactoring. The existing
[structure inventory](../learnings/2026-11-27-csharp-structure-inventory.md) identifies
potential Ruleset and other candidates, but explicitly is not itself
authorization to change them. The user's later clarification identifies the
guidance as the intended basis for the request.

The skill's stop response is for requests outside behavior-preserving
refactoring, not for requests lacking a named operation. Its instruction to
choose one operation per step can be read alongside a prohibition on inventing
structural work to avoid a behavior-change stop; the previous response
incorrectly treated an unspecified operation as a behavior change.

## Candidate improvements

1. Clarify the skill's classification gate for broad guidance-based requests:
   read applicable repository instructions and the referenced guidance, inspect
   the requested scope, and identify concrete structural candidates before
   classifying them. Ask about scope if it remains consequentially ambiguous.
   Reserve the behavior-change stop for an actual behavior or contract change.
2. Add a skill regression scenario for a broad request with a repository
   guidance pointer and a project in context. Expect discovery and
   classification or a focused scope question, not the behavior-change stop.
3. The existing CI workflow builds and tests but does not run the formatter
   checks described in the structure inventory. Consider a deterministic check
   for agreed mechanical style rules after existing findings and workspace
   loading limitations are addressed; this would not replace the skill fix.
