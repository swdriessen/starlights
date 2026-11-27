# F03-a automatic ability allocation

## Delivered

- New supported DND5E5 Characters persist standard-array assignments
  `[15,14,13,12,10,8]` in displayed order: Strength, Dexterity, Constitution,
  Intelligence, Wisdom, Charisma.
- Confirmed switches to standard array or application rolls initialize all
  assignments from authored array order or saved roll order. Rerolls replace
  all six dice and assign their new totals.
- Point buy initializes six 8s: 0 spent, 27 remaining. Physical totals remain
  unassigned until the player enters them.
- These are editable saved inputs, not fallback or class-priority guesses.
  Reopening, recalculation and same-method requests preserve customized,
  partial and absent inputs, including previously saved Characters.
- Supported HTTP creation uses the content-aware Ruleset initializer;
  legacy/minimal and other Ruleset initialization remains unchanged.
- Confirmation and creation copy describe automatic allocation accurately.
  No schema migration, pinned-content change or API-shape change is required.

Stable requirements are in [the behavior examples](../docs/specifications/BEHAVIOR.md#choose-an-ability-score-generation-method),
[API contract](../docs/reference/REFERENCE-BUILD-API.md#f03-ability-generation) and
[UI guidance](../docs/guidance/UI-DESIGN.md).

## Automated verification

From the repository root:

```powershell
dotnet build Starlights.slnx --no-restore -v minimal
dotnet test --solution Starlights.slnx --no-build
```

Final build: **1 existing ASPIRE012 warning, 0 errors**.
Full suite: **Passed: 676, Failed: 0, Skipped: 17** (693 total).
Focused cases cover initial allocation, manual revisions, same-method and
legacy-state preservation, point-buy budget, repeated roll totals, retained
dice, confirmed replacement, idempotency, security and application restart.

The first full-suite run caught two API creation assertions: the HTTP workflow
still used the minimal initializer. Routing supported builds to the
content-aware initializer fixed the cause; the subsequent full suite passed.
The first build also reported unchanged obsolete-driver/nullability warnings
in legacy integration/acceptance projects.

From `src/frontend`:

```powershell
npm run build -w builder-app
npm test -w builder-app
npx --no-install oxlint --config .oxlintrc.json apps\builder-app\src\features\characters
npm run lint -w builder-app
```

Production build, **13 frontend tests** and changed-source lint passed.
Whole-builder lint reports the same six legacy details-page errors:
three conditional hooks and three explicit `any` uses. Existing Vite
configuration/chunk-size warnings remain unchanged.

## Integrated browser verification

VS Code's integrated browser exercised the real React/Kestrel application,
cookie/antiforgery authentication and isolated file-backed SQLite:

1. Created a new Character at revision 0 with all six standard-array inputs
   assigned and other build choices still incomplete.
2. Reallocated Strength/Charisma through existing controls and reopened:
   `[8,14,13,12,10,15]` survived unchanged.
3. Canceled a method switch without losing those inputs. Confirmed point buy
   started at six 8s; editing Strength to 9 persisted across reload with
   1 spent and 26 remaining.
4. Application rolls automatically assigned `[14,13,8,13,8,10]`. Independently
   checked every total against the highest three original dice. Reload and
   switching through physical dice retained the same dice and totals.
5. Physical dice showed six blank numeric inputs. Switching back to rolls
   automatically reallocated the retained totals.
6. Canceling a reroll preserved revision. Confirmed reroll advanced one
   revision and assigned new totals `[11,11,14,14,11,16]`.
7. Switching back to standard array restored its initial allocation. A fresh
   authenticated reload had no page errors or failed API responses.

Verification services and isolated fixture data were cleaned up. No shared
SQL Server database, container or full Aspire graph was changed. SQL Server
provider parity and database-process restart remain deferred.
