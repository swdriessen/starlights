# Character-building application specification

Status: core specification agreed. Items explicitly marked as working directions, open or deferred remain unsettled or outside the initial scope as stated.
See [CONTENT-AND-COMPUTATION.md](./CONTENT-AND-COMPUTATION.md) for content structure, validation and evaluation contracts; [CHARACTER-RULES.md](./CHARACTER-RULES.md) for character, spell and equipment mechanics; [GLOSSARY.md](./GLOSSARY.md) for domain language; and [BEHAVIOR.md](./BEHAVIOR.md) for acceptance examples.

## Product direction

- Build a web-based TTRPG management application centered initially on character building. Play-time tracking and broader management capabilities are later concerns.
- Preserve Aurora's emphasis on correct rules, automatic calculations, minimal manual steps, content extensibility, completeness and a dependable experience.
- Support D&D 5e5 first using SRD 5.2 content. Application software supplies ruleset support; content supplies game material for supported rulesets. The form of ruleset modules or extensions remains open.
- A character uses a selected Ruleset that is fixed after creation; content from another Ruleset is available only under explicit compatibility rules. A Ruleset may represent a different game, not just another edition of the same game.
- Rulesets can define their own character-building concepts and decisions rather than being forced into D&D-specific concepts.
- People who understand a supported game's rules should eventually be able to create and modify content without programming.
- Use Aurora WPF's proven character-building experience, Starlights' domain and behavior work, and Aurora Next's agreements and experiments as source material. Accepted Aurora Next agreements are starting points, subject to applicability and review of conflicts and significant trade-offs.
- Implement the new application from scratch. Use Starlights as a structural reference and the existing projects as sources of behavior and lessons, rather than adopting their implementation wholesale.
- Develop in small, working, tested end-to-end slices that expand toward the full first-release scope; early slices are checkpoints, not a reduction of that scope.
- Begin with the bounded [F00 application-skeleton checkpoint](./F00-SPECIFICATION.md) to validate the runnable solution structure and real local dependencies. Require explicit developer approval of that structure before F01; F00 does not build the Character domain or a general rules engine.
- The first character-building checkpoint is the [level-1 SRD 5.2.1 Human Fighter/Soldier reference journey](./BEHAVIOR.md#first-checkpoint-human-fighter-soldier), with saving, reopening and independently checked sheet values, using real persistent storage and content loading rather than mock data.
- Agree important behaviors through concrete examples before implementation. Focus on C# Playwright end-to-end tests for real user journeys, supplemented by focused automated rules tests for complex calculations and edge cases.
- Use MSTest with the `MSTest.Sdk` project SDK for .NET unit tests.
- Use executable Reqnroll Given/When/Then scenarios backed by C# Playwright for selected critical journeys; do not require Gherkin for every test.
- Use Reqnroll's MSTest integration for those scenarios. Preserve the real-application, SQL Server and browser testing seams.
- Exercise the real application and persistence in critical end-to-end journeys with isolated test data, not a mock-backed browser.
- Run Playwright tests with Chromium initially. Separate verification in installed Chrome or Edge and automated cross-browser coverage are deferred; Chromium testing alone does not establish compatibility with every browser-specific behavior.
- Verify rules against independently established expected results traceable to the relevant SRD rules, including representative levels, multiclass interactions and edge cases.
- Before release, end-to-end tests build at least one valid level-20 character for each supported SRD class and verify key expected sheet results, supplemented by focused tests for intermediate levels, multiclassing and alternative choices.
- Populate a traceable inventory of relevant SRD character-building options and mechanics during implementation, and verify that each is supported before release. Use focused tests and shared mechanic coverage where appropriate; testing every possible character combination or automating deferred play-time effects is not required.

## Application architecture

- The character builder is one application, organized as a modular monolith using clean architecture and domain projects. Use Starlights as the primary structural reference without automatically importing its implementation decisions.
- Keep content resolution and character building as distinct capabilities with an explicit boundary. A user's permission to access content is separate from the Content Set and Source filters chosen for a character.
- Each domain module owns its data and exposes it through explicit contracts; modules do not directly modify each other's tables. They may share a SQL Server instance.
- Use domain-driven composable Element and Character aggregates, with typed Element properties and Ruleset-specific Character state rather than a universal D&D-shaped model. Apply [the domain composition and Ruleset-module requirements](#domain-composition-and-ruleset-modules); use Starlights as design input, not as a persistence-driven implementation to copy.
- Target .NET 11 initially, replacing the earlier .NET 10 choice.
- Use an Aspire AppHost and Service Defaults for orchestration and observability.
- Aspire startup starts local SQL Server and the blob emulator, applies required migrations and makes the initial SRD pack available. It preserves existing user data and surfaces startup failures.
- Exclude passwords, backstories and other private character content from logs and traces by default while retaining useful diagnostic context.
- Use Entity Framework Core and SQL Server for durable application data, keeping domain behavior independent of persistence.
- Use Azure Blob Storage for uploaded portraits, with a local emulator orchestrated by Aspire for development.
- Document operator-managed backup and restore for the database and uploaded portraits to recover from server-wide data loss. Before release, verify the documented procedure by restoring a saved character and its uploaded portrait. An in-app backup or recovery interface is not required initially.
- Use ASP.NET Core Identity for local account sign-in and authorization.
- Require HTTPS for access from other devices on the private network; plain HTTP is permitted for localhost development. Document certificate setup for the operator.
- Restrict direct SQL Server and portrait-storage access to the application and authorized operators, rather than exposing it to all devices on the private network.
- Allow for future separate applications or services, such as content creation, management and marketplace services, without requiring them in the first release.
- The marketplace itself, including its workflows, interface and service design, is out of scope for this specification.
- Working direction to validate in the first slice: retain authoritative character choices, possessions, content/version information and update policy; derive sheet values from those facts and the applicable rules. Any saved sheet cache must be rebuildable and not a competing source of truth.
- Handle build edits and their resulting calculations through a direct asynchronous application workflow, not a required background event chain. Once the server has accepted an edit, closing the browser must not interrupt its processing; pending status, result or failure must be visible when the player returns.
- Detect work interrupted by an application restart and either recover it or visibly report the failure; accepted edits must not silently disappear.

### Domain composition and Ruleset modules

- Design composition around domain meaning, maintainable clean code and correct aggregate-root state. The aggregate root owns its composed parts and controls mutations that affect its invariants, including changes spanning multiple parts. A generic component collection is not permission for callers to bypass domain behavior or attach incompatible state.
- Preserve the distinction between valid aggregate state and a completed build. A newly initialized Character can have unanswered decisions while satisfying its Ruleset's structural invariants. Domain operations may retain invalid or dormant player intent where specified; that does not authorize corrupt or incompatible component state.
- Persistence adapts to the domain model. EF Core mappings, database layout and serialization are infrastructure concerns, not reasons to expose unrestricted mutation, introduce ORM dependencies into domain projects or weaken invariants. Reconstitution must preserve authoritative state and identities and surface invalid persisted structure explicitly. Domain behavior must be testable without EF Core, a database or a DI container.
- The selected Ruleset determines the required Character composition. A D&D Character and a future Daggerheart Character can use different composed parts and domain behavior; common parts are shared only where their meaning and invariants genuinely match. Daggerheart is an architectural example, not an addition to first-release game support.
- Ruleset-specific modules own their Character initialization and Ruleset-specific composed parts, mechanics, parsers and validation. Trusted implementations satisfy focused shared interfaces where behavior varies; their concrete types and exact interface count remain open. Application orchestration resolves the appropriate initialization implementation from the selected Ruleset without requiring callers to assemble or mutate components themselves.
- Initialization establishes a structurally valid Character for the selected Ruleset and applies supported defaults. The selected content-defined Character Element remains the source of build offers, Grants and decisions; an initializer does not hardcode a duplicate content catalogue, grant arbitrary choices or persist derived sheet totals as authoritative state.
- Compose supported Ruleset modules through DI at application startup. Each module must work with the common platform and its explicitly declared shared dependencies without requiring an unrelated Ruleset module to be registered or referenced. Registering another Ruleset must not replace the first Ruleset's initialization or mechanics by registration order. Missing or ambiguous implementations for a selected Ruleset are explicit configuration errors, not a fallback to D&D.
- Keep shared platform code and interfaces Ruleset-neutral. The host can select registered implementations, but Ruleset-specific logic stays in its module rather than spreading switches through the platform. Aggregate behavior does not locate services through the DI container.
- Prefer the smallest design that enforces the actual invariants and supports these variations. Introduce focused composition and interfaces for demonstrated needs rather than an unrestricted component framework, speculative extension hierarchy or one abstraction per property. Independently registering compiled modules is sufficient; runtime plugin discovery, hot loading and separate deployments are not required.
- Verify module isolation using the initial Ruleset and the already required small synthetic Ruleset. Detailed aggregate constituents, initialization interfaces, persistence mappings and module registration contracts are to be specified through concrete slices without changing these architectural requirements.

## Character computation

The following applicable Aurora Next agreements are carried forward as design requirements, not claims that an existing implementation already satisfies them:

- A content-defined Character Element starts the build and offers the relevant decisions. A sole available Character Element can be selected automatically.
- Ruleset-provided character options have content-defined offers and defaults; explicit player settings override those defaults for their character.
- Distinguish an Element definition from each application through a particular origin. Independently granted occurrences retain their own contributions and decisions, subject to the applicable combination and repeatability rules.
- Keep decision and occurrence identities stable across renames and reordering so existing answers do not move to unrelated decisions.
- Apply each character edit atomically: a rejected edit leaves saved character state unchanged, including when the edit affects multiple Choices.
- Distinguish possession and equipment intent from eligibility for equipment benefits. Preserve invalid intent, explain the issue and apply only benefits whose eligibility is established.
- Retain distinct identities for independently meaningful owned items, including their equipment and attunement state. Reusable item modifications retain their ordered applications and provenance.
- Distinguish a known unmet requirement from unresolved evidence or a calculation error. Report unavailable results and their causes rather than presenting incomplete values as authoritative; independent results may still be available.
- Unchosen options do not contribute hypothetical effects to the current Character.

## First release

- Provide a browser-based journey to create, save, reopen and edit characters, with an on-screen sheet and a downloadable PDF sheet.
- Give the character sheet a dedicated page in the first release; players can open it in a second browser tab. A compact character-overview popup during building is a later capability.
- Automatically update an open sheet when the same character changes in another tab, with pending or error status where appropriate.
- Defer importing and exporting the new application's editable character data.
- Include concise relevant abilities, equipment and spell information in the PDF as well as numeric statistics; a full-description appendix is not required. Detailed layout remains open.
- Add readable continuation pages as needed so the PDF does not silently omit relevant sheet entries for large characters.
- Show the character's selected portrait on both the on-screen sheet and the standard PDF, while keeping the printed sheet legible and usable.
- Leave the optional long-form backstory out of the standard PDF; a separate story export or optional appendix is deferred.
- Provide one standard 5e5 PDF layout initially; alternate and user-supplied templates are deferred.
- Allow incomplete characters to be viewed and exported, clearly indicating their incomplete status and not presenting unresolved values as final.
- If an edit is still processing, PDF export waits for the current result or explains why export cannot yet proceed rather than silently exporting a stale sheet.
- Provide a personal character collection with reopening and deletion. Clearly explain that deletion is permanent and require confirmation; a recycle bin or restore feature is deferred.
- Defer character duplication, favorites, folders and custom collection groups.
- Permit multiple characters with the same name; names are not character identities.
- Allow players to begin without a chosen name, using "The Nameless One" as the placeholder until they name the character.
- Save character changes automatically and visibly indicate pending or failed saves.
- Do not let out-of-order save or calculation responses silently replace a more recent player edit or mark unsaved input as saved.
- Detect conflicting edits to the same character rather than silently overwriting newer changes. Keep unsaved input, including long-form story text, visible until the player chooses how to resolve the conflict.
- Preserve unsaved text during a temporary connection interruption, indicate that it is unsaved, and retry or offer a retry after reconnection. The supported interruption duration is not yet specified; recovery after a full page refresh or browser crash is not required initially.
- Update relevant calculated information as choices are made without a separate recalculation action. Use Blazor Server's interactive SignalR connection for user updates and show calculation progress or errors rather than presenting stale results as current.
- Use a Blazor Server application, not a TypeScript frontend. Limited JavaScript interoperability is acceptable for browser capabilities where needed. Use the existing guided-building prototypes as design input, subject to review.
- Use Fluent UI Blazor v5 as the current component-library choice. For frontend development, use the official Fluent UI Blazor MCP server and v5 consumer skill to check version-specific components, APIs, setup and patterns rather than relying on v4 guidance or recalled APIs.
- Keep the MCP server version aligned with the application's Fluent UI Blazor package and the skill available to agents in the workspace. The current configuration and setup instructions are in [local development](../LOCAL-DEVELOPMENT.md#fluent-ui-blazor-assistance). If documentation or version checks disagree, verify the actual package and assembly versions and surface a genuine mismatch rather than assuming compatibility.
- Offer light and dark themes, initially following the system preference with a user override.
- Keep domain behavior independent of the UI; a public third-party API is not required initially.
- Prioritize desktop and laptop browsers; a fully optimized phone experience is deferred.
- Target current desktop Chrome and Edge initially; defer Firefox and Safari support.
- Use English initially; additional interface languages are deferred.
- Require a connection to the local or private-network server for normal use; full offline browser editing is deferred.
- Support keyboard navigation and accessible labels and feedback throughout character building.
- Convey important dynamic status changes, including save failures and calculation errors, to assistive technology as well as visually, without announcing every routine sheet update.
- Let players choose an allowed content-pack portrait or upload their own, with a default available. Uploaded portraits belong to the user and may be reused across their characters; deleting a character does not delete its uploaded portrait, even if no other character uses it. Validate uploads and enforce access controls.
- Let users delete their uploaded portraits only when no character uses them. Explain a blocked deletion so they can change the affected characters first.
- Support all SRD 5.2 classes through levels 1–20, multiclassing under the applicable rules and optional-rule settings, and all relevant SRD character-building content, including species, backgrounds, feats, spells and equipment.
- Do not present an option as fully supported when its required choices or persistent sheet effects are missing or incorrect. Clearly disclose unsupported builder mechanics.
- Allow characters to be created directly at any supported level, while collecting the required decisions and retaining advancement history where the rules require it.
- Defer experience-point tracking; players can set and change levels without it.
- Defer separate companion management; relevant character features remain described on the sheet.
- Let a player specify multiclass advancement order where it affects choices or capabilities.
- In a multiclass build, the first class supplies starting-class benefits. If it is removed, promote the next remaining class, recalculate those benefits and preserve affected Choices for revalidation.
- Support SRD ability-score generation methods, including standard array, point buy and rolling, as well as manual entry of physical rolls.
- Persist application-generated ability rolls across recalculation, edits and reopening; generate new rolls only when the player explicitly requests them.
- Support applicable fixed and rolled hit-point gains, including physical dice results. Retain the result for each level so later changes do not reroll it.
- Let spellcasters record learned and prepared spells according to their class rules, with the sheet reflecting those decisions and their sources. Casting and spending slots are later play-time capabilities.
- Preserve distinct casting rules and permissions when the same spell is available from multiple sources, even if the interface groups it by name.
- Let players record and equip items, with applicable effects reflected in character values and attack profiles.
- Let players add available items, including magic items, directly to inventory without requiring a purchase or loot event.
- Include relevant SRD 5.2 magic items and their applicable sheet effects, including attunement where required. Executing item charges during play is later scope.
- Record a starting-equipment Choice separately from a player-initiated claim that adds its equipment and currency to inventory once. Recalculation must not duplicate it.
- Allow independently available starting-equipment packages, such as class and background packages, to be claimed separately.
- Let players record and adjust character currency, including starting currency, without requiring purchase, sale or trading workflows.
- After receipt, changing the starting-equipment Choice leaves current inventory untouched unless the player confirms a proposed correction.
- Do not silently recreate missing equipment or currency when a starting-equipment correction cannot safely reconcile the original receipt.
- Calculate persistent character values and choices covered by supported content. Situational abilities may initially be presented as accurate descriptions or reminders rather than automated play actions.
- Show supported feature reminders such as action type, maximum uses and recovery conditions when provided by content. Tracking expenditure and performing recovery remain deferred.
- Sheet-entry names, descriptions, action, usage and recovery labels support inline numeric and text replacements, including explicitly bound choice labels and content names. Conditional variants can override these fields without changing mechanics.
- Represent Ruleset-specific movement, senses and defenses through content and supported typed mechanics, with sources and conditional qualifications preserved. New definitions that reuse supported semantics do not require hardcoded generic-core fields for each game's senses or damage types.
- Make the contributing sources of calculated sheet values available for inspection without cluttering the normal sheet.
- Allow identifiable manual bonuses or overrides to numeric values while preserving and exposing the rules-derived value.
- Allow players to move between available building decisions and revisit earlier steps; show incomplete decisions.
- A build is complete when its required decisions are valid; optional details and unclaimed starting equipment do not block completion. Show unclaimed equipment separately as a reminder.
- Show build-choice completion separately from sheet calculation status; unresolved required calculations mean the sheet is not ready even if all required choices are filled.
- Reopen an incomplete character at the last building stage the player used, while allowing free navigation.
- Open a completed character to its sheet or overview first, with easy access to building and editing.
- Defer general undo/redo history; normal choices remain editable and dormant choices are retained.
- Start the guided 5e5 flow with Concept, Origin, Class, Abilities, Equipment and Review stages, subject to refinement against real choices; other Rulesets may use different stages. A written concept can be skipped.
- Provide an optional, editable long-form backstory saved with the character.
- Allow optional character details such as alignment, appearance and personal notes.
- Defer purely descriptive embedded-list choices. Support non-Element decisions when needed by an included mechanic, with stable identities and correctly applied effects rather than a flavor-only approximation.
- At character creation, focus on Ruleset selection, ability-score generation, Ruleset-provided options such as multiclassing, and limiting Sources to avoid irrelevant choices.
- Automatically select the sole available Ruleset while showing it to the player.
- Allow Ruleset-provided character options to be changed after creation; retain dependent Choices that become dormant and revalidate them if the option is enabled again.
- Make the full description and relevant mechanics of included content available while players make choices.
- Keep a dedicated, scrollable side panel visible during building for the selected or focused option's full description or other relevant contextual information.
- Support Markdown Element descriptions and links to related Elements, with documented sidebar/callout extensions and application-owned styling so homebrew authors need not know HTML. Render content safely without executing scripts; use [the description contracts](./CONTENT-AND-COMPUTATION.md#descriptions-and-sheet-presentation) for full and concise text.
- Require concise sheet descriptions to support character-specific calculated values that update with the character; identify unavailable values clearly. Full descriptions initially remain static source text; character-aware full descriptions are a later enhancement.
- Support both `{{inline-replacements}}` and conditional variants in concise sheet descriptions, with explicit Character Level or applicable Class Level context. Allow sheet descriptions to be hidden unconditionally or conditionally, including at a later level, without disabling mechanics.
- Allow one Element to author multiple separate sheet entries with their own labels and descriptions. Each can hide its whole entry or only its description text, independently of sibling entries and always without changing mechanics.
- Allow content to provide a concise sheet label and hide a sheet entry independently of its mechanics. Hiding an entry does not disable its Rules or prevent inspection of calculated contributions.
- Provide search and filtering for large choice lists, such as spells and equipment.
- Defer a standalone searchable compendium outside the character-building workflow.
- Show accessible options that fail character prerequisites as unavailable, with an explanation; prevent selecting them in the first release.
- Defer editing underlying feature definitions; this does not prevent editing character choices or equipment.
- Preserve a Choice that becomes unavailable without applying its effects, explain its status, and revalidate it if the decision becomes available again. Preserve invalid answers without treating them as a valid basis for capabilities.
- Support separate users with private character collections. Initial local or private-network deployment may use predefined development-only accounts with local credentials; registration and a user-management interface are not required.
- Provide sign-out so another predefined user can sign in on the same browser without gaining access to the previous user's private characters or uploaded portraits.
- Keep development account credentials out of source code and configure them for the local environment.
- Do not allow other accounts, including the initial admin account, to view or edit a player's private characters through the application by default.
- Ruleset software can ship with the application initially; independently installable runtime plugins are not required.

## Content distribution

- Obtain the SRD 5.2 pack automatically from a local Content Provider through a boundary compatible with future marketplace-style distribution. An online repository and marketplace interface are not required initially.
- Author the initial pack from the [official SRD 5.2.1](../assets/SRD_CC_v5.2.1.pdf) and identify its source revision and applicable attribution; see the [source and license reference](../assets/README.md).
- Obtained packs remain available from the application server when their original source is temporarily offline. Storage technology and longer-term removal or licensing policies remain open.
- Keep a published content-pack version's identity shared across users while keeping access and character decisions separate.
- A published pack retains its identity across updates; versions distinguish its revisions.
- Use semantic versioning (`major.minor.patch`) for content releases, with breaking changes marked by a new major version. Preview releases may change without advance warning, but each published preview revision has a distinct version identifier.
- Content identities normally survive renames and reorganization. Authors can explicitly map an old identity to a corrected one so existing character references can migrate.
- Characters normally refer to shared content definitions instead of maintaining independent copies of every selected feature; user choices and adjustments remain separate.
- Eventually allow a character to use multiple compatible Content Packs. A pack may represent a whole book or a small homebrew addition.
- Each Content Pack belongs to one Ruleset; distribution of artwork shared across Rulesets remains open.
- Let a character limit individual Sources within a pack; a pack may contain more than one Source. When multiple packs are supported, let the character select packs as well.
- Source filters limit optional choices but do not suppress required automatic grants from filtered Sources when the content is available and the player is authorized to use it.
- Default new 5e5 characters to the core SRD Source. Later, allow each user to configure defaults per Ruleset for new characters; changing a default does not change existing characters, whose selected Sources remain editable.
- Future per-user, per-Ruleset creation defaults also cover settings such as ability generation and optional rules; each character's settings remain independently editable.
- Content Packs carry source, author and license information, and the application makes relevant attribution available to users.
- Content Packs may include assets such as portraits, content images and header or hero images for future marketplace presentation.
- Automatically include available, compatible pack dependencies, show what was added, and explain missing or conflicting dependencies. This replaces Aurora Next's manual dependency-selection agreement.
- Version both packs and their declared dependencies, and allow authors to declare compatible dependency-version ranges. Content authors are not responsible for keeping third-party packs compatible; compatibility discovery is a future concern.
- Report conflicting definitions of the same content identity unless a replacement is explicitly declared by the content. Apply declared replacements without prompting and log them.
- Report unrelated competing replacements as ambiguous rather than choosing a winner silently.
- Allow packs to explicitly extend existing definitions without replacing them; combination and conflict rules remain to be specified.
- Working direction: characters resolve shared content to the latest fixes for their selected release line, including corrected descriptions, without a per-character fix opt-out. Older revisions may not remain available indefinitely; revisit this policy if a concrete need arises. Other updates require explicit approval, except for opted-in preview content.
- Homebrew compatibility issues must not block an official content fix. Apply the fix and report any resulting issues; detailed resolution of these rare conflicts is deferred.
- Infer fix status where needed from the change rather than requiring an author-supplied fix label; detailed update classification remains open.
- After a player opts into clearly identified preview content, its updates may apply automatically without per-update approval even when behavior changes. Preserve Choices and report resulting problems; do not bypass access checks or silently overwrite customizations.
- Notify the player when an automatic fix meaningfully changes a character, and preserve affected Choices rather than silently deleting them.
- Do not present an updated description and outdated calculated effects as consistent. Recalculate against the updated content and show pending or error status until the result is ready.
- Open for later: the detailed handling of incompatible dependencies after fixes, shared-version resolution, and how long older revisions remain available. Official fixes take priority over homebrew compatibility; retain compatibility where possible and report problems.
- When content customization is introduced, apply fixes automatically only when they are established to be independent of customizations; overlapping or uncertain changes require approval. Do not silently overwrite customized content.
- Automated tests include an original internal homebrew pack that reuses supported mechanics without application-code changes. Player-supplied pack import and a content marketplace are later capabilities.
- Use a small synthetic Ruleset in tests to expose accidental D&D-specific assumptions without presenting it as a supported game.
- Content Packs are declarative data, not executable scripts. New software capabilities come from trusted application modules or extensions, not from content packs.
- Use structured JSON as the initial machine-readable content format; converting legacy Aurora XML into the new format is a later capability. Asset packaging remains open.
- Represent game content through a shared Element concept while allowing different kinds of Elements to carry their own relevant information.
- Authors can reuse supported mechanics in new Elements without a software change for each individual feature or item; genuinely new mechanics may require application development.
- Content authors describe supported effects and prerequisites without needing to specify internal calculation scheduling. The application determines a valid order or reports a clear problem.
- A conditional benefit cannot satisfy its own eligibility requirement; unmet requirements must be explainable.
- Explicitly report unsupported Rulesets or mechanical capabilities in a pack; do not silently present unsupported effects as functional. Whether partially usable content may be browsed remains open.
- Support converting legacy Aurora WPF content in the longer term. Each user's import remains separate and customizable; the system may suggest an equivalent marketplace pack but must not silently merge the import with it.
- Converted Aurora WPF 5e content remains identified as 5e; adapting it for 5e5 is a separate, explicit and verified operation.
- Imported or independently created homebrew content is private to its creator by default. Storage, identity scoping and future sharing or publishing remain open.
- A user's customization of marketplace content does not change the shared published pack or other users' characters. The mechanism for representing customizations remains open.
- Distinguish reusable private content changes from adjustments that apply only to one character.
- Report legacy content that cannot be converted faithfully rather than presenting it as fully functional. Whether partial imports are permitted remains open.
- Importing legacy Aurora characters is not a product requirement.
