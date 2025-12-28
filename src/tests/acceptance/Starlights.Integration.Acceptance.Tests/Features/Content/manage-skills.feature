Feature: content management for skills

As a content creator
I want to create, update, delete and list skills
So that players can use those skills during character creation

Background:
    Given I am authenticated as a content creator

@skill
Rule: A content creator can create a new skill

    Scenario: create a skill with defaults
        Given an ability score exists with the name "Dexterity" and an abbreviation "DEX"
        When the content creator creates a skill with the following properties
            | name       | ability   |
            | Acrobatics | Dexterity |
        Then the skill should have at least the following properties
            | name       | ability   | description |
            | Acrobatics | Dexterity |             |

@skill @ignore @backlog
Rule: A content creator can request to create a skill for an ability score

@skill @ignore @backlog
Rule: A content creator can request to create a proficiency for a skill