Feature: content management for ability scores

As a content creator
I want to create, update, delete and list ability scores
So that players can use those ability scores during character creation

Background:
    Given I am authenticated as a content creator

@ability-score
Rule: A content creator can create an ability score

    Scenario: a new ability score with defaults
        When a content creator creates an ability score with the following properties
            | name     | abbreviation |
            | Strength | STR          |
        Then the ability score should have at least the following properties
            | name     | abbreviation | description |
            | Strength | STR          |             |

@ability-score
Rule: A content creator can update an existing ability score

    Scenario: update an existing ability score
        Given an ability score exists with the name "Intelligence" and an abbreviation "INT"
        When the content creator updates the ability score "Intelligence" with the following properties
            | description               |
            | Measures mental acuity... |
        Then the ability score "Intelligence" should have at least the following properties
            | name         | abbreviation | description               |
            | Intelligence | INT          | Measures mental acuity... |
