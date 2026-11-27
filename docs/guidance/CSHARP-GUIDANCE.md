# C# guidance

Apply this guidance to new and changed C# code, including agent-authored code.
Inventory unrelated existing issues separately instead of expanding a task into
an unrequested refactor.

## Features and responsibilities

- Organize implementation by vertical feature slice. Give each distinct API
  operation a feature folder and colocate its Minimal API endpoint, contracts,
  validation and handler where practical. A small composition method can
  register routes; feature implementations belong in their slices.
- Keep orchestration and components focused on cohesive responsibilities.
  Separate independently changing behavior when doing so makes ownership and
  changes clearer; keep it together when splitting would add complexity without
  a meaningful boundary. Use strategies or other polymorphism for genuine
  variation, not an interface or class for every conditional.
- Let each feature own its dispatch rather than requiring a shared switch to
  change for every new request type. A small switch over stable, related
  alternatives can still be the clearest solution.
- Place genuinely shared code under the nearest common feature in a folder named
  for its responsibility. Prefer independent implementations over sharing
  when the behaviors are likely to diverge and coupling would risk breaking
  another feature. Avoid generic catch-all helper folders.
- Organize Ruleset projects by cohesive domain concepts, not a flat list of
  unrelated types. Add folders when they clarify ownership rather than for
  every class.
- Generally mirror feature or domain folders in focused tests. Integration and
  end-to-end tests may span multiple features.

## Names and files

- Name each C# class file after its class and keep one class per file. Classes
  with the same base name and different generic arities may share that file
  (for example `Example<T>` and `Example<T1, T2>` in `Example.cs`). Generated
  code and partial classes may require exceptions.
- Within a Ruleset project, omit the Ruleset name from class names when its
  namespace already supplies that context. Use a distinguishing name when
  necessary to avoid genuine ambiguity. This is a class-naming rule, not
  permission to rename stable identifiers or external contracts.

## Style and verification

- Treat `.editorconfig` as the source of truth for configured style preferences
  and severities. Use braces for applicable control-flow statements, including
  `if`, `else`, loops and `using` statements.
- Resolve warnings introduced by the change and warnings in changed code before
  completion; report unrelated pre-existing warnings rather than fixing them
  incidentally. Use judgment for suggestions and silent preferences, favoring
  consistency with nearby code.
- Prefer simple conditional expressions for a clear two-branch choice; prefer
  readable control flow over dense or nested expressions.
- Apply this guidance during behavior-preserving refactoring as well as new
  work; preserve external contracts and verify behavior when moving or renaming
  code.
