Feature: Content management for languages

As a content creator
I want to create, update, delete and list languages
So that players can use those languages during character creation

Background:
    Given I am authenticated as a content creator

# Create scenarios

@language
Rule: A content creator can create languages with the minimum required fields

    Scenario Outline: create a language of a specific kind
        When the content creator creates a language with the following properties
            | name       | kind   |
            | <language> | <kind> |
        Then the language appears in the language list with all provided properties

        Examples:

            | language | kind     |
            | Common   | Standard |
            | Sylvan   | Rare     |

@language
Rule: A content creator can create a language with an origin

    Scenario: create a language with an origin
        When the content creator creates a language with the following properties
            | name     | kind | origin                   |
            | Infernal | Rare | Devils of the Nine Hells |
        Then the language appears in the language list with all provided properties

@language
Rule: A content creator can create a lanaguage with a description

    Scenario: create a language with a description
        Given the content creator prepared the following markdown description
            """
            Primordial includes the _Aquan_, _Auran_, _Ignan_, and _Terran_ dialects. 
            Creatures that know one of these dialects can communicate with those that know a different one.
            """
        When the content creator creates a language with the following properties
            | name       | kind | origin     | description            |
            | Primordial | Rare | Elementals | <markdown description> |
        Then the language appears in the language list with all provided properties

@language
Rule: A content creator can update a language's name and kind

    Scenario: update a language's name and kind
        Given a language exists that includes the following properties
            | name   | kind     |
            | Elvish | Standard |
        When the content creator updates the language to have the following properties
            | name    | kind |
            | Druidic | Rare |
        Then the language in the language list should have the following properties
            | name    | kind |
            | Druidic | Rare |

@language
Rule: A content creator can update a language's origin

    Scenario: update a language's origin
        Given a language exists that includes the following properties
            | name     | origin         |
            | Infernal | The Nine Hells |
        When the content creator updates the language to have the following properties
            | name     | origin                   |
            | Infernal | Devils of the Nine Hells |
        Then the language in the language list should have the following properties
            | name     | origin                   |
            | Infernal | Devils of the Nine Hells |

@language
Rule: A content creator can update a language's description

    Scenario: update a language's description
        Given a language exists that includes the following properties
            | name          | description |
            | Sign Language |             |
        When the content creator updates the language to have the following properties
            | name                 | description                        |
            | Common Sign Language | This language originated in Sigil. |
        Then the language in the language list should have the following properties
            | name                 | description                        |
            | Common Sign Language | This language originated in Sigil. |
