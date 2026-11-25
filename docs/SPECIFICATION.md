# Character-building application specification

Status: agreed target direction for refactoring this repository in place. This specification describes intended behavior, not proof of completion. The checkout targets .NET 11 and uses React/Vite; the first Identity increment, same-origin API access and generated API contracts are implemented. Private blob storage and remaining F00 checks are still open. Neither the framework migration nor sign-in alone completes F00. Items explicitly marked as working directions, open or deferred remain unsettled or outside the initial scope as stated.
See [CONTENT-AND-COMPUTATION.md](./CONTENT-AND-COMPUTATION.md) for content structure, validation and evaluation contracts; [CHARACTER-RULES.md](./CHARACTER-RULES.md) for character, spell and equipment mechanics; [GLOSSARY.md](../GLOSSARY.md) for domain language; and [BEHAVIOR.md](./BEHAVIOR.md) for acceptance examples.

## Product direction

- Build a web-based TTRPG management application centered initially on character building. Play-time tracking and broader management capabilities are later concerns.
- Preserve Aurora's emphasis on correct rules, automatic calculations, minimal manual steps, content extensibility, completeness and a dependable experience.
- Support D&D 5e5 first using SRD 5.2.1 content. Application software supplies Ruleset support; content supplies game material for supported Rulesets.
- A character uses a selected Ruleset that is fixed after creation; content from another Ruleset is available only under explicit compatibility rules. A Ruleset may represent a different game, not just another edition of the same game.
- Rulesets can define their own character-building concepts and decisions rather than being forced into D&D-specific concepts.
- People who understand a supported game's rules should eventually be able to create and modify content without programming.
- Refactor the existing Starlights application in place. Preserve its modular-monolith, API, React/Vite, Aspire and SQL Server foundations where they fit; replace existing behavior incrementally where it conflicts with this specification rather than starting a parallel application.
- Use Aurora WPF's proven character-building experience, this repository's existing behavior and Aurora Next's agreements and experiments as design input. Adopt only applicable requirements; existing implementation and imported agreements are not proof that a requirement is already satisfied.
- Develop in small, working, tested end-to-end slices that expand toward the full first-release scope; early slices are checkpoints, not a reduction of that scope.
- Begin with the bounded F00 platform-foundation checkpoint in [ROADMAP.md](./ROADMAP.md): upgrade the existing application to .NET 11, establish the OpenAPI-generated TypeScript API client, Identity and owner isolation, and private local blob storage. F00 does not redesign the Character domain or build a general rules engine.
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

- The character builder remains one application, organized as a modular monolith using clean architecture and domain projects. Evolve the existing Starlights modules without treating their current persistence model as the domain contract.
- Keep content resolution and character building as distinct capabilities with an explicit boundary. A user's permission to access content is separate from the Content Set and Source filters chosen for a character.
- Each domain module owns its data and exposes it through explicit contracts; modules do not directly modify each other's tables. They may share a SQL Server instance.
- The dedicated Identity module (`Modules.Identity`) owns accounts, credentials, authentication, account-role assignments and shared access infrastructure. Resource modules own resource-specific authorization decisions; see [Identity and access](#identity-and-access) for the agreed F00 contract.
- Use a domain-driven Character aggregate and composable Element definitions, with validated typed composition and Ruleset-specific behavior rather than a universal D&D-shaped model. Apply [the domain composition and Ruleset-module requirements](#domain-composition-and-ruleset-modules); introduce only the smallest concrete contracts needed by each feature.
- Target .NET 11 for the refactored application. The SDK, application/test target framework, Microsoft Extensions, ASP.NET Core and EF Core dependencies have been migrated as the first part of F00; Roslyn analyzer and attribute projects retain their `netstandard2.0` target.
- Use a React/Vite frontend with the repository's Shadcn-based UI and a C# ASP.NET Core API. Generate the TypeScript API client and contract types from OpenAPI; the React UI itself remains application code, not generated from the API schema.
- Use ASP.NET Core Minimal APIs for the target HTTP API and retire FastEndpoints. The current implementation still uses FastEndpoints; migrate it incrementally without leaving duplicate routes or losing required behavior, authentication, validation or OpenAPI coverage.
- Use relative `/api` requests from the browser. Vite proxies `/api` to the Aspire API during local development; production should expose the UI and API under one browser origin. The production proxy product (including Azure Front Door or YARP) is not selected yet.
- Use an Aspire AppHost and Service Defaults for orchestration and observability.
- Aspire startup starts local SQL Server and the blob emulator, applies required migrations and makes the bundled SRD pack available. The existing development database may be reset during this refactor; after initial setup, normal migrations must preserve user data and surface startup failures.
- Exclude passwords, backstories and other private character content from logs and traces by default while retaining useful diagnostic context.
- Use Entity Framework Core and SQL Server for durable application data, keeping domain behavior independent of persistence.
- Use Azure Blob Storage for uploaded portraits, with a local emulator orchestrated by Aspire for development. Keep blobs private and mediate browser reads and writes through the API, which enforces ownership.
- Uploaded portraits are reusable user-owned assets. Deleting a Character does not delete its portrait; deleting a portrait is blocked while one of that user's Characters references it.
- Document operator-managed backup and restore for the database and uploaded portraits to recover from server-wide data loss. Before release, verify the documented procedure by restoring a saved character and its uploaded portrait. An in-app backup or recovery interface is not required initially.
- Use ASP.NET Core Identity with cookie authentication and antiforgery validation for state-changing requests. F00 uses operator-provisioned username/password accounts without public registration, account-management HTTP endpoints or an administration UI; credentials stay outside source control.
- Enforce owner-only access to Characters and portraits, including on list and detail endpoints. Administrators do not receive an application-level override to view or edit private Character data by default.
- Require HTTPS for access from other devices on the private network; plain HTTP is permitted for localhost development. Document certificate setup for the operator.
- Restrict direct SQL Server and portrait-storage access to the application and authorized operators, rather than exposing it to all devices on the private network.
- Keep the existing developer tunnel an explicit development-only opt-in; it does not bypass application authentication or owner checks.
- Allow for future separate applications or services, such as content creation, management and marketplace services, without requiring them in the first release.
- The marketplace itself, including its workflows, interface and service design, is out of scope for this specification.
- Retain authoritative character choices, possessions, other character-owned facts, content/version information and update policy; derive sheet values from those facts and the applicable rules. Any saved sheet cache must be rebuildable and not a competing source of truth.
- Handle build edits and their resulting calculations through a direct asynchronous application workflow, not a required background event chain. Once the server has accepted an edit, closing the browser must not interrupt its processing; pending status, result or failure must be visible when the player returns.
- Detect work interrupted by an application restart and either recover it or visibly report the failure; accepted edits must not silently disappear.

### Identity and access

- Name the module Identity, following the `Modules.Identity` naming convention. Use standard ASP.NET Core Identity role support and ASP.NET Core named policies and authorization handlers, not a custom configurable permission engine.
- Identity owns accounts, credentials, authentication, account-role assignments and shared authorization integration. Other modules register their own resource-specific policies and handlers and supply their own resource facts. Identity does not query their tables or depend on their domain models, Rulesets or game rules.
- Keep account entities and ASP.NET Core Identity types inside Identity. Other modules obtain the authenticated user's stable GUID through an appropriate trusted contract, never a caller-supplied owner identifier.
- Characters and private assets use Owner / `OwnerId`, not `PlayerId`; the owner identifier is the user's GUID. There is no separate Player entity in F00. A DM may own a Character played by another person; ownership does not identify who plays it. Character transfer remains future work.
- A user keeps the same GUID across username or authentication-method changes. Future external sign-in and multiple linked provider accounts belong to the same local account; they do not replace its identity or its resource ownership. GitHub sign-in, Patreon linking, account-linking workflows and removing local passwords are outside F00.
- Resource modules enforce ownership and resource-specific permissions using shared authentication and authorization integration. A broad capability such as deleting Characters does not itself bypass ownership. F00 introduces no administrator ownership bypass.
- Application security roles are distinct from roles described by game content. Future module-defined permissions or resource policies may explicitly connect game facts to appropriately scoped access; content or Ruleset roles never automatically become account roles. F00 does not implement Ruleset-created security roles, a role-management UI or an editable permission matrix.
- F00 accounts require a unique username and password, without a required email address, real name or profile. Provide a local operator command to create accounts, reset passwords and disable/re-enable accounts. Password entry must not expose credentials in command arguments or ordinary logs. Password recovery is operator-mediated, without an email recovery flow.
- Operator access is an operational privilege, not an automatic application Admin role or permission to inspect private resources. Do not expose public registration or account-administration HTTP endpoints or UI in F00.
- Browser authentication uses a session cookie with an eight-hour inactivity timeout and no remember-me option. Do not promise that closing the browser signs the user out: browsers may restore sessions.
- Ordinary application restarts preserve otherwise-valid sessions, requiring durable authentication keys and revocation state. Sign-out ends the current session. Password reset or account disabling invalidates all of that account's sessions by their next API request, not after a periodic grace period; re-enabling does not restore revoked sessions. Signing out does not roll back an already accepted operation or erase previously downloaded data.
- Validate antiforgery tokens on state-changing browser requests. APIs return 401 for unauthenticated access, 404 for absent or another owner's private resources, and 403 for denied capabilities where the requester may know the resource exists. Lists exclude other owners' private resources. Return HTTP errors rather than HTML login redirects; React handles sign-in navigation.

### Private file management

- Name the module `Modules.FileManagement`. It owns private uploaded files, their stable identifiers, owner identifiers, metadata, stored bytes and lifecycle. Azure Blob Storage is its storage implementation, with Azurite for local development; it is not the module's business interface.
- Files are private to their authenticated owner. Other modules reference file identifiers and own their use as portraits, campaign media, session-note attachments or homebrew content. FileManagement does not interpret campaign notes, game rules or homebrew semantics.
- File contents are immutable. Replacing a file creates a new identifier; a consuming feature explicitly changes its reference. Preserve original bytes after validation, including embedded metadata. Derived or sanitized images are separate future files, not silent replacements.
- Record the actual stored byte size and owner for each file. Storage quotas are a future capability; F00 does not introduce quota configuration or enforcement.
- The first increment is a private image library: upload, paginated list, view and delete through authenticated Minimal APIs and the generated client. Initially accept PNG, JPEG and WebP only, up to 5 MiB and 4096 pixels per side. Validate actual decoded content and reject animated images, SVG, malformed images and oversized uploads.
- File-operation failures must be visible. Interrupted deletions remain visible and retryable, and files pending deletion cannot serve content.
- Character assignment, preventing deletion of referenced files, campaign integration, broader file types, sharing and public links are deferred. Do not expose assignment of library file identifiers through the retained arbitrary portrait-URL field before reference validation and deletion protection are implemented together.

### Domain composition and Ruleset modules

- Use one shared Character aggregate root for common Character state, such as identity, ownership and the selected Ruleset, plus exactly one composition owned by that Ruleset. Do not put every Ruleset's fields on the shared root. The composition may contain a collection of reusable typed parts rather than a fixed property for every possible part.
- The Character owns character-specific state, not the shared catalog of class, item and other Element definitions. It references versioned content and retains its own Choices, possessions and other facts. Loading a Character for an edit loads its aggregate as the consistency unit, not the entire Ruleset catalog.
- The aggregate root is the entry point for Character mutations. Typed parts own their local behavior and invariants; the root delegates local work and coordinates operations and invariants spanning parts. A composition collection does not allow callers to bypass the root or attach incompatible state.
- Expose Character parts to outside callers through read-only views or equivalent protection; a read-only collection alone is insufficient if its elements remain externally mutable. Parts need not be immutable internally, but external changes must go through the root.
- Start with named root operations for meaningful changes, such as recording a Choice or adding an Owned Item, delegating Ruleset-specific behavior as needed. Do not start with a general `Apply(ICharacterChange)` framework; revisit that seam if a demonstrated extension need justifies it. Keep Ruleset-specific rules out of the shared root.
- Favor passing focused Ruleset behavior explicitly to domain operations when needed rather than storing runtime service instances on the Character. The supplied behavior must correspond to the Character's selected Ruleset; mismatches are explicit errors. Concrete interfaces and arguments remain to be designed.
- Ruleset policies evaluate relevant read-only Character facts and content and return domain outcomes; the Character root applies accepted state changes, and the application handler persists them. Policies do not directly mutate Character parts or persist the aggregate. Policy override logic, when introduced, belongs outside the Character root; override selection and precedence remain open.
- A rejected domain operation leaves the in-memory aggregate unchanged as well as leaving saved state unchanged. Retaining an invalid or dormant Choice where the domain permits it is an accepted change, not a rejected operation. A later persistence or concurrency failure is separate; discard or reload the affected in-memory aggregate rather than assuming it reflects stored state.
- Ruleset validation defines which typed parts are required or compatible and, for each part contract, whether it can occur once or multiple times. Unsupported or invalid structures are explicit errors. A part can itself own a collection, such as an inventory containing distinct Owned Items.
- Reuse a typed part across Rulesets only where its domain meaning and invariants genuinely match. Similar-looking data alone is not sufficient. Focused shared interfaces may abstract common behavior, such as statistics calculation or progression, with implementations owned by registered Ruleset modules.
- Assign domain identity to a composed part only when the domain needs to address or track it independently; otherwise use value semantics where appropriate. Persistence row keys and relationships are infrastructure details and do not by themselves justify domain identity.
- Shared base classes are appropriate only for genuine shared domain behavior or invariants, never solely to satisfy EF Core or other persistence needs. Domain composition can use interfaces without requiring a persistence-shaped base type.
- Preserve the distinction between valid aggregate state and a completed build. A newly initialized Character can have unanswered decisions while satisfying its Ruleset's structural invariants. Domain operations may retain invalid or dormant player intent where specified; that does not authorize corrupt or incompatible component state.
- An application command loads the Character through its repository, invokes domain behavior through the root, and commits the aggregate through an application-managed Unit of Work. Changes spanning its parts commit atomically, and stale writes are rejected. The aggregate can be mapped across multiple tables without independent part-level commits.
- Persistence adapts to the domain model. EF Core mappings, database layout and serialization are infrastructure concerns, not reasons to expose unrestricted mutation, introduce ORM dependencies into domain projects or weaken invariants. Reconstitution must preserve authoritative state and identities and surface invalid persisted structure explicitly. Domain behavior must be testable without EF Core, a database or a DI container.
- The selected Ruleset determines the required Character composition. A D&D Character and a future Daggerheart Character can use different composed parts and domain behavior; common parts are shared only where their meaning and invariants genuinely match. Daggerheart is an architectural example, not an addition to first-release game support.
- Ruleset-specific modules own their Character initialization and Ruleset-specific composed parts, mechanics, parsers and validation. Trusted implementations satisfy focused shared interfaces where behavior varies; their concrete types and exact interface count remain open. Application orchestration resolves the appropriate initialization implementation from the selected Ruleset without requiring callers to assemble or mutate components themselves.
- On Character creation, the selected Ruleset initializer supplies its required composition from the selected content; a shared Character factory creates the root with the common state and that composition, enforcing applicable structural invariants. Authentication and owner selection remain application responsibilities. Ruleset modules do not duplicate common root-creation logic.
- Initialization establishes a structurally valid Character for the selected Ruleset and applies supported defaults. The selected content-defined Character Element remains the source of build offers, Grants and decisions; an initializer does not hardcode a duplicate content catalogue, grant arbitrary choices or persist derived sheet totals as authoritative state.
- Compose supported Ruleset modules through DI at application startup. Each module must work with the common platform and its explicitly declared shared dependencies without requiring an unrelated Ruleset module to be registered or referenced. Registering another Ruleset must not replace the first Ruleset's initialization or mechanics by registration order. Missing or ambiguous implementations for a selected Ruleset are explicit configuration errors, not a fallback to D&D.
- Use uppercase Ruleset identifiers in project and namespace names and as the canonical stable identifiers: `DND5E5` for the initial Ruleset and `TEST` for the synthetic test-only Ruleset, for example `Starlights.RuleSets.DND5E5` and `Starlights.RuleSets.TEST`. Match identifiers case-insensitively, store the registered uppercase identifier, and reject registrations that differ only by case as duplicates; input casing need not be uppercase. The test Ruleset is not a supported player-facing game. Migration details remain open.
- Element identifiers are distinct from Ruleset identifiers: a Ruleset-related namespace within an Element ID may be lowercase. `id:dnd5e:srd521:name` is illustrative, not an agreed identifier format or mapping for `DND5E5`; the precise format and mapping remain open.
- Start the DND5E5 Ruleset as one class library, organized by the needs of concrete slices; split projects only when a real dependency or responsibility warrants it. Its domain behavior remains independent of EF Core and HTTP endpoint infrastructure, and the SRD catalog remains declarative versioned content.
- Keep related Ruleset mechanics in focused pieces rather than concentrating all validation and calculations in one large manager. A small coordinator may connect interacting mechanics; organize and register behavior as needed, without requiring an interface or folder for every property.
- Keep shared platform code and interfaces Ruleset-neutral. The host can select registered implementations, but Ruleset-specific logic stays in its module rather than spreading switches through the platform. Aggregate behavior does not locate services through the DI container.
- Prefer the smallest design that enforces the actual invariants and supports these variations. Introduce focused composition and interfaces for demonstrated needs rather than an unrestricted component framework, speculative extension hierarchy or one abstraction per property. Independently registering compiled modules is sufficient; runtime plugin discovery, hot loading and separate deployments are not required.
- Verify module isolation using the initial Ruleset and the already required small synthetic Ruleset. Detailed aggregate constituents, initialization interfaces, persistence mappings and module registration contracts are to be specified through concrete slices without changing these architectural requirements.

### Feature workflows and CQRS

- Organize implementation around vertical feature slices. Separate commands that change state from queries that read it, without requiring separate databases, event sourcing or a mediator library merely to apply CQRS.
- Colocate each feature's Minimal API route registration, request/response contracts, validation and command or query handler in a feature folder/project where practical. Group related routes and register them at the application composition root rather than concentrating feature logic in startup. Domain code remains independent of HTTP and EF Core; do not create a project per feature merely to enforce slicing.
- Evolve the existing `Modules.Characters.Endpoints` project for Character features initially, colocating new handlers with their endpoints. Replace existing FastEndpoints features incrementally with Minimal APIs; extract workflows to a separate project if a concrete non-HTTP reuse or dependency need warrants it. During migration, preserve OpenAPI coverage for all retained routes and avoid registering both implementations of the same route.
- Initially place endpoints and handlers for genuinely Ruleset-specific actions in application feature folders too, calling Ruleset behavior through appropriate contracts. A separate Ruleset-owned presentation project remains an option if a concrete need arises.
- Initially inject and call feature handlers directly; introduce a command dispatcher or mediator only if a concrete need justifies it. Direct dispatch does not remove the need for consistent authorization, validation and transaction handling.
- Share commands where the action's domain meaning is genuinely shared across Rulesets; introduce Ruleset-specific commands when their semantics cannot be expressed as a shared action. The selected Ruleset supplies its behavior without putting Ruleset-specific rules into the shared command.
- Model commands around meaningful player actions rather than one command per field or one generic command for all edits. For example, recording a Choice can be shared across decision kinds where their semantics match and the typed answer is validated against the offered decision; adding an Owned Item is a distinct action. Obtain the authenticated owner from the trusted request context, not a caller-supplied owner ID.
- A command workflow authorizes the caller, loads the aggregate, invokes its domain behavior using the selected Ruleset as needed, and commits through the Unit of Work. Calculation follows the durable asynchronous workflow described above. Queries may read authorized, purpose-built projections without reconstituting the aggregate or changing authoritative Character facts; calculations must expose their revision and pending or error state rather than present stale results as current.

## Character computation

The following applicable Aurora Next agreements are carried forward as design requirements, not claims that an existing implementation already satisfies them:

- A content-defined Character Element starts the build and offers the relevant decisions. A sole available Character Element can be selected automatically.
- Ruleset-provided character options have content-defined offers and defaults; explicit player settings override those defaults for their character.
- Distinguish an Element definition from each application through a particular origin. Independently granted occurrences retain their own contributions and decisions, subject to the applicable combination and repeatability rules.
- Persist character-owned facts and Choices, not derived benefits as a separate source of truth. For example, a Fighter Choice and pinned content can determine martial weapon proficiency; a separately recorded homebrew grant can also contribute. Recalculate derived results when their inputs change; unresolved dependencies make the affected result unavailable rather than guessed.
- Keep decision and occurrence identities stable across renames and reordering so existing answers do not move to unrelated decisions.
- Apply each character edit atomically: a rejected edit leaves saved character state unchanged, including when the edit affects multiple Choices.
- Distinguish possession and equipment intent from eligibility for equipment benefits. Preserve invalid intent, explain the issue and apply only benefits whose eligibility is established.
- Retain distinct identities for independently meaningful owned items, including their equipment and attunement state. Reusable item modifications retain their ordered applications and provenance.
- Distinguish a known unmet requirement from unresolved evidence or a calculation error. Report unavailable results and their causes rather than presenting incomplete values as authoritative; independent results may still be available.
- Support a future per-Character Requirement Override for a specific unmet game requirement, such as a DM permitting a feat despite its prerequisite. The allowed selection can contribute its benefits, and the Character Sheet must show that the requirement was overridden. This does not bypass structural validation, content availability or authorization; who may create an override and its lifecycle remain open.
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
- Update relevant calculated information as Choices are made without a separate recalculation action. Use authenticated SignalR notifications for pending, result and failure status and for cross-tab Character revision changes. Keep edits and authoritative reads on HTTP/OpenAPI; after reconnect, clients refetch the latest saved revision rather than relying on missed notifications.
- Use React/Vite and the existing Shadcn-based component system rather than Blazor. Keep domain behavior independent of the UI.
- Generate the TypeScript API client and request/response types from the API's OpenAPI document. Keep application-specific React UI and TanStack Query composition in the frontend.
- In local development, route browser API calls through Vite's `/api` proxy. Avoid wildcard credentialed CORS; any direct cross-origin development access must use an explicit origin allowlist.
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
- Support separate users with private character collections. Follow the [Identity and access contract](#identity-and-access) for operator-provisioned username/password accounts, session behavior and module-owned authorization.
- Provide sign-out so another provisioned user can sign in on the same browser without gaining application access to the previous user's private characters or uploaded portraits.
- Keep account credentials out of source code, command arguments and ordinary logs; use protected password entry for operator account commands.
- Do not allow other accounts, including accounts with application roles or operator access, to view or edit an owner's private characters through the application by default.
- Ruleset software can ship with the application initially; independently installable runtime plugins are not required.

## Content distribution

- Bundle one versioned SRD 5.2.1 JSON Content Pack with the application and load it locally at startup. Stable pack-scoped string IDs are the public identity of Elements; internal persistence IDs, if used, are separate. No remote Content Provider or marketplace is required initially.
- Support the bundled SRD pack only in the initial release. Keep multiple compatible packs and their dependency/conflict resolution as later capabilities; do not build a multi-pack framework before a second pack requires it.
- Author the initial pack from the [official SRD 5.2.1](../assets/SRD_CC_v5.2.1.pdf) and identify its source revision and applicable attribution; see the [source and license reference](../assets/README.md).
- Obtained packs remain available from the application server when their original source is temporarily offline. Storage technology and longer-term removal or licensing policies remain open.
- Keep a published content-pack version's identity shared across users while keeping access and character decisions separate.
- A published pack retains its identity across updates; versions distinguish its revisions.
- Use semantic versioning (`major.minor.patch`) for content releases, with breaking changes marked by a new major version. Preview releases may change without advance warning, but each published preview revision has a distinct version identifier.
- Content identities normally survive renames and reorganization. Authors can explicitly map an old identity to a corrected one so existing character references can migrate.
- Characters refer to shared content definitions instead of maintaining independent copies of every selected feature; user choices and adjustments remain separate. A Character pins the version of its selected Content Pack so its references and calculations resolve against that version, and moves to another version through a migration rather than silently resolving against an arbitrary latest version. Missing pinned content must be surfaced, not silently replaced.
- Eventually allow a character to use multiple compatible Content Packs. A pack may represent a whole book or a small homebrew addition.
- Each Content Pack belongs to one Ruleset; distribution of artwork shared across Rulesets remains open.
- Let a character limit individual Sources within a pack; a pack may contain more than one Source. When multiple packs are supported, let the character select packs as well.
- Source filters limit optional choices but do not suppress required automatic grants from filtered Sources when the content is available and the player is authorized to use it.
- Default new 5e5 characters to the core SRD Source. Later, allow each user to configure defaults per Ruleset for new characters; changing a default does not change existing characters, whose selected Sources remain editable.
- Future per-user, per-Ruleset creation defaults also cover settings such as ability generation and optional rules; each character's settings remain independently editable.
- Content Packs carry source, author and license information, and the application makes relevant attribution available to users.
- Content Packs may include assets such as portraits, content images and header or hero images for future marketplace presentation.
- When multiple packs are supported, automatically include available compatible dependencies, show what was added, and explain missing or conflicting dependencies. This replaces Aurora Next's manual dependency-selection agreement.
- Version both packs and their declared dependencies, and allow authors to declare compatible dependency-version ranges. Content authors are not responsible for keeping third-party packs compatible; compatibility discovery is a future concern.
- Report conflicting definitions of the same content identity unless a replacement is explicitly declared by the content. Apply declared replacements without prompting and log them.
- Report unrelated competing replacements as ambiguous rather than choosing a winner silently.
- Allow packs to explicitly extend existing definitions without replacing them; combination and conflict rules remain to be specified.
- Apply eligible fixes automatically by migrating the Character's pin to the corrected pack version. The Character then resolves definitions and recalculates against that new version; it does not silently resolve an unchanged pin against the latest pack. Other updates require approval, subject to the preview and customization rules below. Fix classification, migration details and older revision retention remain open.
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
