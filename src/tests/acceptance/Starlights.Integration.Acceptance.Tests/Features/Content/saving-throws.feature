Feature: content management for saving throws

As a content creator
I want to create, update, delete and list saving throws
So that players can use those saving throws during character creation

Background:
    Given I am authenticated as a content creator

@saving-throw
Rule: A content creator can create a saving throw

    Scenario: a new saving throw with defaults
        Given an ability score exists with the name "Strength" and an abbreviation "STR"
        When a content creator creates a saving throw with the following properties
            | name     | ability  |
            | Strength | Strength |
        Then the saving throw should have at least the following properties
            | name     | ability  | description |
            | Strength | Strength |             |
    
@saving-throw
Rule: A content creator can update an existing saving throw

    Scenario: update an existing saving throw
        Given an ability score exists with the name "Constitution" and an abbreviation "CON"
        And a saving throw exists with the name "Constitution" and ability "Constitution"
        When the content creator updates the saving throw "Constitution" with the following properties
            | description                  |
            | Measures physical resilience |
        Then the saving throw "Constitution" should have at least the following properties
            | name         | ability      | description                  |
            | Constitution | Constitution | Measures physical resilience |

@saving-throw @ignore @backlog
Rule: A content creator can request to create a saving throw for an ability score

@saving-throw @proficiency @ignore @backlog
Rule: A content creator can request to create a proficiency for a saving throw
