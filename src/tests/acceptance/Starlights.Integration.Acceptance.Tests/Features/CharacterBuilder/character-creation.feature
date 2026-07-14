Feature: character creation

As a user
I want to create characters
So that I can use them in games

Background:
    Given the "Dungeons & Dragons" ruleset exists
    And I am authenticated as a user

Rule: The user can create a new character

    Scenario: create a character
        Given the user's character collection is empty
        When the user creates a character with the name "Bruenor Battlehammer"
        Then the user's character collection should include the character "Bruenor Battlehammer"

Rule: The user can create a new character with the same name as an existing character

    Scenario: create a character with a duplicate name
        Given the user's character collection includes a character named "Bruenor Battlehammer"
        When the user creates a character with the name "Bruenor Battlehammer"
        Then the user's character collection should include two characters named "Bruenor Battlehammer"

Rule: A new character should have a portrait

    Scenario: new character has a portrait
        When the user creates a character with the name "Bruenor"
        Then the character "Bruenor" should have a portrait

@ignore @backlog
Rule: A new character should have the user as the owner

    Scenario: new character is owned by the user
        When the user creates a character with the name "Bruenor"
        Then the character "Bruenor" should be owned by the user

Rule: The user can delete a character from their collection

    Scenario: delete a character
        Given the user's character collection includes the following characters:
            | name    |
            | Bruenor |
            | Drizzt  |
            | Wulfgar |
        When the user deletes the character with the name "Drizzt"
        Then the user's character collection should only include:
            | name    |
            | Bruenor |
            | Wulfgar |
