Feature: content management for proficiencies

As a content creator
I want to create, update, delete and list proficiencies
So that players can use those proficiencies during character creation

Background:
    Given I am authenticated as a content creator

@proficiency @saving-throw @ignore @wip
Rule: A content creator can create a proficiency

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
            | dexterity:saving-throw:proficiency | proficiency | proficiency    | Proficiency |