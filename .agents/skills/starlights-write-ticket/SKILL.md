---
name: starlights-write-ticket
description: 'Draft or refine a local Starlights ticket for the orchestrator and specification author; preview it and save under .agent-work only after requester confirmation.'
---

# Write a local ticket

Turn a rough request into a clear intake ticket, not a specification or technical
design. The requester owns intent and scope; `starlights-specification-author`
owns the subsequent specification. Read
[local workflow guidance](../../../docs/guidance/LOCAL-WORKFLOW.md) before starting.

## Workflow

1. **Understand:** Read the supplied request and relevant local guidance or
   requirements. Preserve the original request separately from its clarified
   summary. Inspect only enough local context to identify constraints; distinguish
   verified current behavior from requester observations and assumptions.
   A tagged file is context, not permission to refactor it.
2. **Clarify:** Use the available user-question tool to resolve ambiguity that
   prevents a bounded request: the problem, desired outcome, scope or consequential
   behavior. Ask focused questions, not a full specification interview. Leave
   questions needing specialist evidence explicitly open; do not invent answers
   or prescribe interfaces, project layouts or implementation steps.
   If questioning is unavailable, return the draft and blockers without saving.
3. **Draft:** Use the ticket shape below. Make requested outcomes concrete enough
   for the specification author to refine into testable criteria. Label technical
   suggestions as proposals, not requirements, unless explicitly mandated by the
   requester. Omit irrelevant examples; use "None stated" for absent constraints
   and "Not yet determined" for missing evidence rather than silent assumptions.
4. **Preview and confirm:** Inspect existing request folders and propose a new
   `.agent-work/local-NNN/request.md`, with the next unused number above existing
   `local-NNN` folders (minimum three digits). Show the complete ticket and absolute
   destination. Use the user-question tool for explicit confirmation of that exact
   content and path. Changes require a new preview and confirmation. Decline,
   cancellation or missing confirmation leaves the filesystem unchanged.
5. **Save:** Only after confirmation, create the proposed folder and `request.md`.
   Recheck that the destination is unused and resolves inside this repository's
   `.agent-work/`; do not follow links outside it or overwrite existing records.
   If the path conflicts, propose a fresh path and reconfirm. Add the confirmation
   date and the requester's exact confirmation response, without invented identity
   or approval evidence. Verify the saved content matches the confirmed draft
   plus this confirmation evidence, and that Git ignores the file. Report write
   or ignore failures explicitly; do not claim a ready ticket if verification fails.
6. **Hand off:** Return the absolute saved path and a suggested instruction:
   "Use starlights-orchestrator with this request.md as input." Stop here unless
   the requester separately asks to start orchestration. Create no status,
   specification, design or review records, and make no source, Git or remote
   changes. Ticket confirmation approves intake and storage only, not a
   specification, implementation, remote access or local Git mutations.

## Ticket shape

```markdown
# <Short outcome-oriented title>

## Intake
- Request ID: local-NNN
- Revision: 1
- Mode: local
- Stage: confirmed intake; specification pending
- Intake confirmation: <Added only after explicit confirmation>
- Permissions: ticket storage only; no remote access or local Git mutations

## Original request
<Requester text, excluding secrets or sensitive data; identify any omission>

## Problem and desired outcome
<Who or what is affected, why this matters, and what should improve>

## Scope
- Included: <Bounded requested work>
- Excluded: <Explicit non-goals>

## Requested outcomes
- R1: <Observable outcome or preserved behavior; proposed input to specification>
- R2: <Another meaningful outcome, if needed>

## Constraints and compatibility
<Requester mandates, existing contracts to preserve, and known dependencies>

## Context and evidence
<Local paths, supplied observations, and verified facts, clearly distinguished>

## Open questions
<Unresolved decisions or evidence needed; identify the appropriate specialist>

## Handoff
Route to starlights-specification-author through starlights-orchestrator.
Refine requested outcomes into a specification and obtain separate requester
approval. Preserve applicable independent architecture and QA gates.
```

For an explicitly requested revision to an existing ticket, preview the full
replacement and destination, increment its revision, and obtain confirmation
before writing. Preserve original request and prior confirmation history.
Confirm ownership with the requester if orchestration has already started;
do not edit an active orchestrator-owned record concurrently.
