Feature: class content management

As a content creator
I want to create, update, delete and list classes
So that players can use those classes during character creation

Background:
    Given I am authenticated as a content creator

@class
Rule: A content creator can create a class

    Scenario: create a new class element
        When the content creator creates a "Barbarian" class with a "D12" hit point die
        Then the "Barbarian" class should have at least the following properties
            | type  | hit point die |
            | Class | D12           |

@class
Rule: A content creator can create a class feature for a class

    Scenario: create a level 2 class feature
        Given a class exists with the name "Barbarian"
        When the content creator creates a level 2 class feature for the "Barbarian" class with the name "Danger Sense"
        Then the "Danger Sense" class feature should have at least the following properties
            | name         | type          | feature level | feature parent |
            | Danger Sense | Class Feature |             2 | Barbarian      |

@class
Rule: A content creator can create a subclass for a class

    Scenario: create a barbarian subclass
        Given a class exists with the name "Barbarian"
        When the content creator creates a subclass for the "Barbarian" class with the name "Path of the Totem Warrior"
        Then the "Path of the Totem Warrior" subclass should have at least the following properties
            | name                      | type      | feature parent |
            | Path of the Totem Warrior | Sub Class | Barbarian      |

@class @labels
Rule: A created subclass automatically gets a label matching its parent class
    
    Scenario: verify subclass label
        Given a class exists with the name "Rogue"
        When the content creator creates a subclass for the "Rogue" class with the name "Thief"
        Then the "Thief" subclass should contain a "Rogue" label

@spellcasting @ignore @backlog
Rule: A content creator can create class feature with a spellcasting rule

    Scenario: create a spellcasting feature for a wizard
        Given a class exists with the name "Wizard" with the following class features:
            | name         | type          | feature level | feature parent |
            | Spellcasting | Class Feature |             1 | Wizard         |
        When the content creator adds a new spellcasting rule to the "Spellcasting" class feature with the following details:
            | spellcaster name | spellcasting ability |
            | Wizard           | Intelligence         |
        Then the "Spellcasting" element should have at least the following properties
            | name         | spellcasting name | spellcasting ability |
            | Spellcasting | Wizard            | Intelligence         |

@spellcasting @ignore @backlog
Rule: A content creator can create a spell list for a spellcaster class

    Background: :
        Given the following spells with their respective properties exists
            | name          | level | school      |
            | Light         |     0 | Conjuration |
            | Magic Missile |     1 | Evocation   |
            | Shield        |     2 | Abjuration  |
            | Fireball      |     3 | Evocation   |

    Scenario: create a spell list for the Wizard class
        Given a spellcaster class exists with the name "Wizard" and a spellcasting feature named "Spellcasting"
        When the content creator updates to the spellcasting rule of the "Spellcasting" class feature with the following "Wizard" spell list:
            | spell         |
            | Light         |
            | Magic Missile |
            | Shield        |
            | Fireball      |
        Then the "Spellcasting" class feature should have at least the following spells in the "Wizard" spell list:
            | spell         |
            | Light         |
            | Magic Missile |
            | Shield        |
            | Fireball      |