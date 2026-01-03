Feature: spells content management

As a content creator
I want to create, update, delete and list spells
So that players can use those spells during character creation

Background:
    Given I am authenticated as a content creator

@spell
Rule: A content creator can create spells with the minimum required fields

    Scenario: create a spell with the minimum required fields
        When the content creator creates a spell with the following properties
            | name        | level | magic school | casting time | range  | duration      |
            | Magic Spell |     0 | Evocation    | 1 action     | 100 ft | Instantaneous |
        Then the spell appears in the spell list with all provided properties

@spell
Rule: A content creator can create spells up to level 9

    Scenario Outline: create a spell with a specific level
        When the content creator creates a spell with the following properties
            | name   | level   | magic school   | casting time   | range   | duration   |
            | <name> | <level> | <magic school> | <casting time> | <range> | <duration> |
        Then the spell appears in the spell list with all provided properties

        Examples:
            | name                       | level | magic school | casting time   | range | duration                      |
            | Evocation cantrip          |     0 | Evocation    | 1 bonus action | 10 ft | Instantaneous                 |
            | 1st-level necromancy spell |     1 | Necromancy   | 1 action       | Touch | 1 minute                      |
            | 9th-level abjuration spell |     9 | Abjuration   | 1 action       | 30 ft | Concentration, up to 1 minute |

@spell
Rule: A content creator can create spells that require concentration

    Scenario: create a concentration spell
        When the content creator creates a spell with the following properties
            | name        | level | magic school | casting time | range  | duration                        | concentration |
            | Magic Spell |     5 | Evocation    | 1 action     | 100 ft | Concentration, up to 10 minutes | true          |
        Then the spell appears in the spell list as a concentration spell

@spell
Rule: A content creator can create spells that can be cast as rituals

    Scenario: create a ritual spell
        When the content creator creates a spell with the following properties
            | name        | level | magic school | casting time | range  | duration | ritual |
            | Magic Spell |     5 | Evocation    | 1 action     | 100 ft | 1 hour   | true   |
        Then the spell appears in the spell list as a ritual spell

@spell
Rule: A content creator can create spells that require a combination of somatic, verbal, and material components

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

@spell
Rule: A content creator can create spells with detailed descriptions

    Scenario: create a spell with a description
        When the content creator creates a spell with the following properties
            | name        | level | magic school | casting time | range  | duration | description                                   |
            | Magic Spell |     5 | Evocation    | 1 action     | 100 ft | 1 hour   | A powerful evocation spell that deals damage. |
        Then the spell appears in the spell list with the provided description

    Scenario: create a spell with a markdown description
        Given the content creator prepared the following markdown description
            """
            You create an acidic bubble at a point within range,
            where it explodes in a 5-foot-radius Sphere. Each
            creature in that Sphere must succeed on a Dexterity
            saving throw or take 1d6 Acid damage.
            __Cantrip Upgrade.__ The damage increases by 1d6
            when you reach levels 5 (2d6), 11 (3d6), and 17
            (4d6).
            """
        When the content creator creates a spell with the following properties
            | name     | level | magic school | casting time | range  | duration      | description            |
            | Fireball |     3 | Evocation    | 1 action     | 150 ft | Instantaneous | <markdown description> |
        Then the spell appears in the spell list with the provided description
        
@spell
Rule: A content creator can update existing spells

    Scenario: update the required fields of an existing spell
        Given a spell exists that includes the following properties
            | name          | level | magic school | casting time | range  | duration |
            | Level 3 Spell |     3 | Evocation    | 1 action     | 100 ft | 1 minute |
        When the content creator updates the spell with the following properties
            | name          | level | magic school | casting time   | range  | duration  |
            | Level 6 Spell |     6 | Conjuration  | 1 bonus action | 200 ft | 2 minutes |
        Then the spell in the spell list should have the following properties
            | name          | level | magic school | casting time   | range  | duration  |
            | Level 6 Spell |     6 | Conjuration  | 1 bonus action | 200 ft | 2 minutes |

    Scenario: update a non concentration spell to a concentration spell
        Given a spell exists that includes the following properties
            | name          | duration      | concentration |
            | Instant Spell | Instantaneous | false         |
        When the content creator updates the spell with the following properties
            | name                | duration                      | concentration |
            | Concentrating Spell | Concentration, up to 1 minute | true          |
        Then the spell in the spell list should have the following properties
            | name                | duration                      | concentration |
            | Concentrating Spell | Concentration, up to 1 minute | true          |

    Scenario: update a non ritual spell to a ritual spell
        Given a spell exists that includes the following properties
            | name        | ritual |
            | Quick Spell | false  |
        When the content creator updates the spell with the following properties
            | name         | ritual |
            | Ritual Spell | true   |
        Then the spell in the spell list should have the following properties
            | name         | ritual |
            | Ritual Spell | true   |
        
    Scenario: update a components of an existing spell
        Given a spell exists that includes the following properties
            | name       | verbal | somatic | material |
            | Free Spell | false  | false   | false    |
        When the content creator updates the spell with the following properties
            | name            | verbal | somatic | material | material components |
            | Component Spell | true   | true    | true     | a black feather     |
        Then the spell in the spell list should have the following properties
            | name            | verbal | somatic | material | material components |
            | Component Spell | true   | true    | true     | a black feather     |

    Scenario: update the description of an existing spell
        Given a spell exists that includes the following properties
            | name          | description       |
            | Cryptic Spell | You cast a spell. |
        When the content creator updates the spell with the following properties
            | name              | description                     |
            | Descriptive Spell | You cast a very detailed spell. |
        Then the spell in the spell list should have the following properties
            | name              | description                     |
            | Descriptive Spell | You cast a very detailed spell. |

    Scenario: update the markdown description of an existing spell
        Given the content creator prepared the following markdown description
            """
            You create a very details markdown spell description
            that spans multiple lines.
            __Description Upgrade.__ The description increase by 1d6
            when you reach levels 5 (2d6), 11 (3d6), and 17
            (4d6).
            """
        And a spell exists that includes the following properties
            | name         | description                |
            | Simple Spell | You create a simple spell. |
        When the content creator updates the spell with the following properties
            | name           | description            |
            | Markdown Spell | <markdown description> |
        Then the spell in the spell list should have the following properties
            | name           | description            |
            | Markdown Spell | <markdown description> |