Feature: content management for feats

As a content creator
I want to create, update, delete and list feats
So that players can use those feats during character creation

Anatomy of a Feat

A feat is a special feature that provides a character with unique capabilities. 
Feats can enhance a character’s abilities, grant new skills, or provide special advantages in various situations.

- A feat is a member of a category
- A feat can have a prerequisite (optional)
- A feat can be repeatable (optional)

Feat Category

A feat can have one of the following categories (in the future, more categories may be added):

- Origin
- General
- Fighting Style
- Epic Boon

Background:
    Given I am authenticated as a content creator
    And the following feat categories exist
        | name           |
        | Origin         |
        | General        |
        | Fighting Style |
        | Epic Boon      |

# Feat Category scenarios

Rule: A content creator can create a new feat category
        
Scenario: create a new feat category
    When the content creator creates a feat category with the name "Martial Arts"
    Then the feat category should exist in feat category list with the name "Martial Arts"

Rule: A content creator can update a feat category

Scenario: update the name of a feat category
    Given a feat category exists with the name "Stealth Techniques"
    When the content creator updates the feat category to have the name "Advanced Stealth Techniques"
    Then the feat category should exist in feat category list with the name "Advanced Stealth Techniques"

# Feat scenarios

Rule: A content creator can create a feat with default properties

Scenario: create a feat
    When the content creator creates a feat with the following properties
        | name  | category |
        | Alert | General  |
    Then the feat should exist in the feat list with all provided properties

Rule: A content creator can create a feat with a prerequisite

Scenario: create a feat with a prerequisite
    Given a feat exists with the name "Power Attack" and category "Fighting Style"
    When the content creator creates a feat with the following properties
        | name          | category       | prerequisite |
        | Greater Power | Fighting Style | Power Attack |
    Then the feat should have at least the following properties
        | name          | prerequisite |
        | Greater Power | Power Attack |

Rule: A content creator can create a repeatable feat

Scenario: create a repeatable feat
    When the content creator creates a feat with the following properties
        | name    | category | repeatable |
        | Skilled | Origin   | true       |
    Then the feat should have at least the following properties
        | repeatable |
        | true       |


