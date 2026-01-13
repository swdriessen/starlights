Feature: proficiencies content management

As a content creator
I want to create, update, delete and list proficiencies
So that players can use those proficiencies during character creation

Background:
    Given I am authenticated as a content creator

@proficiency
Rule: A content creator can create a proficiency

    @saving-throw
    Scenario: create a saving throw proficiency with defaults
        Given an ability score exists with the name "Dexterity" and an abbreviation "DEX"
        When the content creator creates a proficiency with the following properties
            | name      | proficiency type |
            | Dexterity | Saving Throw     |
        Then the proficiency should have at least the following properties
            | name      | proficiency type | description |
            | Dexterity | Saving Throw     |             |
        And the element should have a statistic rule with the following properties
            | name                               | value       | stacking bonus | display name |
            | dexterity:saving-throw:proficiency | proficiency | proficiency    | Proficiency  |

    @skill
    Scenario: create a skill proficiency with defaults
        When the content creator creates a proficiency with the following properties
            | name            | proficiency type |
            | Animal Handling | Skill            |
        Then the proficiency should have at least the following properties
            | name            | proficiency type | description |
            | Animal Handling | Skill            |             |
        And the element should have a statistic rule with the following properties
            | name                              | value       | stacking bonus | display name |
            | animal-handling:skill:proficiency | proficiency | proficiency    | Proficiency  |
            
    @tool
    Scenario: create a tool proficiency with defaults
        Given an ability score exists with the name "Intelligence" and an abbreviation "INT"
        When the content creator creates a proficiency with the following properties
            | name                 | proficiency type |
            | Alchemist’s Supplies | Tool             |
        Then the proficiency should have at least the following properties
            | name                 | proficiency type | description |
            | Alchemist’s Supplies | Tool             |             |
        And the element should have a statistic rule with the following properties
            | name                                 | value       | stacking bonus | display name |
            | alchemists-supplies:tool:proficiency | proficiency | proficiency    | Proficiency  |

    @weapon
    Scenario: create a weapon proficiency with defaults
        When the content creator creates a proficiency with the following properties
            | name      | proficiency type |
            | Longsword | Weapon           |
        Then the proficiency should have at least the following properties
            | name      | proficiency type | description |
            | Longsword | Weapon           |             |
        And the element should have a statistic rule with the following properties
            | name                         | value       | stacking bonus | display name |
            | longsword:weapon:proficiency | proficiency | proficiency    | Proficiency  |

    @vehicle
    Scenario: create a vehicle proficiency with defaults
        When the content creator creates a proficiency with the following properties
            | name    | proficiency type |
            | Airship | Vehicle          |
        Then the proficiency should have at least the following properties
            | name    | proficiency type | description |
            | Airship | Vehicle          |             |
        And the element should have a statistic rule with the following properties
            | name                        | value       | stacking bonus | display name |
            | airship:vehicle:proficiency | proficiency | proficiency    | Proficiency  |
