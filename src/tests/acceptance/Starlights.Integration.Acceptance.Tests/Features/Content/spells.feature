Feature: Content creation and management for spells

As a content creator
I want to create, update, delete and list spells
So that players can use those spells during character creation

Background:
    Given I am authenticated as a content creator

# Create scenarios

Scenario Outline: create a spell with the minimum required fields
    When the content creator creates a spell with the following properties
        | name   | level   | magic school   | casting time   | range   | duration   |
        | <name> | <level> | <magic school> | <casting time> | <range> | <duration> |
    Then the spell appears in the spell list with all provided properties

Examples:
    | name                   | level | magic school  | casting time   | range  | duration                        |
    | Firebolt               |     0 | Evocation     | 1 action       | 120 ft | Instantaneous                   |
    | Tenser's Floating Disk |     1 | Conjuration   | 1 action       | 30 ft  | 1 hour                          |
    | Shield of Faith        |     1 | Abjuration    | 1 bonus action | 60 ft  | Concentration, up to 10 minutes |
    | Enlarge/Reduce         |     2 | Transmutation | 1 action       | 30 ft  | Concentration, up to 1 minute   |
    | Wish                   |     9 | Conjuration   | 1 action       | 120 ft | Instantaneous                   |


Scenario: create a concentration spell
    When the content creator creates a spell with the following properties
        | name        | level | magic school | casting time | range  | duration                        | concentration |
        | Magic Spell |     5 | Evocation    | 1 action     | 100 ft | Concentration, up to 10 minutes | true          |
    Then the spell appears in the spell list as a concentration spell
    
Scenario: create a ritual spell
    When the content creator creates a spell with the following properties
        | name        | level | magic school | casting time | range  | duration | ritual |
        | Magic Spell |     5 | Evocation    | 1 action     | 100 ft | 1 hour   | true   |
    Then the spell appears in the spell list as a ritual spell
    
Scenario: create a spell with a somatic component
    When the content creator creates a spell with the following properties
        | name        | level | magic school | casting time | range  | duration | somatic |
        | Magic Spell |     5 | Evocation    | 1 action     | 100 ft | 1 hour   | true    |
    Then the spell appears in the spell list as having a somatic component

Scenario: create a spell with a verbal component
    When the content creator creates a spell with the following properties
        | name        | level | magic school | casting time | range  | duration | verbal |
        | Magic Spell |     5 | Evocation    | 1 action     | 100 ft | 1 hour   | true   |
    Then the spell appears in the spell list as having a verbal component

Scenario: create a spell with a material component
    When the content creator creates a spell with the following properties
        | name        | level | magic school | casting time | range  | duration | material | material components |
        | Magic Spell |     5 | Evocation    | 1 action     | 100 ft | 1 hour   | true     | a small crystal rod |
    Then the spell appears in the spell list as having a material component with the provided material components description

Scenario: create a spell with a description
    When the content creator creates a spell with the following properties
        | name        | level | magic school | casting time | range  | duration | description                                   |
        | Magic Spell |     5 | Evocation    | 1 action     | 100 ft | 1 hour   | A powerful evocation spell that deals damage. |
    Then the spell appears in the spell list with the provided description

Scenario: create a spell with all optional properties
    When the content creator creates a spell with the following properties
        | name  | level | magic school | casting time | range  | duration      | concentration | ritual | somatic | verbal | material | material components | description                                                                                  |
        | Haste |     1 | Evocation    | 1 action     | 120 ft | Instantaneous | true          | false  | true    | true   | true     | a piece of flint    | A bright streak flashes from your pointing finger to a target that you can see within range. |
    Then the spell appears in the spell list with all provided properties

# Update, delete and list scenarios

@ignore @wip
Scenario: update an existing spell's description
    Given a spell exists with the following properties
        | name     | level | magic school | casting time | range  | duration      |
        | Firebolt |     0 | Evocation    | 1 action     | 120 ft | Instantaneous |
    When the content creator updates the spell's description to
        | description                                                       |
        | A searing bolt of fire that deals fire damage to a single target. |
    Then the spell in the spell list shows the updated description for "Firebolt"

@ignore @wip
Scenario: delete an existing spell
    Given a spell exists with the following properties
        | name     | level | magic school | casting time | range  | duration      |
        | Firebolt |     0 | Evocation    | 1 action     | 120 ft | Instantaneous |
    When the content creator deletes the spell named "Firebolt"
    Then the spell named "Firebolt" no longer appears in the spell list

@ignore @wip
Scenario: list all existing spells
    Given the following spells exist
        | name          | level | magic school | casting time | range  | duration      |
        | Firebolt      |     0 | Evocation    | 1 action     | 120 ft | Instantaneous |
        | Magic Missile |     1 | Evocation    | 1 action     | 120 ft | Instantaneous |
    When the content creator requests the list of spells
    Then the returned list contains at least the following spells with their specified properties
        | name          | level | magic school | casting time | range  | duration      |
        | Firebolt      |     0 | Evocation    | 1 action     | 120 ft | Instantaneous |
        | Magic Missile |     1 | Evocation    | 1 action     | 120 ft | Instantaneous |