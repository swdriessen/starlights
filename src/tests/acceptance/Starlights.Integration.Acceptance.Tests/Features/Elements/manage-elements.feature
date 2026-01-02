Feature: elements content management 

As a content creator
I want to manage elements
So that they are available during character and content creation

Anatomy of an Element

This file describes managing of generic elements that do not necessarily fit 
into other specific categories like spells, feats, or items. These elements
usually have no other required properties beside the name and an optional description. 
They can be updated as normal after creation.

When creating these elements, you provide the "Type" of the element.
This allows the system to categorize and handle them appropriately.

Background:
    Given I am authenticated as a content creator

@elements
Rule: A content creator can create a new generic element

    Scenario: create an element with defaults
    TODO: add compendium false as default check
        When the content creator creates an element with the following properties
            | name         | type |
            | Demo Element | Rule |
        Then the element should have at least the following properties
            | name         | type | description |
            | Demo Element | Rule |             |

    Scenario: create an element of a given type
        When the content creator creates an element with the following properties
            | name           | type        | description                |
            | Custom Element | Custom Type | A demo element for testing |
        Then the element should have at least the following properties
            | name           | type        | description                |
            | Custom Element | Custom Type | A demo element for testing |

@elements
Rule: A content creator can delete an element

    Scenario: delete an element
        Given an element exists with the name "Existing Element"
        When the content creator deletes the element with the name "Existing Element"
        Then the element list should not contain an element with the name "Existing Element"

    @ignore @backlog
    Scenario: bulk delete elements
        Given the following elements with their respective properties exists
            | name          | type |
            | Element One   | Rule |
            | Element Two   | Rule |
            | Element Three | Rule |
        When the content creator deletes the following elements:
            | name          |
            | Element One   |
            | Element Three |
        Then the element list should not contain elements with the following names:
            | name          |
            | Element One   |
            | Element Three |

@elements @ignore @backlog
Rule: A content creator can change the visibility of an element in the compendium

    Scenario: create an element available in the compendium
        When the content creator creates an element with the following properties
            | name            | type | compendium |
            | Visible Element | Rule | true       |
        Then the element should have at least the following properties
            | compendium |
            | true       |

    Scenario: create an element hidden from the compendium
        When the content creator creates an element with the following properties
            | name                     | type | compendium |
            | Hidden (Default) Element | Rule | false      |
        Then the element should have at least the following properties
            | compendium |
            | false      |
            
@labels
Rule: A content creator can add labels to an element to allow more granular selection rules

    Scenario: label a tool proficiency as a musical instrument
        Given an element exists with the name "Tool Proficiency (Pan Flute)"
        When the content creator adds the "Musical Instrument" label to the "Tool Proficiency (Pan Flute)" element
        Then the element "Tool Proficiency (Pan Flute)" should contain a "Musical Instrument" label

    Scenario: label an element with multiple labels
        Given an element exists with the name "Fireball"
        When the content creator adds the following labels to the "Fireball" element:
            | label     |
            | Fire      |
            | Evocation |
        Then the element "Fireball" should contain the following labels:
            | label     |
            | Fire      |
            | Evocation |
            
    Scenario: change labels on an element
        Given an element exists with the name "Wrongfully Labeled"
        And the content creator adds the following labels to the "Wrongfully Labeled" element:
            | label      |
            | Ice        |
            | Abjuration |
            |          1 |
        When the content creator updates the "Wrongfully Labeled" element with the following labels:
            | label   |
            | Simple  |
            | Martial |
            | Origin  |
            |       4 |
        Then the element "Wrongfully Labeled" should contain the following labels:
            | label   |
            | Simple  |
            | Martial |
            | Origin  |
            |       4 |




@description
Rule: A content creator can update the description of an existing element

    Scenario: update an existing element's description
        Given the content creator prepared the following markdown description
            """
            # Element Title
            This is a **bold** statement.
            - First item
            - Second item
            """
        And an element exists with the name "Updatable Element"
        When the content creator updates the element "Updatable Element" with the following properties
            | description            |
            | <markdown description> |
        Then the element "Updatable Element" should have at least the following properties
            | description            |
            | <markdown description> |