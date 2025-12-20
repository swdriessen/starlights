Feature: Content Creation (Spells)

As a content creator
I want to create and manage spells
So that players can use them in the character creation process

Terminology:
    - content creator: manages game content
    - player: selects spells during character creation

Rule: A content creator should be able to create a new spell

Scenario: creating a level 0 spell
    When the content creator creates a spell with the following properties
        | Name     | Level | Casting Time | Range  | Duration      |
        | Firebolt |     0 | 1 action     | 120 ft | Instantaneous |
    Then the spell is created successfully with a unique identifier

Scenario: creating a level 1 spell
    When the content creator creates a spell with the following properties
        | Name          | Level | Casting Time | Range  | Duration      |
        | Magic Missile |     1 | 1 action     | 120 ft | Instantaneous |
    Then the spell is created successfully with a unique identifier

Scenario: creating a level 9 spell
    When the content creator creates a spell with the following properties
        | Name | Level | Casting Time | Range  | Duration      |
        | Wish |     9 | 1 action     | 120 ft | Instantaneous |
    Then the spell is created successfully with a unique identifier

Rule: a content creator should be able to update an existing spell

Rule: a content creator should be able to delete an existing spell

Rule: a content creator should be able to list all existing spells
