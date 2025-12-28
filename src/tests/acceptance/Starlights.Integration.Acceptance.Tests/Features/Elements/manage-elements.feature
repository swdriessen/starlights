Feature: content management for generic elements

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
