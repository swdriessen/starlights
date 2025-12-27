Feature: content management for element rules

As a content creator
I want to create, update, delete, and list rules for elements
So that they are available during character creation

A rule provides the data, the actual handling of a rule happens during character creation.

Background:
    Given I am authenticated as a content creator

# statistic rules

Rule: A content creator can add a new statistic rule to an element

    Scenario: a statistic rule applies defaults
        Given an element exists with the name "Default Element"
        When the content creator adds a new statistic rule to the element with the following properties
            | name     | value |
            | strength |     2 |
        Then the element should have a statistic rule with the following properties
            | name     | value | level requirement |
            | strength |     2 |                 0 |

    Scenario: a statistic rule with a positive value
        Given an element exists with the name "Increase Health"
        When the content creator adds a new statistic rule to the element with the following properties
            | name   | value |
            | health |    +5 |
        Then the element should have a statistic rule with the following properties
            | name   | value |
            | health |     5 |

    Scenario: a statistic rule with a negative value
        Given an element exists with the name "Decrease Health"
        When the content creator adds a new statistic rule to the element with the following properties
            | name   | value |
            | health |    -5 |
        Then the element should have a statistic rule with the following properties
            | name   | value |
            | health |    -5 |

    Scenario: a statistic rule with a zero value
        Given an element exists with the name "Neutral Speed"
        When the content creator adds a new statistic rule to the element with the following properties
            | name  | value |
            | speed |     0 |
        Then the element should have a statistic rule with the following properties
            | name  | value |
            | speed |     0 |

    Scenario: a statistic rule with a named value
        Given an element exists with the name "Named Element"
        When the content creator adds a new statistic rule to the element with the following properties
            | name      | value       |
            | alertness | proficiency |
        Then the element should have a statistic rule with the following properties
            | name      | value       |
            | alertness | proficiency |
    
    @ignore @wip
    Scenario: a statistic rule with a negative named value
        Given an element exists with the name "Negative Element"
        When the content creator adds a new statistic rule to the element with the following properties
            | name      | value              |
            | alertness | -strength-modifier |
        Then the element should have a statistic rule with the following properties
            | name      | value              |
            | alertness | -strength-modifier |

Rule: A content creator can specify a stacking bonus on a statistic rule

    Scenario: a statistic rule with a stacking bonus
        Given an element exists with the name "Intellect Element"
        When the content creator adds a new statistic rule to the element with the following properties
            | name         | value | stacking bonus |
            | intelligence |     5 | Item           |
        Then the element should have a statistic rule with the following properties
            | name         | stacking bonus |
            | intelligence | item           |


Rule: A content creator can specify a display name on a statistic rule
    
    @ignore @wip
    Scenario: a statistic rule with a display name
        Given an element exists with the name "Barbarian Traits"
        When the content creator adds a new statistic rule to the element with the following properties
            | name   | value | display name |
            | wisdom |     4 | Barbarian    |
        Then the element should have a statistic rule with the following properties
            | name   | value | display name |
            | wisdom |     4 | Barbarian    |

Rule: A content creator can specify constraints in the form of requirements on a statistic rule

    Scenario: a statistic rule with a level requirement constraint
        Given an element exists with the name "Increase Dexterity"
        When the content creator adds a new statistic rule to the element with the following properties
            | name      | value | level requirement |
            | dexterity |     1 |                 5 |
        Then the element should have a statistic rule with the following properties
            | level requirement |
            |                 5 |

    @ignore @wip
    Scenario: a statistic rule with an equipment requirement constraint

    @ignore @wip
    Scenario: a statistic rule with a requirements expression constraint

Rule: A content creator can specify constraints in the form of minimum and maximum values on a statistic rule

    @ignore @wip
    Scenario: a statistic rule with a minimum value constraint

    @ignore @wip
    Scenario: a statistic rule with a maximum value constraint

Rule: All provided statistic names and values are normalized
        lowercase, spaces replaced by dashes, special characters removed
        apart from '-' for spaces and ':' for sub statistics e.g. 'hp:max'
        unless they are leading or trailing

    Scenario Outline: a statistic name is normalized
        Given an element exists with the name "Normalized Element"
        When the content creator adds a new statistic rule to the element with the following properties
            | name   | value  | stacking bonus |
            | <name> | <name> | <name>         |
        Then the element should have a statistic rule with the following properties
            | name       | value      | stacking bonus |
            | <expected> | <expected> | <expected>     |

        Examples:
            | name                | expected          |
            | Agility             | agility           |
            | Sneak Attack:die    | sneak-attack:die  |
            | HP:max              | hp:max            |
            | Luck (Fortune)      | luck-fortune      |
            | Wisdom/Insight      | wisdom-insight    |
            | Charisma & Presence | charisma-presence |
            | Intelligence#1      | intelligence-1    |
            | -leading-trailing-  | leading-trailing  |
            | :leading:trailing:  | leading:trailing  |
            | multiple:::colons:  | multiple:colons   |
            | multiple   spaces   | multiple-spaces   |
            | multiple---dash     | multiple-dash     |
            | spec!@#$%^&*()+char | spec-char         |

# include rules

Rule: A content creator can add a new include rule to an element

# selection rules

Rule: A content creator can add a new selection rule to an element

# generic rules

@ignore @wip
Rule: A content creator can re-arrange the order of the rules of an element

    Scenario: re-arrange statistic rules
        Given an element exists with the name "Mixed Boosts Element"
        And the element has the following statistic rules
            | name      | value |
            | strength  |     2 |
            | dexterity |     3 |
            | health    |     5 |
        When the content creator re-arranges the statistic rules to the following order
            | name      |
            | health    |
            | strength  |
            | dexterity |
        Then the element should have the statistic rules in the following order
            | name      |
            | health    |
            | strength  |
            | dexterity |


Rule: A content creator can delete rules from an element

    Scenario: delete a statistic rule
        Given an element exists with the name "Element with Statistic Rules"
        And the element has the following statistic rules
            | name      | value |
            | strength  |     2 |
            | dexterity |     3 |
            | health    |     5 |
        When the content creator deletes the statistic rule with the name "dexterity"
        Then the element should have the following statistic rules
            | name     | value |
            | strength |     2 |
            | health   |     5 |

    @ignore @wip
    Scenario: delete an include rule
        
    @ignore @wip
    Scenario: delete a selection rule
