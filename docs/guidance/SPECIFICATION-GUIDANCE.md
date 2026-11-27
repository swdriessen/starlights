# Specification guidance

Use this guidance when turning an issue or user request into requirements,
refining a roadmap slice or changing an agreed specification. Specification
writing defines the intended outcome; implementation proposals describe how
to achieve it. Neither substitutes for the other.

## Ownership and document placement

- [starlights-specification-author](../../.github/agents/starlights-specification-author.agent.md)
  owns request refinement and approved product requirements. The requester
  decides scope and consequential behavior; the author does not self-approve.
- Read [the specification](../specifications/SPECIFICATION.md),
  [the roadmap](../ROADMAP.md) and applicable domain contracts first. Inspect
  implementation only to establish constraints and current behavior, not to
  treat existing code as the intended design.
- Update the existing authoritative document where the requirement belongs.
  Put observable scenarios in
  [BEHAVIOR.md](../specifications/BEHAVIOR.md); add a focused delivery contract
  under `docs/specifications/` only when it needs a separate scope. Link shared
  rules instead of copying them into every feature specification.
- Keep roadmap status in its current-status table and execution evidence in
  `progress/`. Specifications describe stable requirements, not work logs,
  approval history or claims of completion. Keep unapproved drafts and approval
  evidence in the conversation, issue or session workspace.

## A complete specification

Use the following as a checklist, not a mandatory new file or fixed template:

- **Intent and traceability:** identify the original request or issue, the user
  outcome and relevant roadmap slice and existing contracts.
- **Scope:** state included behavior, exclusions, prerequisites and the bounded
  increment. Refining one increment does not redefine its parent slice as done.
- **Behavior:** define meaningful inputs, outputs, defaults, constraints and
  important state transitions. Include applicable ownership, invalid/unresolved
  states, retention, interruption and compatibility behavior.
- **Acceptance:** give criteria stable identifiers within the specification,
  with concrete examples and observable expected results. Include important
  success, error and edge cases. Use measurable bounds and output shapes where
  needed rather than labels such as "correct", "fast" or "user-friendly".
- **Evidence and decisions:** cite independently established rules evidence for
  mechanics, label assumptions and list unanswered decisions. A test expectation
  derived only from current implementation is not independent rules evidence.

Specify technical contracts when required for observable interoperability;
leave internal classes, project layouts and algorithms to the implementation
proposal unless an existing architectural requirement constrains them.

## Approval and handoff

1. Clarify consequential ambiguity before deciding behavior. In coordinated
   work, the orchestrator obtains requester decisions and routes expert
   questions; specialists report back rather than spawning their own agents.
2. Present the exact draft for explicit requester approval. Existing approved
   requirements may be reused without creating another specification or
   requiring repeated approval of unchanged behavior. For a behavior-preserving
   refactor, reference existing requirements and identify preserved contracts.
3. After approval, persist the assigned changes and hand off the request,
   criterion identifiers, authoritative document paths and approval evidence.
   Implementation specialists prepare technical proposals; apply the separate
   [architecture review gate](./ARCHITECTURE-GUIDANCE.md#independent-architecture-review)
   when required. Product approval does not waive that gate.
4. If implementation reveals a material requirement change, return it to the
   author and requester before implementing that changed behavior. QA verifies
   the approved criteria against the delivered snapshot; neither implementation
   nor QA may silently rewrite acceptance criteria to match existing code.
