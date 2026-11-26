# Reference build and reliable edits

## Bounded content contract

New DND5E5 Characters select the sole current bundled Character Element and pin
`srd-5.2.1` **0.2.0**. The original **0.1.0** initialization pack and saved
compositions are retained unchanged; reopening never upgrades them. Summary
`buildSupported` identifies exact bindings with implemented mechanics. Legacy
APIs remain separate and reject initialized Characters.

The new pack covers the level-1 Human/Fighter/Soldier reference, not the full SRD.
Definitions and numeric values come from JSON; the independent oracle remains
[the reference journey](BEHAVIOR.md#first-checkpoint-human-fighter-soldier).
The official English PDF was retrieved from
https://media.dndbeyond.com/compendium-images/srd/5.2/SRD_CC_v5.2.1.pdf and the
reference checked against pages 13, 20–22, 47–48, 83, 86–88 and 91–92.
The pack includes the SRD attribution and CC-BY-4.0 license notice.

The supported `starlights-content/1` mechanics subset comprises:

- Stable Element and local Rule identities, acyclic Grants and Selections,
  fixed positive Selection counts, explicit identity/type matching and
  nonrepeatable ordinary answers. Ineligible and dormant answers remain facts.
- Character numeric `increase` Rules. Bounded integer literals, statistic
  references, arithmetic, parentheses and `floor` are interpreted, not compiled
  or executed. Missing player inputs remain unresolved; unknown authored
  references and unsupported mechanics fail validation.
- The representative inline `ability` property and reusable typed `properties`
  contracts: `standard-array`, `armor`, `weapon`, `weapon-mastery` and `starting-package`.
  Package items identify a definition or an explicitly named decision. An
  equipment Choice never acquires possessions; independent receipts retain the
  original acquired items and currency after subsequent Choice changes.
- One worn armor and one supported wielded weapon. Weapon profiles currently
  cover greatsword, flail and melee javelin. Inventory-only items are not offered
  for unsupported roles. Broader hand positions, ranged/thrown ammunition,
  attunement and receipt corrections remain outside this increment.
- Base sheet entries, occurrence-qualified entry identities, numeric literals
  and expressions, explicit `text` and `element-name` values, and single-pass
  `{{name}}` replacements. Raw HTML, images, unsafe link schemes and unsupported
  `:::` containers are rejected. Conditional groups/variants, hiding and broader
  contextual bindings are explicitly unsupported, not silently ignored.

Build Completion and Sheet Readiness are distinct: unanswered decisions can
coexist with resolved current statistics, while unassigned ability inputs can
leave numbers unavailable. Unclaimed equipment does not block completion.

## HTTP and acceptance

Authenticated owner-only `GET /api/characters/{characterId}/build` returns the
authoritative facts, current revision, calculation status and qualified sheet.
The five POST actions are `choices`, `ability-scores`, `equipment-claims`,
`equipment`, and `name`. OpenAPI and
[`CharacterBuild.http`](../src/modules/characters/Modules.Characters.Endpoints/CharacterBuild.http)
document their bodies. All mutations require the standard antiforgery header.

Each action requires a nonempty `operationId` and numeric `expectedRevision`.
Valid partial ability maps replace the recorded input map; duplicates,
unsupported abilities and values outside the content array are rejected.
Invalid input returns **400** without changing facts. A stale revision or
operation-ID payload mismatch returns **409**. Absent/other-owner resources
return **404**.

The accepted facts, incremented concurrency-checked revision and pending
operation receipt commit in one transaction. Processing then continues directly
in the application workflow with no request cancellation token. Its result or
safe failure is durable. A completed action normally returns **200** with the
latest authoritative snapshot. Other requests can observe **pending** during
calculation. Pending/failed responses contain no calculated sheet.

Retries reuse the original operation ID, expected revision and semantic payload.
Object-property order is normalized for its stored fingerprint. A successful
replay returns the latest snapshot plus **acceptedRevision**, the original
accepted revision—not a later remote edit mislabeled as this operation's
acceptance. Clients must use this receipt to reconcile their own queued edits.

Cached sheets are rebuildable and carry their exact revision. Finishing an older
operation never publishes its obsolete sheet over a later revision. Startup
explicitly marks interrupted pending operations failed while retaining accepted
facts. Submit another revision-checked edit to recalculate.

`/api/characters/notifications` is an authenticated SignalR hub. Server-assigned
groups derive exclusively from trusted current-user identity; no client group
or owner commands exist. `CharacterChanged` carries only
`{ characterId, revision, status }`; HTTP remains authoritative. Negotiation and
transport POSTs require the standard antiforgery header.

## Isolated browser fixture

Set `STARLIGHTS_BROWSER_FIXTURE_DIRECTORY` to a new descendant directory such as
`.agent-work\browser-fixture`, `STARLIGHTS_BROWSER_FIXTURE_PORT`,
`STARLIGHTS_BROWSER_FIXTURE_USERNAME`, and `STARLIGHTS_BROWSER_FIXTURE_PASSWORD`
in the process environment. Credentials are not command arguments or files.

```powershell
dotnet test --project src\modules\characters\Modules.Characters.Integration.Tests\Modules.Characters.Integration.Tests.csproj --filter "FullyQualifiedName~RunIsolatedBrowserFixture"
```

The opt-in test starts Kestrel at `http://localhost:<port>` with isolated SQLite
Character, Identity and file-metadata databases. It exports `openapi.json` and
creates a `ready` sentinel after successful HTTP readiness. Create the `stop`
sentinel in that same directory to end it. Normal test runs skip the fixture.
Restart with the same directory and username to reuse the existing account,
Character facts, sessions and protection keys. Startup removes stale `ready`
and `stop` sentinels before initialization; it writes a fresh `ready` only after
successful HTTP and antiforgery checks. An existing username is looked up with
the scoped Identity `UserManager` and is not recreated, renamed, re-enabled or
assigned a new password/security stamp. The password environment variable
remains required for creating a missing account, but does not reset an existing
account; use its original credentials when signing in again. Stop the previous
fixture before restarting in its directory.
The fixture neither migrates shared SQL nor creates containers. Blob upload
verification requires a separately isolated blob service; this Character fixture
does not provide one.
