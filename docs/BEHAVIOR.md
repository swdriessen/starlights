# Character-building behavior examples

These agreed examples complement the [specification](./SPECIFICATION.md). They are not yet executable tests.

The [F00 platform foundation](./ROADMAP.md#f00-establish-the-platform-foundation) is verified before the Character-building slices below. These examples do not require Character functionality to be added to F00.

## F00: Identity and access

These examples exercise the [Identity and access contract](./SPECIFICATION.md#identity-and-access). Use private assets for resource-isolation checks in F00; Character slices carry the same guarantees forward.

**Given** an operator has local account-management access  
**When** the operator provisions two accounts using protected password entry  
**Then** each account has a distinct, stable user GUID and can sign in with its username and password without an email address or profile  
**And** passwords are not exposed in command arguments or ordinary logs  
**And** no public registration or account-administration HTTP endpoints or UI are exposed

**Given** a user signs in through the real browser application  
**When** the browser calls an authenticated API through the generated client and same-origin `/api` path  
**Then** the request is authenticated using the account's cookie session  
**And** a state-changing request without a valid antiforgery token is rejected without changing authoritative state

**Given** an account and an otherwise-valid browser session exist  
**When** the application restarts  
**Then** the account retains its user GUID and credentials  
**And** the existing session remains valid without another sign-in

**Given** a browser session has had no activity for eight hours  
**When** it next requests an authenticated API  
**Then** it receives 401 and the UI requires sign-in again  
**And** no remember-me option extends that session

**Given** a user has sessions in two browsers  
**When** the user signs out in one browser  
**Then** that session can no longer make authenticated requests  
**And** the other session remains valid subject to its normal expiry  
**And** another account can sign in without gaining application access to the previous owner's private resources

**Given** a user has multiple active sessions  
**When** the operator resets that user's password  
**Then** every existing session receives 401 on its next authenticated API request without a revocation grace period  
**And** the old password no longer signs in  
**And** the new password can establish a new session without changing the user GUID or ownership

**Given** a user has active sessions  
**When** the operator disables that account  
**Then** every existing session receives 401 on its next authenticated API request  
**And** the disabled account cannot sign in  
**When** the operator re-enables it  
**Then** a new sign-in is possible but previously revoked sessions remain invalid

**Given** an API request has no valid authenticated session  
**When** it requests a protected resource  
**Then** the API returns 401 rather than an HTML login redirect

**Given** one account owns a private uploaded asset  
**When** another account lists assets or requests that asset for reading, modification or deletion  
**Then** the list excludes the first account's asset and identifier-based requests return 404, just as for an absent asset  
**And** the asset remains unchanged  
**And** application roles or operator access do not grant an ownership bypass

**Given** a module-defined policy requires a capability for a resource whose existence the requester may know  
**When** an authenticated user without that capability requests the protected operation  
**Then** the API returns 403 rather than an HTML redirect  
**And** the policy uses module-owned resource facts without requiring Identity to know that module's domain model

## F00: Private file library

**Given** two provisioned accounts  
**When** one uploads a supported image through the private library  
**Then** its file identifier is stable and its stored bytes match the original upload  
**And** only that account can list, inspect, read or delete it  
**And** another account receives 404 for identifier-based access without a storage URL being disclosed

**Given** an authenticated upload  
**When** the image is malformed, animated, not PNG/JPEG/WebP, larger than 5 MiB or wider or taller than 4096 pixels  
**Then** the upload fails visibly without creating a usable file

**Given** a valid static image exactly 5 MiB in size or 4096 pixels per side  
**When** its owner uploads it  
**Then** the boundary value is accepted and its original contents and metadata are preserved

**Given** an uploaded private image  
**When** the application and local storage emulator restart using their existing persistent volumes  
**Then** the file metadata and original contents remain available to its owner

**Given** deletion of an owned file is interrupted by a storage or database failure  
**Then** the failure is visible and the file remains listed as pending deletion without readable content  
**When** the owner retries deletion, including after an application restart  
**Then** deletion can finish without exposing the file or requiring its original bytes

**Given** a user replaces an image in a future consuming feature  
**Then** the replacement receives a new file identifier and existing references are not silently changed

## F01.1: Create, list and reopen a Character

These examples refine the [first F01 increment](./F01-01-CREATE-LIST-REOPEN.md),
not the complete reference build. They require F00's verified foundation.

**Given** a signed-in owner and the bundled, pinned SRD 5.2.1 content  
**When** they create a DND5E5 Character named `Mira` through the real UI  
**Then** success is shown only after the root and required Ruleset composition commit atomically  
**And** the Character has a stable identifier, the authenticated owner, canonical Ruleset `DND5E5` and exact content bindings  
**And** its detail view clearly identifies the build as incomplete, without invented Choices or calculated sheet values

**Given** the owner has created `Mira`  
**When** they return to their collection, reopen it and refresh its detail URL  
**Then** the same saved identifier, name, Ruleset and pinned content are shown  
**And** reopening does not rerun creation or regenerate composed state

**Given** an owner creates two Characters named `Mira`  
**Then** both appear in the collection with distinct identifiers and reopen independently

**Given** the player omits a name or supplies only whitespace  
**When** they create a Character  
**Then** its displayed name is `The Nameless One`  
**And** a supplied 100-character name is accepted, while 101 characters produces a visible error without creating a Character

**Given** a saved Character and isolated SQL Server storage  
**When** the application and database restart using the same persistent storage without a reset  
**Then** its owner can list and reopen the same Character after signing in if necessary  
**And** its required composition, identities and exact content bindings are unchanged  
**And** publishing a newer pack version does not silently change its bindings

**Given** creation fails after database writes begin but before commit  
**Then** the operation fails visibly and no partial Character, ownership or composed state remains  
**And** the UI retains the entered name rather than navigating to a success view

**Given** a create response is lost and the browser cannot determine whether commit succeeded  
**Then** the UI reports the uncertain outcome instead of reporting success or automatically submitting another create  
**And** the player can refresh the collection before deciding whether to try again

**Given** a second account uses the application, including after switching users on the same browser  
**Then** the first owner's Characters are absent from its collection and client cache  
**And** requesting a first-owner detail identifier returns 404, just like an absent identifier  
**And** unauthenticated API requests return 401 and creation without valid antiforgery is rejected without a write

**Given** invalid creation content or an unsupported Ruleset selection  
**When** creation is requested  
**Then** it fails explicitly before committing a Character, without falling back to DND5E5

**Given** a saved Character has invalid composed state, an unavailable Ruleset or unavailable pinned content  
**When** it is reconstituted  
**Then** the failure is explicit rather than repaired through guessed defaults or a newer pack  
**And** read failures are not represented as an empty collection

**Given** DND5E5 alone, TEST alone or both Rulesets registered in either order in an isolated test host  
**When** each registered Ruleset creates a Character which is saved and loaded through a fresh SQL-backed context  
**Then** it retains its canonical Ruleset and that Ruleset's required typed state without depending on the other Ruleset  
**And** TEST does not require D&D class, ability or progression parts  
**And** duplicate identifiers including case-only duplicates, missing implementations and ambiguous initialization fail explicitly

**Given** the ordinary application host  
**Then** TEST is neither registered nor offered in creation options  
**And** submitting `TEST` manually does not enable it

## Reliable Character edits

**Given** two tabs show the same saved Character revision  
**When** one tab saves a supported edit  
**Then** the other tab updates to the saved Character and its current sheet without a manual refresh

**Given** a tab did not receive another tab's saved-revision notification  
**When** it submits an edit based on its stale revision  
**Then** the edit is rejected without changing the saved Character  
**And** its input remains visible and is identified as unsaved  
**When** the player reloads the latest Character and reapplies that input  
**Then** the application validates it against the latest state before saving

**Given** a supported choice is being edited while the connection is interrupted  
**When** the browser reconnects  
**Then** the choice is retried using the revision on which it was originally based  
**And** an intervening saved edit produces a conflict instead of an overwrite

**Given** the server accepted an edit for processing  
**When** the browser closes or the application restarts before its calculation outcome is observed  
**Then** the accepted operation remains pending until recovered or explicitly failed  
**And** the saved Character is not reported as unsaved

## Full and concise descriptions

**Given** an Element has a full description and a separately authored concise sheet description  
**When** a player views its builder details, on-screen sheet and PDF  
**Then** the details expose the full description  
**And** the on-screen sheet and PDF use the same concise description

**Given** an Element has no concise sheet description  
**When** its sheet entry is shown  
**Then** the entry shows its sheet label, or Element name if no sheet label is provided  
**And** the full description is not substituted or automatically shortened

**Given** content hides an Element's sheet entry  
**When** the Character is calculated  
**Then** hiding the entry does not disable its Rules  
**And** its contributing mechanics and sources remain inspectable

## Conditional sheet descriptions and inline replacements

**Given** the same feature applies through two independent class origins  
**When** its sheet entries are produced  
**Then** each occurrence retains separate entries and distinguishable origins  
**And** each evaluates its own originating class context even if the entry names and text match

**Given** one feature declares two separate sheet entries with different labels and descriptions  
**When** its sheet presentation is produced  
**Then** both entries appear in the on-screen sheet and PDF without creating additional mechanical occurrences

**Given** one entry hides only its description and another entry hides its whole entry  
**When** the sheet is rendered  
**Then** the first entry's label remains visible without its description  
**And** the second entry's label and description are absent  
**And** sibling entries and the feature's mechanics are unaffected

**Given** a feature has different concise sheet descriptions below and from level 17 of its originating class  
**And** each description contains a named calculated value referenced inside double braces  
**When** that Class Level changes from 16 to 17  
**Then** both the on-screen sheet and PDF use the level-17 description and its current calculated value  
**And** the full description remains static source text

**Given** one sheet entry has a base label and description, a first variant from Class Level 5 and a later variant from Class Level 17  
**And** both variants replace its description while only the first replaces its label  
**When** the applicable Class Level is 17  
**Then** the later variant's description wins  
**And** the first variant's label remains because the later variant does not override it  
**And** no mechanics change as a consequence of variant ordering

**Given** an entry's variant or hiding condition cannot be resolved  
**When** its sheet presentation is produced  
**Then** both the on-screen sheet and PDF show a local presentation-unavailable status with an inspectable reason  
**And** the entry is not silently hidden or presented with a supposedly definitive default description  
**And** unrelated entries and resolved numeric mechanics remain available

**Given** a Character has 16 levels in a feature's originating class and 1 level in another class  
**When** a sheet-description condition checks level 17 of the originating class  
**Then** that condition is not satisfied merely because Character Level is 17  
**And** a separate condition explicitly checking Character Level 17 is satisfied

**Given** content hides a sheet description from Character Level 17  
**When** the Character reaches level 17  
**Then** the sheet description is hidden in both the on-screen sheet and PDF  
**And** the feature's independently eligible mechanics and calculation evidence remain available  
**When** the Character returns to level 16  
**Then** the description is shown again if otherwise eligible

## Markdown descriptions and sidebars

**Given** a sheet description references a named calculated value  
**When** a Character edit changes that evaluated value  
**Then** the sheet description reflects the current result  
**And** rendering does not apply additional mechanics

**Given** a sheet-description value cannot be resolved  
**When** the sheet description is rendered  
**Then** it identifies the value as unavailable with an inspectable reason rather than showing 0 or a stale result

**Given** a full description contains static source text  
**When** it is read in a Character's builder  
**Then** the initial release preserves that static text without requiring Character-specific substitutions

**Given** a full description contains Markdown emphasis, a list and a titled sidebar containing Markdown  
**When** the application renders the description  
**Then** its formatting and sidebar title are preserved using application-owned styles  
**And** the author does not need HTML or CSS

**Given** a supported description appears in both browser and PDF output  
**When** the application renders each output  
**Then** both preserve its textual meaning and supported semantic structure without requiring identical layout

## Sheet action, usage, recovery and text replacements

**Given** an entry's name, description, action, usage and recovery fields contain named inline replacements  
**And** their bindings resolve to numbers and typed text  
**When** browser and PDF sheet output is produced  
**Then** every field uses its current bound values  
**And** no placeholder in these fields requires a separate renderer or gameplay action  
**And** full descriptions remain static

**Given** an entry has base action, usage and recovery labels  
**And** successive satisfied variants override some of those fields  
**When** its sheet presentation is produced  
**Then** each field uses its last explicitly provided matching value  
**And** omitted fields retain their earlier values and an explicit empty string clears its label

**Given** a text replacement refers to a particular recorded mechanical choice  
**When** that valid choice changes to another permitted option  
**Then** its sheet label updates to the selected option's display text in the same occurrence context  
**And** an unanswered or unavailable decision produces an explainable unavailable value rather than a guessed answer

**Given** bound text contains Markdown delimiters, HTML-looking text or another double-brace placeholder  
**When** it is inserted into a sheet field  
**Then** it is displayed as text rather than interpreted as markup or evaluated recursively

## Composable properties and Ruleset-specific mechanics

**Given** only the initial Ruleset module and its required common dependencies are registered  
**When** a Character is created for that Ruleset  
**Then** its initialization establishes the required composed state without depending on another Ruleset module  
**And** its build offers and Grants still come from the selected content-defined Character Element  
**And** unanswered build decisions do not by themselves make the aggregate structurally invalid

**Given** the initial and synthetic Ruleset modules are registered together  
**When** a Character is initialized for each Ruleset  
**Then** each uses its own initialization and Ruleset-specific composed parts  
**And** reversing registration order does not change the selected initialization or mechanics  
**And** a missing or ambiguous initialization for a selected Ruleset is reported rather than silently using another Ruleset

**Given** a domain operation changes several composed parts of an Element or Character  
**When** that change would violate an aggregate invariant  
**Then** the operation is rejected without leaving a partially changed aggregate  
**And** this behavior can be exercised without EF Core, a database or a DI container

**Given** a saved Character has Ruleset-specific authoritative state and stable decision and occurrence identities  
**When** infrastructure saves and reconstitutes it  
**Then** the state and identities are preserved without bypassing the aggregate's structural invariants  
**And** invalid persisted structure is reported explicitly rather than repaired through guessed defaults

**Given** a supported spell property contract requires a spell level and range  
**When** content supplies invalid typed data or omits a required property  
**Then** content validation reports the affected property  
**And** the generic Element is not treated as a supported spell merely because its name or type says so

## Build a level-1 Wizard

**Given** a level-1 Wizard with effective Intelligence 16 and Proficiency Bonus +2\
**When** the Character Sheet is calculated\
**Then** the Wizard has three cantrips, six learned 1st-level spells, two 1st-level spell slots and capacity to prepare four spells\
**And** its spell attack bonus is +5 and spell save DC is 13\
**And** the six learned spells are not prepared unless separately chosen

The expected values use the SRD 5.2.1 Wizard class table and spellcasting rules: 16 Intelligence gives a +3 modifier; spell attack is +3 + 2, save DC is 8 + 3 + 2, and preparation capacity is 1 + 3.

## Advance and lower a Wizard

**Given** a level-5 Evoker with effective Intelligence 16 and Proficiency Bonus +3\
**When** the Character Sheet is calculated\
**Then** the Character has learned eighteen leveled spells, four cantrips, preparation capacity eight, and slot capacities of four 1st-level, three 2nd-level and two 3rd-level slots\
**And** its spell attack bonus is +6 and spell save DC is 14\
**And** none of the displayed capacities represents a spent or remaining resource

The spellbook contains fourteen spells from the ordinary progression (six starting spells plus two at each of levels 2–5), three Evocation Savant additions at level 3 (two chosen Evocation spells of up to 2nd level and one 2nd-level Evocation spell for the new slot level), and one 3rd-level Evocation spell when level 5 grants 3rd-level slots. Preparation capacity follows the F07 contract: Wizard Class Level plus effective Intelligence modifier, minimum one. Lowering the Wizard to level 2 retains the level-3-through-5 spell decisions as dormant, removes their active access and capacity effects, and restoring level 5 revalidates the same decisions.

## Keep Wizard learning and preparation separate

**Given** a Wizard has learned a leveled spell at a particular class advancement<br>
**When** the player prepares and later unprepares that spell<br>
**Then** the Spell Learning remains associated with the same advancement and identity<br>
**And** only the current Spell Preparation and its source-specific prepared access change

**Given** a Wizard has learned a ritual spell but has not prepared it<br>
**When** Wizard spell access is evaluated<br>
**Then** ritual access is shown separately where the SRD permits it<br>
**And** the spell is not reported as prepared

**Given** content declares a new sense using an already supported sense-and-range mechanic  
**When** a Character acquires it and a Character of a different Ruleset is also evaluated  
**Then** the first Character's sheet exposes the sense and its range with its origin  
**And** no generic-core enumeration edit is required  
**And** the other Ruleset does not acquire that sense merely because its definition is available

**Given** a defense applies only against a declared category of effect  
**When** the Character Sheet is projected without an incoming effect  
**Then** the sheet preserves the qualification instead of presenting unconditional immunity or simulating damage

## Mechanically meaningful non-Element choices

**Given** an included mechanic requires choosing a damage type from typed options  
**When** the player changes that valid choice  
**Then** its declared effects and sheet reminders follow the stable chosen option identity  
**And** renaming its display label does not change the recorded option's identity

**Given** a suggested personality-trait list has no mechanical effect  
**When** first-release scope is assessed  
**Then** that prompted list may remain deferred while optional free-form personal details remain available

## Guided Character building

**Given** a player moves freely between the Concept, Origin, Class, Abilities, Equipment and Review stages<br>
**When** they reopen an incomplete Character<br>
**Then** the last used stage is restored<br>
**And** optional concept details remain saved without affecting Build Completion

**Given** a player replaces an origin that supplied a dependent decision<br>
**When** the new origin does not offer that decision<br>
**Then** the prior answer remains recorded as dormant and contributes no occurrence or effect<br>
**When** the player restores the original origin<br>
**Then** the retained answer is revalidated and becomes active if it remains valid

**Given** a player replaces a starting-equipment package after claiming it<br>
**When** the Character is saved and reopened<br>
**Then** the original receipt and its possessions or currency remain unchanged<br>
**And** the application does not claim the replacement package or silently repair the receipt

**Given** a player searches or focuses an available candidate<br>
**When** the candidate list narrows or its details are displayed<br>
**Then** the Character's recorded Choices and calculated effects do not change

## Restart the local application

**Given** a player has a saved character in the local application  
**When** the application is restarted through Aspire  
**Then** the required local dependencies and SRD content become available  
**And** the saved character remains accessible

## First checkpoint: Human Fighter Soldier

This reference build exercises the first checkpoint; it does not limit the full release scope. Use the SRD 5.2.1 Ruleset and content, real content loading and durable application storage.

The player makes these choices:

- Level 1 Fighter; Human of Medium size; Soldier background.
- Standard array assigned as Strength 15, Dexterity 14, Constitution 13, Intelligence 8, Wisdom 10 and Charisma 12; Soldier increases Strength by 2 and Constitution by 1.
- Fighter skills Insight and Survival; Human Skillful proficiency in Perception; Soldier's gaming-set proficiency in dice.
- Human Versatile feat Alert; Fighter Fighting Style feat Defense; Weapon Mastery in greatsword, flail and javelin.
- Languages Common, Dwarvish and Elvish.
- Fighter starting-equipment package A and Soldier starting-equipment package A.

**Given** a signed-in player can access the SRD 5.2.1 content  
**When** they enter the choices above  
**Then** the package selections alone have not added their items or currency  
**When** they separately claim both packages, wear the chain mail and wield the greatsword  
**And** their changes finish saving  
**And** they reopen the character  
**Then** their choices, possessions, equipment and calculated results persist  
**And** the sheet shows:

- Ability scores: Strength 17, Dexterity 14, Constitution 14, Intelligence 8, Wisdom 10 and Charisma 12.
- Proficiency bonus +2; hit-point maximum 12; speed 30 feet; armor class 17, including Defense; initiative +4, including Alert.
- Strength saving throw +5 and Constitution saving throw +4.
- Athletics +5, Intimidation +3, Insight +2, Survival +2 and Perception +2; passive Perception 12.
- A greatsword attack of +5 to hit and 2d6 + 3 slashing damage, with Graze mastery.
- Savage Attacker, Alert, Defense, Second Wind and the selected weapon masteries; Second Wind has two uses.
- Both claimed starting packages and 18 GP in total, without duplicate equipment or currency after reopening.

Expected results are independently derived from the [local SRD 5.2.1](../assets/SRD_CC_v5.2.1.pdf), pages 13, 20–22, 47–48, 83, 86–88 and 91–92. They must not be generated from the application under test.

This work includes material from the System Reference Document 5.2.1 (“SRD 5.2.1”) by Wizards of the Coast LLC, available at https://www.dndbeyond.com/srd. The SRD 5.2.1 is licensed under the Creative Commons Attribution 4.0 International License, available at https://creativecommons.org/licenses/by/4.0/legalcode.

## Switch users on a shared browser

**Given** a player has private characters and uploaded portraits  
**When** they sign out and another user signs in on the same browser  
**Then** the other user cannot access those characters or portraits

## Preserve a temporarily unavailable choice

**Given** a level-3 Fighter has a recorded subclass Choice  
**When** the player reduces that Fighter to level 2  
**Then** the subclass Choice is retained as dormant and its benefits do not apply  
**When** the Fighter returns to level 3  
**Then** the Choice is revalidated and, if valid, applies again

## Choose an ability-score generation method

**Given** a new D&D 5e5 Character  
**When** the player opens its Abilities stage  
**Then** the standard array (15, 14, 13, 12, 10 and 8) is the selected method  
**And** those values are already assigned to Strength, Dexterity, Constitution, Intelligence, Wisdom and Charisma in that order  
**And** the player can revise the assignments, using each available value only once  
**And** other unanswered build decisions still keep the Character incomplete

**When** the player selects point buy  
**Then** all six Ability inputs start at 8, spending 0 of 27 points  
**And** each Ability can be assigned a score from 8 through 15  
**And** the costs for 8 through 15 are 0, 1, 2, 3, 4, 5, 7 and 9 points  
**And** the 27-point budget and remaining points update with saved inputs  
**And** a score or total outside those limits is rejected without changing saved inputs

**When** the player requests application rolls  
**Then** six 4d6-drop-lowest results are generated, shown and assigned in displayed Ability order  
**And** assigning them does not change the recorded results  
**When** the player edits another Choice or reopens the Character  
**Then** the same results remain saved  
**When** the player requests a reroll and confirms replacing the results  
**Then** the replacement results are saved and assigned in displayed Ability order, replacing the old assignments

**When** the player enters physical dice totals  
**Then** each entered total from 3 through 18 is recorded as player-provided  
**And** the interface does not claim that the physical rolling procedure was verified

**Given** a Character has Ability Score Inputs  
**When** an eligible increase and maximum are applied  
**Then** the saved inputs remain distinct from the Effective Ability Scores  
**And** modifiers and dependent values use the Effective Ability Scores  
**And** calculation sources identify the inputs and applicable increases and limits

**Given** assignments exist for the current method  
**When** the player chooses another method  
**Then** confirmation is required before replacing assignments  
**And** canceling leaves the saved method and inputs unchanged  
**And** standard array and application rolls initialize assignments in displayed Ability order, point buy initializes all six at 8, and physical dice starts unassigned  
**And** existing application dice are retained and assigned when switching away and back

**Given** a saved Character has customized, partial or absent Ability inputs  
**When** the Character is recalculated, reopened or its current method is selected without a reroll  
**Then** those saved inputs remain unchanged; initial allocation is not a read-time fallback or migration

**Given** point buy has six valid assignments costing at most 27 points  
**Then** the ability decisions are complete even with points remaining  
**And** other unanswered build decisions still keep the Character incomplete

**Given** application rolls contain two totals of 18 and no third 18  
**Then** two Abilities may receive 18  
**And** a third assignment of 18 is rejected without changing saved inputs

**Given** an application-roll or reroll response is lost after acceptance  
**When** the identical operation identifier, revision and request are retried  
**Then** the original saved dice are returned without another generation or revision increment

**Given** Strength input is 18, an eligible increase is +2 and the applicable maximum is 19  
**Then** the input stays 18, the ordinary score is 20 and the effective score is 19  
**And** the modifier is +4 and dependent checks use +4, not +5  
**And** raising the maximum to 20 reveals the retained ordinary score and modifier +5

## Delete a character

**Given** a player has a saved character  
**When** they request its deletion  
**Then** the application explains that deletion is permanent and asks for confirmation  
**When** they confirm  
**Then** the character is deleted without a recycle-bin restore option

## Keep and safely delete reusable portraits

**Given** a player uploaded a portrait used by two characters  
**When** they delete one of those characters  
**Then** the portrait remains available and the other character still uses it  
**When** they try to delete the portrait  
**Then** the application explains that a character still uses it and blocks deletion  
**When** no character uses the portrait  
**Then** the player can delete it

## Do not duplicate starting equipment

**Given** a player has selected a starting-equipment package  
**Then** its equipment and currency are not received merely by making that Choice  
**When** the player claims the package  
**Then** its equipment and currency are added once  
**When** its sheet is recalculated or the character is reopened  
**Then** the equipment is not added again  
**When** the original starting-equipment Choice is changed  
**Then** current inventory remains unchanged unless the player confirms a proposed correction

## Claim independent starting packages

**Given** a character has selected class and background starting-equipment packages  
**When** the player claims only the class package  
**Then** its equipment and currency are received once  
**And** the background package remains unclaimed and available to claim separately

## Correct a receipt after currency has been spent

**Given** a player claimed a starting package and later spent some of its currency  
**When** they change the starting-equipment Choice  
**Then** the current inventory and currency are not silently changed  
**And** a proposed correction explains any discrepancy it cannot safely reconcile  
**And** missing currency is not silently recreated

## Recalculate after removing the first class

**Given** a multiclass character started as a Fighter and subsequently gained a Rogue level  
**When** the player removes the Fighter class  
**Then** Rogue becomes the first remaining class and its starting-class benefits are recalculated  
**And** affected Choices are preserved for revalidation

## Keep spell access sources distinct

**Given** a character can access the same spell through two different sources with different casting permissions  
**When** the sheet presents that spell  
**Then** both sources and their respective permissions remain distinguishable  
**And** neither source's permissions are silently applied to the other

## Distinguish a complete build from an unresolved sheet

**Given** all required build decisions are valid and a starting-equipment package is unclaimed  
**And** a required sheet calculation is unresolved  
**When** the player views the character  
**Then** the build is marked complete and the unclaimed package is shown as a reminder  
**And** the sheet is not presented as ready and the unavailable result is identified

## Export a long character sheet

**Given** a character has a selected portrait, a long backstory and more relevant sheet entries than fit on one page  
**And** no edit is still processing  
**When** the player exports the standard PDF  
**Then** it includes the portrait and all relevant entries on readable pages  
**And** it does not include the long-form backstory

## Detect a conflicting edit

**Given** a player opens the same character in two browser tabs  
**When** one tab saves a change and the other attempts to save a stale conflicting change  
**Then** the application reports the conflict instead of silently overwriting the newer change  
**And** the stale tab retains the unsaved input for the player to resolve

## Update a sheet in another tab

**Given** a player has the builder and sheet for the same character open in separate tabs  
**When** a build edit is processed successfully  
**Then** the sheet tab updates to the current result without a manual refresh

## Preserve text during a temporary disconnection

**Given** a player is editing a character's backstory  
**When** the browser temporarily loses its connection before the latest text is saved  
**Then** the text remains available in the open tab and is identified as unsaved  
**And** after reconnection the application retries or offers a retry

## Finish a submitted edit after closing the browser

**Given** the server has accepted a character edit for processing  
**When** the player closes the browser before processing finishes  
**Then** the server continues processing the edit  
**And** its pending status, result or failure is visible when the player returns

## Detect an edit interrupted by a server restart

**Given** the server has accepted a character edit for processing  
**When** the application restarts before processing finishes  
**Then** the application recovers the edit or reports its failure to the player  
**And** it does not silently lose the accepted work

## Filter sources without breaking required grants

**Given** a selected Element grants a supporting Element from another Source  
**And** both Elements are available and the player is authorized to use them  
**When** the player filters out the supporting Element's Source  
**Then** it is hidden as an optional choice but its required automatic grant remains effective

## Report accidental content conflicts

**Given** two selected Content Packs define the same Element identity differently  
**And** neither declares an applicable replacement  
**When** the Content Set is resolved  
**Then** the application reports the conflict rather than silently selecting a definition

## Preserve partial and excessive selection answers

**Given** a Selection permits two answers and each eligible answer grants a benefit  
**When** one valid answer is recorded  
**Then** its benefit applies and the Selection reports one remaining answer  
**When** three answers are restored for that Selection  
**Then** all three are retained, an excess-answer issue is reported and none of their benefits apply  
**And** unrelated calculations remain available

## Keep option preview failures local

**Given** a valid Character has an available Selection  
**When** a candidate's eligibility cannot be resolved during preview  
**Then** that candidate reports its unresolved eligibility  
**And** its Rules are not applied and its preview failure does not make the current Character calculation incomplete

## Reject an invalid content snapshot

**Given** a Content Set has two independently detectable invalid Rule declarations  
**When** it is compiled  
**Then** both errors identify their applicable content locations  
**And** no valid compiled model is returned for evaluation

## Report recursive grants

**Given** Element A unconditionally grants B and B unconditionally grants A  
**When** the content is validated  
**Then** the recursive Grant chain is reported and rejected rather than silently deduplicated

## Resolve dependencies without author scheduling

**Given** a character has Strength 12 and an eligible item increases it by 2  
**And** a separate Rule contributes the Strength modifier to another statistic  
**When** the character is calculated  
**Then** Strength is 14 and the other statistic receives +2  
**And** the author has not supplied phase, priority or dependency-list instructions

## Retain independent feature occurrences

**Given** a character has a Strength score input of 12  
**And** two independently granted occurrences each increase Strength by 1  
**And** both increases are additive and no other limit changes the result  
**When** the character is calculated  
**Then** its effective Strength is 14  
**And** each contribution retains its own origin

## Apply a conditional rule only once

**Given** an item occurrence has one Rule that adds 1 to Strength when the item is carried or equipped  
**And** the character's Strength is 12 before that Rule  
**When** the item is both carried and equipped  
**Then** the Rule contributes once and the resulting Strength is 13

## Recombine independent qualification evidence

**Given** Strength is 12 before two contributions in the same highest-wins group  
**And** an independent +1 and a conditional +2 compete in that group  
**And** the +2 requires independently established Strength of at least 13  
**When** the character is calculated  
**Then** the independent +1 establishes qualification Strength 13  
**And** the +2 applies as the group winner and final Strength is 14  
**And** the independent +1 remains available in the explanation

## Choose between conditional alternatives

**Given** a feature grants Expertise if Proficiency is independently established and otherwise grants Proficiency  
**When** independent Proficiency is present  
**Then** only its Expertise branch applies  
**When** independent Proficiency is known to be absent  
**Then** only its Proficiency branch applies  
**When** independent Proficiency is unresolved  
**Then** neither branch applies and the unresolved decision is reported

## Combine signed grouped contributions

**Given** a numeric target has eligible contributions of -2 and -5 in one highest-wins group  
**When** the group is combined  
**Then** it contributes -2 and both origins remain explainable  
**And** an undeclared zero is not added

## Apply a score-setting effect

**Given** a 5e5 character has a Strength score input of 12  
**And** an applicable ordinary increase of 2, an eligible setting value of 19 and a maximum of 20  
**When** the character is calculated  
**Then** its effective Strength is 19 rather than 21

## Resolve an absent ability input explicitly

**Given** a required ability input has not been answered  
**And** its Ability Element declares a default of 8  
**When** the character is calculated  
**Then** the calculation uses 8 as a fallback and identifies it as such  
**And** the required answer remains missing  
**When** the player records 0 as the input  
**Then** the recorded 0 is used instead of the default

**Given** an ability input is absent and its Ability Element has no declared default  
**When** the character is calculated  
**Then** the ability input convention supplies 0  
**And** a required unanswered decision remains reported as missing

**Given** a recorded ability input is invalid  
**When** the character is calculated  
**Then** the invalid input is reported rather than replaced by the default or 0

## Do not let a conditional benefit qualify itself

**Given** a character has Strength 13 before a conditional item benefit  
**And** that benefit requires Strength 14 and would increase Strength by 1  
**When** the character is calculated  
**Then** the benefit does not apply and its unmet requirement is explained

## Equipment benefits require the right intent

**Given** a character has an armor class of 10 and owns a test shield that grants +2 armor class when equipped  
**And** the shield is not equipped  
**Then** the character's armor class remains 10  
**When** the player equips the shield in a valid loadout  
**Then** the character's armor class is 12

## Retain an unavailable equipment allocation

**Given** a character has allocated two units of an owned weapon stack\
**When** its quantity is reduced to one\
**Then** the two-unit allocation remains recorded and is reported as unavailable\
**And** the weapon does not produce an attack-ready profile\
**When** the owned item is removed\
**Then** the allocation remains visible after reopening until the player removes it

## Keep ordinary possessions separate from equipment

**Given** a character owns one test sword and has 12 GP\
**When** the player adds a stack of three test arrows and adjusts the balance to 15 GP\
**Then** the sword and arrows have distinct Owned Item identities\
**And** the sword remains unallocated\
**And** the character has 15 GP\
**When** the player changes the arrow stack to two and removes the sword\
**Then** the arrow stack retains its identity and quantity two\
**And** no equipment allocation or currency is changed

## Preview a safe starting-equipment correction

**Given** a character claimed a package containing one test sword and 4 GP\
**And** the player later acquired an unrelated test sword and adjusted the balance to 10 GP\
**When** the player changes the package Choice to one containing a test shield and 7 GP\
**Then** possessions and currency remain unchanged\
**And** the correction preview identifies only the claimed sword and its 4 GP\
**When** the player confirms the current safe preview\
**Then** only the claimed sword is replaced by the shield\
**And** the unrelated sword remains owned\
**And** the balance becomes 13 GP\
**And** confirming the same preview again does not change possessions or currency

## Block a starting-equipment correction with missing receipt property

**Given** a character claimed a package containing one test sword and 4 GP\
**And** the player removed the claimed sword\
**When** the player changes the package Choice and previews a correction\
**Then** the preview explains that the receipt sword is unavailable\
**And** confirming the preview is rejected without changing possessions, currency or receipt state

## Refresh a stale starting-equipment correction

**Given** a character has a safe preview to replace a claimed package\
**When** its currency or possessions change before confirmation\
**Then** confirming the old preview is rejected without partial changes\
**And** a refreshed preview is required before the correction can be applied

## Report attunement over capacity without choosing winners

**Given** a character's attunement maximum is 3  
**And** four otherwise eligible items each require a cost-one bond  
**And** another otherwise eligible item requires a cost-zero bond  
**When** their attunement intent is evaluated  
**Then** all four cost-one bonds are capacity-invalid without selecting a winning subset  
**And** the cost-zero bond remains eligible

## Keep a decorator edit atomic

**Given** an Owned Item has two compatible ordered Decorator Applications  
**And** one application requires a property supplied by the other  
**When** the player requests an incompatible ordering  
**Then** the edit is rejected and the item's composition and version remain unchanged  
**And** the failed compatibility is explained
