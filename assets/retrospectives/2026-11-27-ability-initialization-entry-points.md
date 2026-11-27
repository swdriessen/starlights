# Trace creation entry points before changing initialization

## What happened

F03-a initially added automatic allocation to `DndBuildRules.Initialize`.
Domain checks passed, but the HTTP creation workflow called
`DndRuleset.Initialize`, the minimal initializer. Two full-suite API assertions
therefore observed six missing inputs instead of the expected standard array.

## Impact and cause

The first full-suite run failed and required another build/run. The shared
initialization interface had two implementations with different responsibilities;
checking only the content-aware domain entry point missed the production caller.
The fix routes supported content through its mechanics initializer while
preserving minimal/legacy and other Ruleset initialization.

## Candidate improvements

- Before revising creation defaults, trace the actual HTTP creation caller
  and its selected initializer, not just the domain helper.
- Keep API assertions for the persisted revision-0 facts alongside domain
  checks, including untouched alternative Rulesets and old content bindings.
- Treat these as workflow improvement proposals, not new instructions.
