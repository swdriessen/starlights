Feature: feats content management

As a content creator
I want to create, update, delete and list feats
So that players can use those feats during character creation

Anatomy of a Feat

A feat is a special feature that provides a character with unique capabilities. 
Feats can enhance a character’s abilities, grant new skills, or provide special advantages in various situations.

- A feat is a member of a category
- A feat can have a prerequisite (optional)
- A feat can be repeatable (optional)

Background:
    Given I am authenticated as a content creator
    And the following feat categories exist
        | name           |
        | Origin         |
        | General        |
        | Fighting Style |
        | Epic Boon      |

@feat-categories
Rule: A content creator can create a new feat category
        
    Scenario: create a new feat category
        When the content creator creates a feat category with the name "Martial Arts"
        Then the feat category should exist in feat category list with the name "Martial Arts"

@feat-categories
Rule: A content creator can update a feat category

    Scenario: update the name of a feat category
        Given a feat category exists with the name "Stealth Techniques"
        When the content creator updates the feat category to have the name "Advanced Stealth Techniques"
        Then the feat category should exist in feat category list with the name "Advanced Stealth Techniques"

@feat-categories
Rule: A content creator can delete a feat category

    Scenario: delete a feat category
        Given a feat category exists with the name "Temporary Category"
        When the content creator deletes the element with the name "Temporary Category"
        Then the element list should not contain an element with the name "Temporary Category"

@feat
Rule: A content creator can create a feat with default properties

    Scenario: create a feat with default properties
        When the content creator creates a feat with the following properties
            | name  | category |
            | Alert | General  |
        Then the feat should have at least the following properties
            | name  | category | description | prerequisite | repeatable |
            | Alert | General  |             |              | false      |

    Scenario: create a feat with a description
        When the content creator creates a feat with the following properties
            | name  | category | description                        |
            | Alert | General  | A sudden warning to react quickly. |
        Then the feat should have at least the following properties
            | name  | description                        |
            | Alert | A sudden warning to react quickly. |

@feat
Rule: A content creator can create a feat with a prerequisite

    Scenario: create a feat with a prerequisite
        Given a feat exists with the name "Power Attack" and category "Fighting Style"
        When the content creator creates a feat with the following properties
            | name          | category       | prerequisite |
            | Greater Power | Fighting Style | Power Attack |
        Then the feat should have at least the following properties
            | name          | prerequisite |
            | Greater Power | Power Attack |

@feat
Rule: A content creator can create a repeatable feat

    Scenario: create a repeatable feat
        When the content creator creates a feat with the following properties
            | name    | category | repeatable |
            | Skilled | Origin   | true       |
        Then the feat should have at least the following properties
            | repeatable |
            | true       |

@feat
Rule: A content creator can update the category of a feat

    Scenario: update the category of a feat
        Given a feat exists that includes the following properties
            | name    | category |
            | Skilled | Origin   |
        When the content creator updates the feat with the following properties
            | name         | category |
            | Very Skilled | General  |
        Then the feat should have at least the following properties
            | name         | category |
            | Very Skilled | General  |

@feat
Rule: A content creator can update the description of a feat

    Scenario: update the description of a feat
        Given a feat exists that includes the following properties
            | name  | category | description |
            | Alert | General  |             |
        When the content creator updates the feat with the following properties
            | description                        |
            | A sudden warning to react quickly. |
        Then the feat should have at least the following properties
            | name  | description                        |
            | Alert | A sudden warning to react quickly. |

@feat
Rule: A content creator can update the prerequisites of a feat

    Scenario: update the prerequisites of a feat
        Given a feat exists that includes the following properties
            | name           | category       | prerequisite |
            | Greater Attack | Fighting Style |              |
        When the content creator updates the feat with the following properties
            | prerequisite           |
            | Fighting Style Feature |
        Then the feat should have at least the following properties
            | name           | category       | prerequisite           |
            | Greater Attack | Fighting Style | Fighting Style Feature |

@feat
Rule: A content creator can update the repeatable property of a feat

    Scenario: make a feat repeatable
        Given a feat exists that includes the following properties
            | name    | category | repeatable |
            | Skilled | Origin   | false      |
        When the content creator updates the feat with the following properties
            | repeatable |
            | true       |
        Then the feat should have at least the following properties
            | name    | category | repeatable |
            | Skilled | Origin   | true       |

    Scenario: make a feat non-repeatable
        Given a feat exists that includes the following properties
            | name              | category  | repeatable |
            | Boon of Truesight | Epic Boon | true       |
        When the content creator updates the feat with the following properties
            | repeatable |
            | false      |
        Then the feat should have at least the following properties
            | name              | category  | repeatable |
            | Boon of Truesight | Epic Boon | false      |

@feat
Rule: A content creator can delete a feat

    Scenario: delete a feat
        When the content creator creates a feat with the following properties
            | name              | category  |
            | Boon of Truesight | Epic Boon |
        And the content creator deletes the element with the name "Boon of Truesight"
        Then the element list should not contain an element with the name "Boon of Truesight"