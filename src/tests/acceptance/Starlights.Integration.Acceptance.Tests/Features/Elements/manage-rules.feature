Feature: content management for element rules

As a content creator
I want to create, update, delete, and list rules for elements
So that they are available during character creation

Background:
    Given I am authenticated as a content creator

Rule: a content creator can add a new statistic rule for an element

Scenario: create a statistic rule with defaults
    Given an element exists with the following properties
        | name              | type |
        | Increase Strength | Rule |
    When the content creator adds a new statistic rule to the element with the following properties
        | name     | value |
        | strength |     2 |
    Then the element should have a statistic rule with the following properties
        | name     | value | level requirement |
        | strength |     2 |                 0 |


Rule: a content creator can specify a value for a statistic rule

Scenario: create a statistic rule with a positive value
    Given an element exists with the following properties
        | name            | type |
        | Increase Health | Rule |
    When the content creator adds a new statistic rule to the element with the following properties
        | name   | value |
        | health |    +5 |
    Then the element should have a statistic rule with the following properties
        | name   | value |
        | health |    5 |

Scenario: create a statistic rule with a negative value
    Given an element exists with the following properties
        | name            | type |
        | Decrease Health | Rule |
    When the content creator adds a new statistic rule to the element with the following properties
        | name   | value |
        | health |    -5 |
    Then the element should have a statistic rule with the following properties
        | name   | value |
        | health |    -5 |

Scenario: create a statistic rule with a zero value
    Given an element exists with the following properties
        | name          | type |
        | Neutral Boost | Rule |
    When the content creator adds a new statistic rule to the element with the following properties
        | name  | value |
        | speed |     0 |
    Then the element should have a statistic rule with the following properties
        | name  | value |
        | speed |     0 |

Scenario: create a statistic rule with a named value
    Given an element exists with the following properties
        | name              | type |
        | Rule of Alertness | Rule |
    When the content creator adds a new statistic rule to the element with the following properties
        | name      | value       |
        | alertness | Proficiency |
    Then the element should have a statistic rule with the following properties
        | name      | value       |
        | alertness | proficiency |

Rule: a content creator can specify a level requirement on a statistic rule

Scenario: create a statistic rule with a level requirement
    Given an element exists with the following properties
        | name               | type |
        | Increase Dexterity | Rule |
    When the content creator adds a new statistic rule to the element with the following properties
        | name      | value | level requirement |
        | dexterity |     3 |                 5 |
    Then the element should have a statistic rule with the following properties
        | level requirement |
        |                 5 |

Rule: a content creator can specify a stacking bonus on a statistic rule

Scenario: create a statistic rule with a stacking bonus
    Given an element exists with the following properties
        | name            | type |
        | Intellect Power | Rule |
    When the content creator adds a new statistic rule to the element with the following properties
        | name         | value | stacking bonus |
        | intelligence |     5 | item           |
    Then the element should have a statistic rule with the following properties
        | name         | stacking bonus |
        | intelligence | item           |

@ignore @wip
Scenario: create a statistic rule with a display name
    Given an element exists with the following properties
        | name             | type |
        | Barbarian Traits | Rule |
    When the content creator adds a new statistic rule to the element with the following properties
        | name   | value | display name |
        | wisdom |     4 | Barbarian    |
    Then the element should have a statistic rule with the following properties
        | name   | value | display name |
        | wisdom |     4 | Barbarian    |


Rule: statistic rule names are normalized
lowercase, spaces replaced by dashes, special characters removed 
apart from '-' for spaces and ':' for sub statistics e.g. 'hp:max' 
unless they are leading or trailing

Scenario Outline: statistic rule name is normalized when a rule is created
    Given an element exists with the following properties
        | name               | type |
        | Normalized Element | Rule |
    When the content creator adds a new statistic rule to the element with the following properties
        | name             | value |
        | <statistic name> |     3 |
    Then the element should have a statistic rule with the following properties
        | name                      |
        | <expected statistic name> |

Examples:
    | statistic name          | expected statistic name |
    | Agility                 | agility                 |
    | Sneak Attack:die        | sneak-attack:die        |
    | HP:max                  | hp:max                  |
    | Luck (Fortune)          | luck-fortune            |
    | Wisdom/Insight          | wisdom-insight          |
    | Charisma & Presence     | charisma-presence       |
    | Intelligence#1          | intelligence-1          |
    | -leading-trailing-      | leading-trailing        |
    | :leading:trailing:      | leading:trailing        |
    | multiple:::colons:      | multiple:colons         |
    | multiple   spaces       | multiple-spaces         |
    | multiple---dash         | multiple-dash           |
    | special!@#$%^&*()+chars | special-chars           |

