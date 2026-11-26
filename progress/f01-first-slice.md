# F01.1 implementation and workflow learnings

## Status

Implementation is committed. F01 acceptance is **verified**, as confirmed by
the user on 2026-11-26. This record supplements the
[delivery contract](../docs/F01-01-CREATE-LIST-REOPEN.md); it does not weaken it.

Acceptance verification date: 2026-11-26. The implementation and checks below
were recorded on 2026-11-25; detailed results of the final verification were
not recorded here.

## Delivered

- The shared Character root owns identity, account ownership, name and canonical
  Ruleset selection. Trusted Rulesets supply validated, immutable typed state.
- `DND5E5` initializes an `awaiting-choices` build. Its bundled JSON content pins
  `srd-5.2.1` version `0.1.0`, Character Element `srd521:character:default`.
  This is initialization content, not an implemented SRD mechanics engine.
- `TEST` has distinct synthetic state and a separate embedded pack. Only test
  composition references/registers it; the ordinary host rejects its selection.
- Root and Ruleset-specific state are saved in one EF unit of work. Reopening
  validates the saved state and exact content binding without initialization,
  upgrades or guessed repairs.
- A SQL Server migration adds nullable `ruleset_id` and the one-to-one
  `characters.character_build` table. Existing rows remain explicitly legacy.
  New Characters cannot be changed through the retained legacy builder APIs.
- Authenticated Minimal APIs replace create/list/reopen in place. Collection
  reads are owner-filtered, projected and paginated, with 50 visible records and
  a lookahead. OpenAPI includes the retained and new contracts.
- The React journey has lazy-loaded collection, creation and saved-overview
  pages using the existing shared shadcn/Base UI components. It displays
  incomplete state, exact pins, loading, empty and explicit failure states.
- Duplicate names and unnamed creation work. Pending creation disables inputs;
  transport failures retain the name and explain that a commit may have
  succeeded. Creation is never automatically retried. Both success and failure
  invalidate the collection so checking it does not reuse a fresh-but-stale list.
- Existing signed-in, per-account query-client isolation is retained.

## Executed evidence

### .NET

Commands from the repository root:

```powershell
dotnet build Starlights.slnx --no-restore -v minimal
dotnet test --solution Starlights.slnx
```

- Full solution build passed. Existing package, obsolete harness and nullable
  warnings remain; this was not a zero-warning build.
- Final full suite: **Passed: 646, Failed: 0, Skipped: 16** (662 discovered).
  Existing ignored/backlog acceptance cases remain skipped.
- The new Characters integration project contributes 23 passing cases:
  Rulesets independently and together in both orders; fresh-context round trips;
  rollback after an INSERT; invalid saved JSON without repair; real cookie and
  antiforgery workflows; owner isolation; application-factory restart using the
  same database files; names; pagination; unsupported selections; TEST exclusion;
  unique routes; OpenAPI; and missing state/Ruleset/pinned-content errors.
- Pure domain/content tests exercise immutable composition, compatibility,
  registration and exact content resolution separately from EF and HTTP.
- Legacy create, duplicate-name collection and deletion scenarios pass.

Relational tests use temporary, file-backed **SQLite**, not EF's in-memory
provider. They exercise real transactions and durable files, but do not validate
SQL Server migrations, SQL Server restart behavior or provider parity.

### Frontend and contracts

Commands from `src/frontend`:

```powershell
npm run build -w builder-app
npx --no-install oxlint --config .oxlintrc.json apps\builder-app\src\features\characters apps\builder-app\src\main.tsx
npm test -w builder-app
npm run build -w packages/api-client
```

- Final builder-app and API-client builds passed. The existing large-chunk and
  Vite native-config warnings remain.
- Changed-source lint passed; existing frontend configuration tests passed 3/3.
- Whole-app `npm run lint -w builder-app` was also executed. It fails on six
  pre-existing conditional-hook/explicit-any errors in the retained, no-longer-
  routed legacy `src/pages/characters/details/Index.tsx`. Those files were not
  changed to hide the baseline.
- Exported OpenAPI through the real integration host, regenerated
  `schema.d.ts` using the existing generation script, and compared its SHA-256
  before/after repeat generation: unchanged. No hand-edited generated contract.

### Real browser, isolated persistence

Used the actual React app, Vite same-origin proxy, Kestrel-hosted application,
real cookie authentication/antiforgery, and isolated SQLite databases. No mock
session or mock successful Character API was used.

Observed checks:

1. Sign in, see the empty collection, create `Mira`, and see the saved name,
   `DND5E5`, exact content pins and incomplete status.
2. Reload the detail URL, create another `Mira` with a different identifier,
   return to the collection, reopen and reload the original.
3. Stop/recreate the owned application host while keeping its database files
   and listening port; reload the same URL with the same saved identity, pins
   and session. This is a host restart, not a SQL Server/process restart test.
4. Reject 101-character form input visibly. Create with a blank name and show
   `The Nameless One`.
5. Inject a connection reset **after the real POST has returned 201 upstream**.
   The UI reports an unconfirmed outcome, retains the name, sends one POST only,
   and finds the committed Character when checking the collection.
6. Sign out and sign in as a second account in the same browser without a page
   reload between accounts. Its collection is empty; opening the first owner's
   detail URL returns 404 rather than cached private data.
7. Interrupt collection GETs: the UI shows an error, not an empty collection;
   remove the interruption and recover with Try again.
8. Check the creation page at 390px in both themes: no horizontal overflow.
   Desktop saved-overview rendering was also inspected.

The browser fixture and Vite process were task-owned helpers, not replacements
for the normal Aspire startup path. Temporary accounts/databases are not demo
data, and no credentials were added to application configuration.

## Gates recorded as open at implementation close (2026-11-25)

The following items were recorded as open before the user confirmed F01
acceptance on 2026-11-26. They are historical status, not current F01 blockers.

- Review/apply the new Characters migration to an **approved isolated SQL
  Server** instance. Check preservation of existing data and real provider
  behavior.
- Run the real UI create/list/reopen journey there. Restart the application
  process and SQL Server using the same persistent storage, then reopen the same
  identifier and verify composed state.
- Complete F00's full Aspire graph, migration ordering/failure visibility,
  same-origin authentication and developer-tunnel/private-network restrictions.
- Resolve or explicitly track the existing whole-app lint baseline separately.

At the time of this implementation record, permission to create/migrate/restart
SQL verification resources had not been obtained. The pre-existing shared SQL
container was not migrated or restarted. The subsequent user confirmation
establishes F01 acceptance; the detailed SQL Server/Aspire verification results
are not captured in this file.

## Learnings for the agentic workflow

1. **Verify the ownership of resources before operating on them.** A listening
   SQL container or shared browser is not authorization to migrate, restart or
   delete it. Separate implementation evidence from blocked environment gates.
2. **Use small vertical commits, then run the wider suite.** Targeted SQLite
   and single-Character scenarios passed while multi-Character legacy tests
   found that sorting a generated strongly typed ID was not supported by EF
   in-memory. Ordering by the converted GUID fixes both tested providers without
   loading an unbounded collection or changing the identifier generator.
3. **Test response loss after commit, not only rejected requests.** Browser
   fault injection exposed a fresh query-cache window hiding the committed
   Character. Invalidating on either mutation outcome repaired the actual
   recovery journey; disabling retries alone was insufficient.
4. **Wait for the observable outcome, not the first matching old element.**
   A collection can temporarily display cached cards during a refetch. Browser
   assertions should await the newly saved identifier/card or completed request.
5. **Initialize test schemas before starting hosted services.** Accessing a
   WebApplicationFactory's services starts background workers and Data
   Protection. Creating the SQLite schemas first prevents worker startup races
   and misleading disposed-host failures.
6. **Check the installed shadcn base.** This repository uses Base UI. Shared
   buttons compose router links with `render`, not Radix's `asChild`; use the
   package exports and semantic theme tokens rather than copying primitives.
7. **Respect generated and ignored files.** Export the real OpenAPI document,
   regenerate contracts and prove repeatability. The new `.http` example needed
   an explicit force-add because HTTP scratch files are ignored; do not broaden
   ignore rules or accidentally commit unrelated staged work.
8. **Record tooling mismatches rather than silently upgrading.** SDK 11 solution
   additions needed `--include-references false` to avoid relocating existing
   dependencies. The installed EF CLI was older than the runtime; migration
   generation warned, so real SQL validation remains particularly important.
9. **For external browser fixtures, make hosting assumptions explicit.**
   WebApplicationFactory content-root discovery differs outside the checkout.
   Kestrel binding, content root and local HTTP-only fixture cookie overrides
   belong in temporary verification setup, never production defaults.

## Commit trail

| Commit | Scope |
| --- | --- |
| `948c528` | Refined delivery contract and behavior examples |
| `616fac8` | Shared initialization and independent Ruleset modules |
| `cd538b9` | Atomic persistence, migration and relational tests |
| `72f15c2` | Owned Minimal APIs, real-cookie tests and generated contracts |
| `967b4b3` | Character collection, creation and saved-overview UI |
| `1c4ad20` | Provider-compatible paging and failure-state regression tests |
| `33aa613` | Fresh collection after an uncertain creation result |

Unrelated staged AppHost and frontend-readiness edits were kept out of these
commits.
