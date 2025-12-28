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

Rule: a content creator can create a new generic element

    Scenario: create a generic element of a given type
        When the content creator creates an element with the following properties
            | name         | type | description                |
            | Demo Element | Rule | A demo element for testing |
        Then the element should have at least the following properties
            | name         | type | description                |
            | Demo Element | Rule | A demo element for testing |

@ignore @wip
Rule: an element is available in the compendium by default

    Scenario: create a generic element available in the compendium
        When the content creator creates an element with the following properties
            | name        | type |
            | Any Element | Rule |
        Then the element should have at least the following properties
            | compendium |
            | true       |

@ignore @wip
Rule: a content creator can create a generic element hidden from the compendium

    Scenario: create a generic element hidden from the compendium
        When the content creator creates an element with the following properties
            | name           | type | compendium |
            | Hidden Element | Rule | false      |
        Then the element should have at least the following properties
            | compendium |
            | false      |
