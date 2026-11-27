# F03 ability generation

## Delivered

- Standard array, 27-point buy, application 4d6-drop-one-lowest rolls and
  player-provided physical totals, using the existing revision-checked edit
  pipeline and generated OpenAPI client.
- Immutable generation state in the existing composition JSON. No schema
  migration or change to pinned content is required. Earlier compositions
  without generation state use standard array without clearing their inputs.
- Confirmed method changes clear assignments and retain previous application
  dice. Explicit rerolls replace all six results and clear assignments.
  Idempotent retries return accepted results instead of generating again.
- Multiset allocation permits repeated totals only as often as rolled.
  Point buy permits partial input and unspent points; physical totals are
  integers from 3 through 18 and are not represented as verified dice.
- Separate recorded input, ordinary score, maximum, effective score, modifier,
  missing-answer/fallback issues and source origins. Content default input is
  honored; absent input without a default uses 0 without completing the decision.
  Invalid recorded input remains unresolved rather than becoming fallback.
- An accessible shared-component editor with replacement confirmation,
  visible saved point accounting, original dice, expandable sources and
  retained rejected input with actionable server validation messages.

Stable contracts and observable examples are in
[REFERENCE-BUILD-API.md](../docs/REFERENCE-BUILD-API.md) and
[BEHAVIOR.md](../docs/BEHAVIOR.md#choose-an-ability-score-generation-method).

## Executed automated checks

From the repository root:

```powershell
dotnet build Starlights.slnx --no-restore -v minimal
dotnet test --solution Starlights.slnx --no-build
```

Build passed with **1 existing ASPIRE012 warning and 0 errors**.
The full suite passed: **Passed: 675, Failed: 0, Skipped: 17**
(692 total). Skips include the existing acceptance backlog and opt-in browser
fixture. Focused additions exercise all eight point-buy costs and modifiers,
duplicate roll allocation, rejected root operations, explicit fallback,
effective-score limits and dependent checks, HTTP validation, ownership,
antiforgery, idempotent roll/reroll replay and restart durability.

From `src/frontend`:

```powershell
npm run generate -w @starlights/api-client -- http://localhost:55341/openapi/v1.json
npm run build -w @starlights/api-client
npm run build -w builder-app
npm test -w builder-app
npx --no-install oxlint --config .oxlintrc.json apps\builder-app\src\features\characters
npm run lint -w @starlights/api-client
npm run lint -w builder-app
```

Generation used the real integration fixture's OpenAPI document; generated
declarations were not hand-edited. Both builds, **13 frontend tests**,
changed-source lint and whole API-client lint passed. Whole builder lint still
reports the six unchanged legacy errors in its retained details page
(conditional hooks and explicit `any`). Existing Vite configuration/chunk-size
warnings remain unchanged.

## Integrated browser checks

Used VS Code's integrated browser, the real React UI and Kestrel with isolated
file-backed SQLite and real cookie/antiforgery authentication. No successful
API responses or generation results were mocked.

1. Created a fresh Character and observed standard array selected, no
   assignments, explicit fallback labels and incomplete build status.
2. Assigned `[15,14,13,8,10,12]` through browser controls and independently
   asserted modifiers `[2,2,1,-1,0,1]`.
3. Canceled a point-buy switch and verified inputs remained unchanged.
   Confirmed the switch, assigned `[15,15,15,8,8,8]` and observed 27 spent,
   zero remaining. Changing the last 8 to 9 returned 400 with the budget reason,
   retained local 9 and left saved input at 8. Correcting and explicitly
   reapplying saved successfully.
4. Generated six application results and independently checked each total
   against the largest three original dice. Allocated every result and
   reloaded without changing dice or assignments.
5. Canceled a reroll without changing revision or results; confirmed a reroll
   and verified one revision increment and cleared assignments.
6. Switched to physical dice, entered `[3,18,18,3,10,12]` and independently
   asserted modifiers `[-4,4,4,-4,0,1]`. Input 2 returned 400 with the range
   reason and retained input. Correcting and reapplying succeeded.
7. Switched back to application rolls without changing retained dice. An
   unrelated name edit did not reroll. A naturally generated pair of totals
   12 could be assigned twice; a third 12 was disabled. The saved inputs were
   `[12,12,9,14,11,15]`.
8. Checked a 390-pixel viewport in dark and light themes, expanded source
   evidence, and verified no horizontal overflow (client and scroll widths
   were both 375 pixels with the scrollbar). A fresh authenticated load had
   no page errors or unexpected failed API responses.
9. Stopped and restarted Kestrel against the same fixture databases and
   protection keys. The existing browser session reopened without sign-in:
   revision 13, the same six original dice, inputs `[12,12,9,14,11,15]` and
   selected application-roll method survived. Restart-related 502/WebSocket
   failures were expected during downtime; the subsequent load was healthy.

Temporary verification services and isolated fixture data were cleaned up.

## Remaining provider validation

SQLite checks do not establish SQL Server provider parity, database-process
restart or the full Aspire graph. No shared SQL Server database or container
was changed. Those checks require an isolated approved environment.
Setting effects and dynamic maximum-changing content remain outside this
bounded generation slice; literal ability maximums and ordinary increases
are supported and tested.
