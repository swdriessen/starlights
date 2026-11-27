# F02 reliable Character edits

## Scope

The request to implement F02 was expanded, at the requester's choice, to first
complete the missing F01 Human/Fighter/Soldier mechanics. The historical F01.1
create/list/reopen evidence is retained in [its record](f01-first-slice.md).

The bounded content and HTTP contracts are documented in
[REFERENCE-BUILD-API.md](../docs/reference/REFERENCE-BUILD-API.md). New Characters pin
`srd-5.2.1` version `0.2.0`; existing `0.1.0` pins are unchanged. Broader origin,
ability-generation, advancement and equipment slices are not implied.

## Delivered

- Content-defined choices, standard-array assignments, separate idempotent
  equipment claims, owned possessions, wearing/wielding and calculated sheets.
- Root-controlled mutations with atomic revision checks and durable operation
  receipts. Invalid input and conflicts leave saved facts unchanged.
- Calculation pending/result/failure state qualified by the saved revision.
  Accepted processing does not use browser-request cancellation. Startup
  explicitly fails interrupted calculations without discarding accepted facts.
- Owner-only SignalR invalidations, with HTTP/OpenAPI authoritative for reads
  and writes. Reconnection reloads saved state.
- Autosave retains queued local input, including typing during an earlier
  request. Unconfirmed retries retain their operation ID and original revision.
  Conflicts require explicit reapplication or discarding of unsaved input.
- Same-origin WebSocket proxying and antiforgery-protected SignalR transport
  requests. Generated API integers are checked for safe representation.

## Executed checks

From the repository root:

```powershell
dotnet build Starlights.slnx --no-restore -v minimal
dotnet test --solution Starlights.slnx --no-build
```

The initial integrated build passed with **22 warnings and 0 errors**. After
the fixture restart correction, the final incremental build passed with
**1 warning and 0 errors**. The final full suite reported
**Passed: 662, Failed: 0, Skipped: 17** (679 total).
Existing skipped acceptance backlog remains; one skip is the opt-in browser
fixture, which was executed separately for the browser checks.

From `src/frontend`:

```powershell
npm run build -w builder-app
npm run build -w @starlights/api-client
npm test -w builder-app
npx --no-install oxlint --config .oxlintrc.json apps\builder-app\src\features\characters apps\builder-app\vite.config.ts
```

Builder and API-client builds passed. **13 frontend tests passed**.
Changed-source lint passed. The existing six whole-builder lint errors in the
retained legacy details page remain unrelated and unchanged.
Repeated generation from the real integration-host OpenAPI export produced an
unchanged schema hash. The generated declarations were not hand-edited.

## Integrated browser evidence

Used VS Code's integrated browser, the real React/Vite UI, real
cookie/antiforgery authentication and Kestrel with isolated file-backed SQLite.
No successful Character response or session was mocked. Deliberate transport
faults were injected only for the failure scenarios.

1. Created a fresh Character, chose Human/Medium, Fighter and Soldier, and made
   the reference skills, feats, masteries, languages, increases and standard-array
   assignments through browser controls. Package selection left possessions
   empty and currency at zero.
2. Claimed both packages separately, wore Chain Mail and wielded Greatsword.
   After reloading, independently asserted all **20 numeric reference values**,
   Greatsword **+5 / 2d6+3 slashing / Graze**, required feature reminders,
   **two receipts**, **12 possession records** and **18 GP**.
3. Identified two genuinely distinct tabs. Observed **two CharacterChanged
   SignalR frames** and the other tab's saved-name update **8 ms after the save
   response**, without refreshing it.
4. Interrupted a request before acceptance, saved a competing edit in the
   other tab, then retried. The retry retained its original ID/revision and
   returned **409**. Local input stayed visible; explicit reapplication used a
   new operation and saved against the latest state.
5. Held an already-committed response while entering a later name. Both edits
   were submitted and the late acknowledgment did not replace newer input.
6. Reset a connection after the upstream save committed. Retrying the same
   operation confirmed the original accepted revision without incrementing it
   again.
7. Took the browser context offline and back online. Input remained visible;
   online recovery automatically submitted against its original revision.
   The offline antiforgery fetch failed before a POST could be sent.
8. Submitted a 101-character name. **400** left authoritative name/revision
   unchanged and retained the invalid input. Corrected input saved only after
   explicit reapplication.
9. At **390 px**, dark and light themes had no horizontal overflow. A fresh
   normal load produced no unexpected console errors. Errors during intentional
   offline/reset/400/409 scenarios were expected and visible.
10. Stopped the application process and restarted against the same fixture
    databases and account. The existing browser context reopened the Character
    without signing in again. A real authenticated read returned the unchanged
    name at **revision 33**, ready calculation, **two receipts**, **12 possessions**,
    the same worn/wielded items, numeric sheet values and Greatsword attack.
    The reopened UI still displayed **18 GP**. The fixture now reuses an existing
    account without resetting its password or revoking sessions, and removes
    stale startup sentinels; a focused regression test verifies account reuse.

## Remaining provider validation

The new SQL Server migration was generated, not applied to a shared database.
SQLite verifies relational transactions and file durability, not SQL Server
provider parity, database-process restart or the full Aspire graph. Those
checks require an isolated approved SQL Server environment.

Application-process restart passed with the same fixture databases and existing
authenticated browser context. Browser checks complement, rather than replace,
the automated pending/cancellation/restart integration cases.
