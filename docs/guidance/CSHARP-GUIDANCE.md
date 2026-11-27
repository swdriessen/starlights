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
- Organize existing and new Ruleset code by cohesive domain concepts; do not
  keep unrelated implementations flat merely because their types are public.
  Add folders when they clarify ownership rather than for every class. Prefer
  namespaces that follow folders for new implementation types. When a refactor
  must retain a public namespace, move files without changing that namespace or
  use a narrow `.editorconfig` exception for those files; do not let
  IDE0130 alone veto folder organization.
- Generally mirror feature or domain folders in focused tests. Integration and
  end-to-end tests may span multiple features.

## Names and files

- Name each C# class file after its class and keep one class per file. Classes
  with the same base name and different generic arities may share that file
  (for example `Example<T>` and `Example<T1, T2>` in `Example.cs`). Generated
  code and partial classes may require exceptions.
- Within a Ruleset project, omit the Ruleset name from class names when its
  namespace already supplies that context. Use a distinguishing name when
  necessary to avoid genuine ambiguity. Apply this rule to existing
  implementations during a requested refactor, not only to new classes.
  Migrate in-repository callers when renaming; do not add compatibility classes
  for old names. A public CLR rename may break external callers: if preserving
  that API is required, ask before choosing between retaining the old identity
  and explicitly authorizing a breaking rename. Until decided, preserve the
  identity and continue independent work. Check runtime lookups and serialized
  data separately; preserve and test required payloads without assuming the old
  CLR type is needed.

## Style and verification

- Aim for no more than eight parameters in a constructor. Treat a larger
  parameter list as a prompt to reconsider the design, not as an absolute
  prohibition. Group related primitive values into a cohesive value object or
  record when that better represents the domain (for example, use a `Point`
  with `X` and `Y` instead of separate `x` and `y` parameters). Do not group
  unrelated values solely to reduce the parameter count.

- Add XML summaries to public types and members; use `<inheritdoc />` when
  implementing documented interfaces. Write summaries on separate lines, even
  when short, and leave a blank line between adjacent members so a summary
  does not immediately follow the preceding member:

  ```csharp
  /// <summary>
  /// Example.
  /// </summary>
  public string Example { get; }

  /// <summary>
  /// Another example.
  /// </summary>
  public string AnotherExample { get; }
  ```

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
