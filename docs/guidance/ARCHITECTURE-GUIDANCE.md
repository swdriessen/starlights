# Architecture guidance

Apply this guidance to new and changed architecture, including agent-authored
work. The [specification](../specifications/SPECIFICATION.md) and
[content contracts](../specifications/CONTENT-AND-COMPUTATION.md) define
product and domain requirements; this document guides their implementation.

## Design and ownership

- Identify each responsibility, its owner, callers, invariants and reasons to
  change before introducing a shared contract, module dependency or Ruleset
  extension point. Keep unrelated behavior from accumulating in an
  orchestrator; a small coordinator may delegate cohesive responsibilities.
- Apply SOLID to actual variation and dependencies. Prefer focused contracts
  and cohesive implementations over a broad interface, a central switch for
  every new implementation, or an interface, project or abstraction for every
  class. File length alone is not a design criterion.
- Keep shared contracts Ruleset-neutral. Ruleset-owned implementations supply
  their own mechanics, validation and initialization through explicit
  contracts. Share behavior only when its meaning and invariants match across
  its consumers; adding content using supported mechanics must not require
  hardcoded generic-core cases.
- Keep module-owned data private. Other modules use explicit contracts or
  domain events, not direct writes to another module's tables. Keep domain
  behavior independent of HTTP, persistence and dependency injection.
- Make registration ownership, lifetimes, required dependencies, supported
  declarations and failures explicit. Missing, duplicate or ambiguous
  registrations are errors; registration order must not select a Ruleset
  silently. Runtime plugin discovery is not required.
- Preserve authoritative player intent, exact content bindings and persisted
  data. Trace production entry points and serialization, registration and
  public-contract consumers before restructuring. Separate
  behavior-preserving refactoring from new behavior or contract changes.
- Keep interfaces small enough that callers need not coordinate internal
  parsing, validation and evaluation steps. A shared module can contain
  multiple cohesive internal implementations without exposing each as a
  separate public contract.

## Independent architecture review

Before implementing a change that introduces or modifies a shared contract,
module dependency or Ruleset extension point:

1. Prepare a bounded proposal describing the problem, responsibilities,
   dependency direction, registration, compatibility risks, alternatives and
   verification. Cite the relevant existing code and requirements.
2. Obtain an independent review from `starlights-architect` in a separate
   agent execution, or from a human reviewer who did not produce the proposal
   or implement the change. The reviewer returns **approved** or **rejected**,
   with concrete findings and required changes.
3. Address rejection and obtain a new review before implementation. If no
   independent reviewer is available, stop and report the blocker; proceed
   only if the requester explicitly waives the gate.
4. Implement only the approved scope. If the design changes materially,
   obtain another review before implementing the changed design. Report the
   review outcome and verification with the delivery.

The review is a process requirement, not an automated permission boundary.
Use automated architecture checks where they can reliably enforce an agreed
invariant; review remains necessary for responsibilities and domain meaning.

## Specialist coordination

Default to [local workflow guidance](./LOCAL-WORKFLOW.md). Local tickets and
reports replace remote issue/PR steps unless the requester explicitly authorizes
remote operations. Local Git inspection does not authorize branch creation or
commits. The review and verification gates still apply.

For new or ambiguous requirements,
[starlights-specification-author](../../.github/agents/starlights-specification-author.agent.md)
refines the request using
[specification guidance](./SPECIFICATION-GUIDANCE.md). Requester approval
establishes intended behavior; implementation specialists then propose its
technical design. This is separate from independent architecture approval.

Use [starlights-orchestrator](../../.github/agents/starlights-orchestrator.agent.md)
when a request calls for coordinated specialist work. It delegates and collects
results; specialists own investigation, proposals, implementation and executed
verification. Its definition specifies routing, fresh-context briefs,
background execution, file ownership and worker reporting. Delegation does not
waive the independent architecture review gate.

For coordinated deliveries, obtain independent integrated verification from
[starlights-qa-reviewer](../../.github/agents/starlights-qa-reviewer.agent.md).
Implementation specialists retain their own testing responsibilities; QA
checks the original acceptance criteria without implementing repairs.
[starlights-git-ops](../../.github/agents/starlights-git-ops.agent.md) owns
authorized branching, issues, pull requests and CI/CD operations. Coordinate
checkout changes before workers start and tie delivery decisions to the
verified change snapshot. Assigning that role does not authorize publication,
merging or deployment.

Git Ops assembles authorized worker branches in dependency order before QA.
Application conflict resolution belongs to the implementation specialists;
Git Ops completes the Git operation after their repairs. For shared-checkout
work, identify the combined snapshot without requiring a branch merge.
Record the integration commit and any included uncommitted changes. Hold the
snapshot stable during QA; subsequent changes require renewed verification.

Assign verification resources in every execution brief: checkout/worktree,
build-output paths, ports, database/storage fixtures, browser profile or
session, and cleanup owner as applicable. Separate worktrees do not isolate
ports, services or explicitly shared outputs. Serialize checks sharing these
resources and wait for completion and cleanup before transferring ownership.
Independent checks may run in parallel only when their resources are isolated.
For direct specialist invocation, establish exclusive or isolated resources
before executing checks; report a blocker when ownership is uncertain.

Agent tool lists select the capabilities needed for their roles. Check the
actual runtime inventory before relying on an agent: the specification author
needs reading/search and assigned documentation editing, but no execution or
delegation; architect and Ruleset
reviewers need reading/search; QA needs execution and browser verification;
these reviewers may edit only their assigned local reports, not reviewed files.
Implementation and Git Ops agents need
editing/execution but no delegation; the orchestrator needs coordination but
may edit only assigned local request/status records and must not execute commands.
If required tools are unavailable, report the limitation
or use an allowed existing alternative. Tool filtering varies by harness:
execution can still modify files, and folder ownership and no-mutation rules
remain instructions, not filesystem security guarantees. Runtime inventory
and representative behavior must be verified separately from file metadata.
