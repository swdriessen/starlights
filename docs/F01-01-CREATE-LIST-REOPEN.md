# F01.1: Create, list and reopen a named Character

Status: implemented; acceptance remains blocked on SQL Server migration/restart
and F00 topology verification. Executed checks and workflow learnings are in
[the implementation progress record](../progress/f01-first-slice.md). This is
the first increment of [F01](./ROADMAP.md#f01-build-save-and-reopen-the-reference-character),
not the complete Human Fighter/Soldier checkpoint.

## Outcome and prerequisite

**User story:** As a signed-in player, I want to create a named Character, find
it in my collection and reopen it later, so that I can begin a build without
losing its identity or initialized state.

F00's remaining foundation verification is still a prerequisite for accepting
this increment. Refinement does not mark F00 complete or replace its full
Aspire, same-origin authentication and private-network checks.

The demonstrable player journey uses `DND5E5` and a bundled, version-pinned SRD
5.2.1 Content Pack. A separate `TEST` implementation proves that creation,
persistence and reads are not accidentally D&D-specific. It is never offered as
a playable Ruleset.

## In scope

- Create through the real React UI, generated API client, authenticated Minimal
  API, domain initialization and SQL Server storage.
- Show an owner-only Character collection, including its empty, loading and
  failure states; reopen a saved Character by its stable identifier.
- Show the saved name, selected Ruleset, pinned content and an explicit
  incomplete-build state. Do not present an empty builder as a finished sheet.
- Preserve the entire initialized aggregate across fresh requests, browser
  reload, application restart and SQL Server restart with the same storage.
- Exercise the synthetic Ruleset independently and alongside `DND5E5` through
  test-host composition, including its persistence round trip.

## Creation and domain contract

Follow the [shared aggregate and Ruleset-module contract](./SPECIFICATION.md#domain-composition-and-ruleset-modules).

1. The application obtains `OwnerId` from the authenticated user's stable GUID;
   the request cannot choose ownership. Common root state includes a stable
   Character identifier, name and immutable selected Ruleset.
2. Resolve a registered Ruleset case-insensitively and store its canonical
   uppercase identifier. Resolve the permitted Content Set to exact pack
   identities and versions and a compatible content-defined Character Element.
   The initial UI may select the sole supported option automatically.
3. The selected initializer returns its required typed composition; the shared
   factory creates and validates the root. Ruleset implementations do not
   persist, choose an owner or duplicate common root-creation logic.
4. The initial DND5E5 composition represents a build before player Choices.
   Record its selected Character Element and content bindings, with no chosen
   class, origin, equipment or invented answers. Only introduce typed parts
   needed to represent and validate this state; do not instantiate the legacy
   set of D&D components merely because existing readers require them.
5. Persist the root, ownership, content bindings and required composed state in
   one aggregate-wide Unit of Work. Return success only after commit; do not
   depend on an outbox worker to finish structural initialization.
6. Reconstitution preserves saved identities and authoritative values and
   validates structure. It does not rerun creation, regenerate parts or upgrade
   content to the latest version. Invalid structure, an unavailable Ruleset or
   unavailable pinned content is an explicit failure, not guessed defaults.

Parts are externally read-only, including their contents. Missing required,
duplicate single-instance and incompatible parts are rejected without partial
in-memory mutation or saved state. Domain tests exercise these invariants
without EF Core, HTTP or a DI container. Concrete type names and EF layout are
implementation choices, not a requirement for a generic component framework.

### Names

The primary journey supplies a name, for example `Mira`. Keep the existing
100-character limit and explain overlong input without creating a Character.
Permit duplicate names: identifiers, not names, distinguish Characters.

Carry forward the agreed unnamed-creation behavior: omitted, empty or
whitespace-only input uses `The Nameless One`. This is a placeholder, not a
uniqueness key or a reason to require a naming step. Name editing is not part
of this increment.

### Content boundary

Use the [representative JSON baseline](./CONTENT-EXAMPLE.md) and
[content contracts](./CONTENT-AND-COMPUTATION.md), not a hardcoded creation
option in the UI or initializer. Deliver only the validated loading and binding
subset needed to identify the bundled pack and its Character Element here.
Record the concrete pack identity, semantic version and Character Element
identifier in the implementation and its fixtures; do not use a floating
`latest` reference.

Grants, Selections, formulas and sheet evaluation remain subsequent F01
increments. Retain their declarations without treating unevaluated mechanics as
applied or complete. Unsupported content must not be silently discarded or
advertised as supported. Missing, invalid or incompatible creation content
blocks creation visibly before any Character is committed. A later pack
version does not silently rebind an existing Character.

### Test-only Ruleset

- Provide the small trusted `Starlights.RuleSets.TEST` module, referenced and
  registered only by test composition, not the ordinary application host.
- Give it a distinct required typed part with a deterministic non-D&D value
  that can be asserted after SQL reconstitution, plus minimal test content.
  Do not implement classes, ability scores, progression or a second game.
- Test `DND5E5` alone, `TEST` alone and both registration orders. The shared
  creation workflow and persistence must work in each supported test setup.
- Reject duplicate identifiers, including case-only duplicates, and ambiguous
  initialization explicitly. Missing implementations never fall back to D&D.
- Ordinary player-facing creation options exclude `TEST`; a manually supplied
  `TEST` request cannot enable it.

## HTTP and UI boundary

Replace the existing create/list/detail routes in place with Minimal APIs,
preserving their `/api` route family and generated OpenAPI coverage. Do not
register parallel FastEndpoints handlers for the same method and route.

| Operation | Observable contract |
| --- | --- |
| Create command | Accept name and a supported Ruleset/content selection; return 201 with the committed Character identifier and a detail location. Owner, composition and completion cannot be supplied by the caller. |
| Collection query | Return only the authenticated owner's Characters, with stable identifiers, names, Rulesets and incomplete status. Do not load D&D-specific components merely to render common summaries. |
| Reopen query | Return the authorized saved Character summary and pinned content information. Exercise full aggregate reconstitution in persistence tests even if this query uses a projection. |

Use 400 for invalid name length or an unsupported client selection; 401 for an
unauthenticated request; 404 for absent or another owner's Character. Require
antiforgery on create. Server-side registration, bundled-content and storage
failures remain explicit errors, not validation successes or empty lists.
Responses must not disclose another owner's data or private content in errors.

The UI disables duplicate submission while creation is pending and navigates
only after confirmed success. On failure it retains the entered name and shows
accessible feedback. Do not automatically retry a create whose outcome is
unknown after a lost response: explain the uncertainty and let the player
refresh the collection before explicitly trying again. Do not promise
exactly-once retries without an implemented deduplication contract.

Clear owner-scoped query data on account changes. A shared browser must not
show the previous owner's collection or detail from a client cache. Direct
detail navigation and refresh use the API, not an in-memory-only draft.

Log and trace creation, Ruleset resolution and persistence outcomes with
correlation and safe identifiers; exclude Character names, private content and
credentials. Read failures remain distinguishable from an empty collection.

## Verification and acceptance evidence

Add the executable checks alongside existing domain, integration and
acceptance tests. Expected initialization values come from explicit fixture
expectations, not a second invocation of the initializer under test.

| Layer | Required evidence |
| --- | --- |
| Domain | Valid incomplete creation; protected composition; required/multiplicity/compatibility rejection; rejected operations leave state unchanged. |
| Registration | Each Ruleset alone; coexistence in both orders; canonical casing; duplicate, missing and ambiguous implementations fail explicitly. |
| SQL integration | Full aggregate round trip for both Rulesets; stable root/part identities where applicable; exact content pins; injected failure after writes begin rolls back root and parts; malformed persisted structure is reported without repair. |
| HTTP integration | Create/list/reopen; duplicate names; placeholder naming; 100 accepted and 101 rejected; unsupported Ruleset/content; authentication, antiforgery and two-owner isolation; no duplicate routes; ordinary host excludes TEST. |
| Real browser | Sign in, create `Mira`, see incomplete status, return to collection, reopen and refresh the detail URL; repeat with a same-name Character and switch accounts on the same browser. |
| Restart | With isolated persistent SQL storage, create through the real UI, restart the application and database without resetting data, sign in if necessary, then list and reopen the same identifier, name, Ruleset and pins. Verify composed state through integration evidence. |

Run the full .NET build/test suite and applicable frontend build/lint checks.
Regenerate the API client and verify repeatable output. Execute the browser
journey against the real application and SQL Server, not a mock API, EF
in-memory provider or browser storage. Record executed evidence and remaining
blockers separately from this delivery contract; missing checks block
acceptance.

## Explicitly deferred

No species/background/class Choices, ability generation, item acquisition,
equipment claims, numeric calculations, sheet or PDF output, portraits,
renaming, deletion, autosave editing, SignalR or cross-tab edit recovery.
Do not introduce a mediator, universal change dispatcher, runtime plugin
loader, generic repository framework or speculative parts hierarchy.

Retained legacy Character routes must not mutate the new aggregate through
unchecked legacy component access; explicitly reject unsupported operations
for it while preserving authorized behavior for retained legacy data. Decide
and test the legacy-data migration boundary during implementation rather than
silently converting existing Characters to a guessed Ruleset or content pin.

This increment is accepted only when the create/list/reopen evidence passes
and F00's prerequisite gate is satisfied. F01 and milestone M1 remain open
until the full reference journey and independently checked sheet values pass.
