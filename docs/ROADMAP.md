# Roadmap: epic and feature slices

Status: target delivery sequence for refactoring this repository toward the agreed first release. The .NET 11 migration and first Identity increment of F00 are implemented; private blob storage and the rest of F00 remain open. The React builder route is still a placeholder. Existing character and content code is partial implementation input, not completion evidence. Reassess each slice against its acceptance criteria before marking it done.

## Epic: build a dependable SRD character from concept to sheet

**User story:** As a TTRPG player, I want to create, understand, save and revise a character using supported game content, so that I can bring an accurate and readable character sheet to the table.

**Target product:** the [agreed first release](./SPECIFICATION.md#first-release): a private, web-based SRD 5.2.1 character builder supporting all SRD classes at levels 1-20, multiclassing, relevant equipment and magic items, and on-screen and PDF sheets.

The epic is complete only when the release gates below are met. Completing the first checkpoint or a subset of classes does not redefine the product as finished.

## How to use this roadmap

- Each numbered slice normally delivers user-visible behavior through content, domain behavior, persistence, application workflow, UI and tests as needed. F00 is the bounded platform foundation for the existing application: it upgrades and secures the current API/React/Aspire setup without redesigning the Character domain or building a general rules engine. Avoid separate "build the engine," "build the database" and "build the UI" phases thereafter.
- Numbers indicate a suggested sequence, not a requirement to finish every lower number first. The listed dependencies are prerequisites; independent features may proceed separately once their prerequisites are satisfied.
- These are high-level feature slices, not estimates or sprint-sized tickets. Before starting a large class/progression feature, refine it into smaller end-to-end stories with concrete content and independently checked expected results. Keep the parent feature open until its full scope works.
- Before implementing a slice, resolve only the open contracts it needs and add its observable examples to [BEHAVIOR.md](./BEHAVIOR.md). Consult [the content contracts](./CONTENT-AND-COMPUTATION.md), [character mechanics](./CHARACTER-RULES.md) and [the JSON baseline](./CONTENT-EXAMPLE.md) rather than inventing a parallel specification.
- Carry forward earlier features and regression checks. New content is not considered supported if its required decisions or persistent sheet effects are missing.
- Accessibility, authorization, diagnostics, safe rendering and persistence correctness apply from the first feature that uses them. Later features extend those guarantees; they are not permission to postpone them.
- Preserve [domain-first composition and independent Ruleset modules](./SPECIFICATION.md#domain-composition-and-ruleset-modules). Add the smallest typed contracts needed for the slice; persistence adapts to the domain. Do not construct a speculative framework for all future games.
- Implement slices with [command/query separation](./SPECIFICATION.md#feature-workflows-and-cqrs), sharing commands only where their domain semantics match across Rulesets.
- Use Minimal APIs for new and replaced HTTP features. Existing FastEndpoints routes may remain temporarily during migration, with equivalent security, validation and OpenAPI coverage; retire FastEndpoints once the retained routes have been migrated.

## Milestones

| Milestone                             | Demonstrable result                                                                                                                           |
| ------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------- |
| M0: Platform foundation verified      | F00 upgrades the existing app to the agreed stack and verifies authentication, ownership, generated API contracts and private local storage. |
| M1: First usable character            | F01's reference character can be built, saved and reopened with verified results.                                                             |
| M2: Everyday building                 | Reliable editing, ability methods, origin choices, advancement and ordinary equipment work through F02-F06.                                   |
| M3: Full class journeys               | F07-F12 complete all SRD class progressions and multiclass creation, including direct creation at higher levels.                              |
| M4: Complete first-release experience | Relevant traits and magic items, contextual sheets, adjustments, portraits, PDF, content updates and operator workflows work through F13-F20. |
| M5: Release verified                  | The release gates pass across the combined product, not just isolated feature demonstrations.                                                 |

Milestones summarize outcomes, not additional implementation layers. Some M4 features can be delivered before M3 is complete.

## Feature slices

### F00: Establish the platform foundation

**User story:** As the developer, I want the existing application to run on the agreed .NET 11 and React/Vite stack with secure identity, a generated TypeScript API client and private local storage, so that Character work starts on a dependable foundation.

**Progress:** the .NET 11 migration and first Identity increment are implemented. Operator services, cookie-session persistence/revocation, antiforgery, owner isolation on retained Character routes, the React sign-in/sign-out gate, same-origin Vite proxy and OpenAPI-generated contracts are present. The full .NET suite passes (583 passed, 16 existing skips), frontend workspaces build, changed frontend sources lint, and repeated generation covers 71 API paths. SQL Server migrations and account persistence were checked in an isolated container; full Aspire startup and private blob storage/emulator behavior are not yet established. Unrelated dependency advisories and existing full-frontend lint violations remain. F00 is not complete.

**Scope:** upgrade the current .NET 10 API to .NET 11; retain React/Vite/Shadcn, Aspire and SQL Server; establish feature-organized Minimal API endpoints for the foundation and migrate existing FastEndpoints routes as needed without duplicate routes; generate the TypeScript API client and contract types from OpenAPI; establish Identity (`Modules.Identity`) with ASP.NET Core Identity cookie authentication and antiforgery; provide secure local operator commands to create accounts, reset passwords and disable/re-enable accounts; enforce owner-only access through GUID-based module contracts; add Azure Blob Storage with a local emulator and API-mediated authorization; route local browser `/api` requests through Vite; and retain the developer tunnel only as an explicit, authenticated development option. Resetting the existing development database is allowed. Do not redesign Character/Element aggregates or build a general calculation engine in F00.

**Identity boundary:** Identity owns accounts, credentials, authentication, account-role assignments and shared access integration. Other modules register resource-specific policies and enforce ownership without exposing Identity entities or depending on each other's tables. Use standard roles/policies/handlers, not a configurable permission engine. Owners use the stable user GUID as `OwnerId`, with no separate Player entity or administrator ownership bypass. External sign-in/linking, account/role-management UI, game-defined security roles and Character transfer remain outside F00.

**Done when:** focused browser/API checks verify operator-provisioned username/password sign-in and sign-out without required email, owner isolation, antiforgery on writes, account and valid-session persistence across restart, the eight-hour inactivity timeout without remember-me, and session revocation by the next API request after password reset or disabling. Re-enabling does not restore revoked sessions. Verify 401/403/404 semantics without HTML API redirects, repeatable OpenAPI client generation covering retained routes, and private blob read/write/delete through authorized API paths. Foundation endpoints use Minimal APIs; remaining legacy routes are not left unsecured or registered twice. Aspire starts SQL Server and the local blob emulator; migration/startup failures remain visible. F00 stays domain-light and introduces no dummy Character aggregate, generic repository framework or speculative component framework.

**Depends on:** none.

**Detailed contract:** [Application architecture](./SPECIFICATION.md#application-architecture), [Identity and access](./SPECIFICATION.md#identity-and-access), the [foundation behavior examples](./BEHAVIOR.md#f00-identity-and-access) and the [local/private-network requirements](./SPECIFICATION.md#first-release).

**First increment:** an operator provisions a local account and the user signs in and out through the real React UI, generated API client, Minimal APIs and durable Identity storage on .NET 11. Include antiforgery, protected retained routes and the agreed session lifecycle; demonstrate two accounts, restart and revocation without introducing a dummy Character. Complete private asset storage and its owner-isolation checks in subsequent F00 increments; sign-in alone does not finish F00.

### F01: Build, save and reopen the reference character

**User story:** As a player, I want to build a level-1 Human Fighter with the Soldier background and reopen it later, so that I can trust the builder with a real character.

**Scope:** use the completed F00 application and configured sign-in; load the bundled, versioned SRD 5.2.1 JSON pack; initialize the Character through the selected Ruleset; record the reference Choices; separately claim class and background equipment; equip the character; save and display its calculated sheet.

**Done when:** the [reference journey and expected values](./BEHAVIOR.md#first-checkpoint-human-fighter-soldier) pass through the browser with real content and durable storage, including save/reopen and application restart. Identity, grants, choices and possessions are not hardcoded to this one character. Private access and visible save/calculation failures work for this journey.

**Depends on:** F00.

**Acceptance evidence:** [the reference journey and expected values](./BEHAVIOR.md#first-checkpoint-human-fighter-soldier). A separate slice document will be created only when this feature is ready to refine.

Resolve the minimum aggregate, initialization, content-format, evaluation and Character persistence contracts as part of F01; they are not moved into F00. Preserve the shared Character root, validated Ruleset-owned composition, pinned content and aggregate-wide Unit of Work described in the specification. Exercise domain invariants without EF and verify persisted aggregate state survives reopening.

**First increment after F00:** a signed-in player creates a named Character for `DND5E5` using the bundled, pinned SRD pack. The Ruleset initializes its required composition and the application persists it atomically; the player can list and reopen it, with its incomplete build clearly indicated. Exercise this through the real UI, API and persistence. Introduce the minimal test-only `TEST` Ruleset in this increment to verify Ruleset selection and initialization without accidental DND5E5 coupling, including independent and coexistence registration. It is not a playable game or a speculative framework. Add Choices, possessions and sheet calculations in subsequent end-to-end increments; this first increment does not claim to produce a complete sheet or finish F01.

### F02: Revise a character without losing work

**User story:** As a player, I want edits, interruptions and concurrent tabs to have predictable outcomes, so that my latest decisions are not silently lost.

**Scope:** autosave and calculation status; stale-response protection; conflicting-edit detection; temporary disconnection handling; accepted work surviving browser closure; recovery or explicit failure after server interruption; authenticated SignalR notifications for status and cross-tab revision changes. HTTP/OpenAPI remains authoritative for edits and reads; reconnecting clients refetch the latest saved revision.

**Done when:** focused browser/integration scenarios demonstrate retained unsaved input, no silent overwrite, explicit pending/result/failure, and no lost accepted operation. Rejected edits leave authoritative state unchanged.

**Depends on:** F01.

**Acceptance examples:** [reliable Character edits](./BEHAVIOR.md#reliable-character-edits).

### F03: Choose how to generate ability scores

**User story:** As a player, I want to use standard array, point buy, application rolls or physical dice results, so that the character matches my table's generation method.

**Scope:** the supported generation methods, allocation and validation, persistent generated rolls, effective scores, maximums, modifiers and inspectable sources.

**Done when:** each method produces independently verified values; recalculation and reopening do not reroll scores; invalid input is explained; absent-input fallback remains distinguishable from a required missing answer.

**Depends on:** F01.

**Acceptance examples:** [ability-score generation](./BEHAVIOR.md#choose-an-ability-score-generation-method).

### F04: Choose an origin and navigate the build

**User story:** As a player, I want to explore species, backgrounds and available build decisions with clear descriptions and prerequisites, so that I can make informed choices.

**Scope:** expand origin content beyond the reference build; the guided Concept, Origin, Class, Abilities, Equipment and Review flow; contextual full-description panel; search/filtering; source restrictions; Ruleset-provided options; completion status; retained dormant and invalid choices.

**Done when:** changing an origin revalidates dependent answers without silently applying invalid benefits or deleting intent. Optional details and unclaimed equipment do not block build completion; unresolved calculations remain separately visible. Mechanical non-Element choices are supported when required; flavor-only suggestion lists remain deferred.

**Depends on:** F01.

**Acceptance examples:** [guided Character building](./BEHAVIOR.md#guided-character-building).

### F05: Advance a Fighter and revisit earlier decisions

**User story:** As a Fighter player, I want to change levels and make the required progression decisions, so that I can build beyond level 1 without recreating my character.

**Scope:** Fighter levels 1-20, relevant SRD subclass progression, feats and ability improvements, fixed/rolled hit-point gains including physical rolls, and class-context decisions. Introduce the minimum shared advancement contracts through this concrete journey.

**Done when:** intermediate and level-20 builds have verified results; per-level hit-point results survive edits and reopening; reducing and restoring levels preserves and revalidates the appropriate decisions.

**Depends on:** F02, F03, F04.

**Acceptance examples:** [retaining temporarily unavailable Choices](./BEHAVIOR.md#preserve-a-temporarily-unavailable-choice) and [recalculating after removing the first class](./BEHAVIOR.md#recalculate-after-removing-the-first-class).

### F06: Manage ordinary possessions and equipment

**User story:** As a player, I want to add, equip and remove items and correct starting-equipment claims, so that my sheet reflects what my character actually possesses and uses.

**Scope:** ordinary SRD equipment, quantities, direct item acquisition, currency adjustment, armor/shield/hand allocations, attack profiles, compatible ammunition, and starting-equipment correction previews and confirmation.

**Done when:** ownership and equipment intent remain distinct; conflicts are explained without arbitrary winners; AC and attacks reflect eligible equipment; claims never duplicate possessions; unsafe corrections are blocked rather than recreating missing items or money.

**Depends on:** F02.

### F07: Build the first spellcaster

**User story:** As a Wizard player, I want to learn and prepare spells and see my casting statistics, so that I can build a usable low-level spellcaster.

**Scope:** Wizard levels 1-5; composable spell properties; spellbook acquisition, learning and preparation; cantrips; casting ability, attack bonus, save DC and slot/preparation capacities; searchable spell choices and relevant spell details.

**Done when:** representative builds at levels 1 and 5 save/reopen with verified casting values and valid spell decisions. Spellbook membership and preparation are distinct; displaying spell slots does not spend them.

**Depends on:** F05.

### F08: Build a prepared divine spellcaster

**User story:** As a Cleric player, I want to build and advance my character using its preparation and always-prepared rules, so that its spell choices and class benefits are correct.

**Scope:** Cleric levels 1-20, relevant SRD subclass features and source-specific preparation permissions. Reuse shared spell/advancement contracts rather than introducing a second engine.

**Done when:** representative intermediate and level-20 journeys verify preparation, always-prepared entries, casting statistics and persistent class effects without transferring permissions between spell sources.

**Depends on:** F07.

### F09: Support arcane progression and Pact Magic

**User story:** As a Wizard, Sorcerer or Warlock player, I want the builder to respect my class's distinct progression and spell-access rules, so that superficially similar spellcasters do not behave identically.

**Scope:** complete these classes through level 20; relevant SRD subclasses; supported spell learning/replacement; Sorcerer and Warlock choices; Pact Magic capacities; source-specific casting permissions and feature reminders.

**Done when:** each class has a valid verified level-20 journey and focused replacement/capacity cases. Undoing a supported replacement restores its original entry; unresolved replacement targets are not silently reassigned.

**Depends on:** F07.

### F10: Build the other martial classes

**User story:** As a Barbarian, Monk or Rogue player, I want my class choices, training and persistent capabilities represented correctly, so that I am not limited to Fighter.

**Scope:** these classes at levels 1-20, relevant SRD subclasses and choices, training/Expertise, applicable AC alternatives, attacks and resource-capacity reminders.

**Done when:** each class has a verified level-20 journey and relevant intermediate checks. Situational effects remain accurately qualified reminders where execution is deferred; a description is not substituted for a required persistent mechanic.

**Depends on:** F05, F06.

### F11: Complete the remaining class journeys

**User story:** As a Bard, Druid, Paladin or Ranger player, I want my combination of class features and spellcasting to work together, so that all SRD classes are genuinely supported.

**Scope:** these classes at levels 1-20, relevant SRD subclasses, spell access and preparation, class-specific choices and persistent benefits. Companion-related features retain accurate reminders without adding standalone companion management.

**Done when:** each class has a verified level-20 journey and focused checks for its differing progression and choices. Refinement must identify any feature that needs its own narrower slice before claiming class completeness.

**Depends on:** F06, F08, F09.

### F12: Create higher-level and multiclass characters

**User story:** As a player joining an ongoing game, I want to create directly at my starting level and specify multiclass advancement order, so that I do not have to simulate a campaign's play history.

**Scope:** direct creation at supported levels, ordered class entries, prerequisites and optional-rule settings, first-class benefits, advancement-bound spell decisions and multiclass casting capacities.

**Done when:** representative higher-level single-class and multiclass builds save/reopen with verified results. Removing the first class promotes the next appropriately, retains affected decisions and does not recreate starting possessions. Character Level and Class Level are never substituted for each other implicitly.

**Depends on:** F05, F08, F09, F10, F11.

### F13: Understand movement, senses and defenses

**User story:** As a player, I want my character's movement, senses and defenses summarized with their conditions and sources, so that important capabilities are not buried in prose.

**Scope:** complete the relevant SRD movement modes, sense ranges, damage and condition defenses, conditional qualifications and source inspection. Earlier class/origin slices introduce any of these mechanics they already require; this slice completes the combined presentation and interactions.

**Done when:** overlapping and conditional sources have verified outcomes without invented combination policies or unconditional claims. An original content-defined sense using supported semantics works without adding a generic-core enumeration member.

**Depends on:** F04, F06.

### F14: Equip magic items and manage attunement

**User story:** As a player, I want to add relevant magic items and understand their equipment and attunement requirements, so that eligible benefits apply to the correct physical items.

**Scope:** relevant SRD magic items, attunement intent/cost/capacity, item-specific benefits, and supported ordered Item Decorator compositions with provenance.

**Done when:** over-capacity and ineligible intent are retained and explained; unrelated eligible benefits remain active; zero-cost bonds follow their contract; incompatible new compositions fail atomically; unresolved existing compositions do not silently activate bare-base mechanics.

**Depends on:** F06, F07, F13. Extend source-specific spell access where required by items rather than treating an item spell as an ordinary class spell.

### F15: Read contextual feature reminders

**User story:** As a player, I want concise sheet reminders to reflect my level, choices and item context, so that I can use the sheet without repeatedly rereading full rules.

**Scope:** complete Markdown presentation, related-Element links and sidebars; multiple entries per occurrence; numeric/text replacements in names, descriptions and action/usage/recovery labels; ordered variants; entry/text hiding; local presentation diagnostics.

**Done when:** the agreed presentation scenarios pass in the on-screen sheet, including multiclass and repeated-origin cases. Hiding never changes mechanics, substitutions do not execute markup, and full descriptions remain static. Earlier slices provide the descriptions/reminders necessary for their own journeys.

**Depends on:** F04, F12, F14.

### F16: Apply identifiable numeric adjustments

**User story:** As a player, I want to record a numeric bonus or override while still seeing the rules-derived result, so that agreed table adjustments remain transparent.

**Scope:** supported stable Character and item/profile targets; user bonuses and final-value overrides; validation; source inspection; dormant adjustments when a target disappears.

**Done when:** overrides and bonuses follow the agreed precedence; invalid new adjustments are rejected; target replacement does not inherit another target's adjustment; an adjustment does not make an ineligible item or attack valid.

**Depends on:** F06, F07.

### F17: Maintain a personal collection and portraits

**User story:** As a player, I want to recognize, reopen and organize my play-ready characters through their details and portraits, so that my collection is useful between sessions.

**Scope:** extend the baseline collection with permanent character deletion and confirmation, duplicate-name support, optional concept/backstory/appearance/notes, expected resume destinations, pack/default portraits and private reusable uploads. Folders and custom collection groups remain deferred.

**Done when:** another account, including the default admin, cannot access private characters or uploads. Sign-out permits safe account switching; deleting a character retains its uploaded portrait; deleting an in-use portrait is blocked with an explanation.

**Depends on:** F02, F04.

### F18: Export a usable PDF sheet

**User story:** As a player, I want a readable downloadable character sheet, so that I can bring my character to a table without relying on the live builder.

**Scope:** one standard 5e5 layout, portrait, calculated values, concise abilities, equipment and spell information; continuation pages; incomplete-state indicators; consistent presentation bindings and current-result export.

**Done when:** representative long martial, spellcasting and multiclass sheets omit no relevant entries, do not export pending/stale results as current, and preserve the meaning of browser reminders. Optional long-form backstory is not included in the standard PDF.

**Depends on:** F12, F14, F15, F16, F17.

### F19: Keep characters consistent with content fixes

**User story:** As a player, I want content corrections to update my character coherently and explain resulting issues, so that corrected prose and calculations do not disagree.

**Scope:** local provider retention and attribution; supported version/update behavior; stable identity migration; explicit reference/conflict diagnostics; retained decisions and notification after meaningful fixes. Exercise compatible dependency inclusion and replacement/append behavior through controlled internal packs.

**Done when:** a representative local update recalculates both mechanics and presentation, preserves player intent, and reports incompatibilities rather than serving a success-shaped partial result. A temporarily unavailable provider does not remove obtained content.

**Depends on:** F02, F04, F15.

**Decision gate:** resolve fix classification, migration and compatibility details and the replacement/append contracts before implementing those parts. Eligible fixes automatically migrate a Character's pinned version as described in the specification. This slice does not introduce player imports, marketplace workflows or a multiple-pack selection UI into the first release; eventual capabilities remain scoped by [SPECIFICATION.md](./SPECIFICATION.md#content-distribution).

### F20: Operate the builder safely on a private network

**User story:** As the operator, I want to start, protect, back up and restore the application, so that players can depend on their characters and portraits remaining available.

**Scope:** documented operator setup, private-network HTTPS and certificates, configured account secrets, database/storage isolation, safe logs, persistent startup, and database/portrait backup and restore.

**Done when:** the documented startup and recovery procedures work on persistent data, including restoring a saved character and its portrait; unauthorized access is rejected and private text/credentials are absent from ordinary diagnostics.

**Depends on:** F02, F17.

**Exposure rule:** earlier slices remain localhost-only unless the applicable private-network protections are already in place. Security controls are required before exposure, not deferred until this feature is marked complete.

## Release gates

Before calling the epic complete:

1. **SRD coverage:** the traceable inventory accounts for every relevant SRD 5.2.1 character-building option and mechanic, including cross-feature interactions. Populate it throughout delivery, not as an unplanned end-of-project content dump.
2. **All classes:** at least one valid browser-driven level-20 build per SRD class has independently checked key results. Focused rules tests cover representative intermediate levels, multiclassing, alternative choices, source-specific spells, equipment and invalid/unresolved cases.
3. **Real application:** critical journeys use actual content loading, application workflows and isolated persistent test data. Selected journeys use Reqnroll with C# Playwright; mocks or snapshots alone do not establish correctness.
4. **Composable design:** focused domain tests enforce aggregate invariants without persistence or DI. The synthetic Ruleset and original internal homebrew pack exercise independent module registration and reuse of supported mechanics without accidental D&D assumptions.
5. **Dependable state:** save/reopen, restart, conflicts, temporary disconnects, accepted-work recovery and idempotent equipment claims retain their agreed behavior across the combined product.
6. **Usable output:** on-screen and PDF sheets distinguish missing decisions, unresolved calculations and local presentation problems, retain source inspection, and do not silently omit long-character entries.
7. **Accessible browser experience:** keyboard navigation, labels, meaningful status announcements, light/dark themes and large-choice search work across the initial desktop experience. Chrome and Edge remain targets; initial Chromium automation does not claim installed-browser verification, which remains deferred.
8. **Operational readiness:** F20's documented restore has been exercised; accounts and assets remain isolated; HTTPS is configured for other-device access; normal post-initialization migrations preserve user data and startup exposes failures. The existing development database may be reset during F00.
9. **Scope honesty:** situational reminders are not advertised as executed actions, and unsupported options or mechanics are not presented as complete. Explicitly open contracts needed by shipped features have been resolved and documented.
10. **HTTP migration:** retained API routes use Minimal APIs, with FastEndpoints registrations and dependencies removed. Security, request validation, OpenAPI and endpoint integration checks remain effective.

## Later product epics

These are directions, not scheduled first-release slices or newly approved requirements:

- Content-authoring and private homebrew workflows, including eventual legacy Aurora XML conversion.
- Additional supported Rulesets, using independent modules; Daggerheart remains an example rather than a committed next game.
- Per-Character Requirement Overrides, such as a DM-permitted exception to a prerequisite; authority and lifecycle are deferred.
- Play-time resources, rests, effects and other explicitly selected gameplay assistance.
- Campaigns, companions and broader TTRPG management.
- Real-time campaign/room broadcasts and DM-triggered notifications, using SignalR after campaign membership and broadcast authorization are designed. These are not part of the first release.
- Marketplace and standalone compendium experiences, only after their separate scope and design are agreed.

Other deferred items, including editable character transfer, duplication, offline editing and alternate sheet templates, retain their status in [SPECIFICATION.md](./SPECIFICATION.md). None is a hidden completion condition for this epic.
