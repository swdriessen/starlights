Feature: character builder ability scores

As a user
I want to change the ability scores of my character
So that I can influence my character build

The ability score generation methods that exist are:

- standard array
- rolling dice (3d6, 4d6 discard lowest, also supports manual input)
- point buy (27 points)

The current default score is 10, this might change to 8 due to the point buy generation. 
With 10 this means that the user has 15/27 points left to spend.

Background:
    Given the "Dungeons & Dragons" ruleset exists
    And I am authenticated as a user
    
Rule: A new character should have all ability scores set to 10 by default

    Scenario: new character has default ability scores
        When the user creates a character
        Then the character should have the following ability scores:
            | ability      | base score |
            | Strength     |          10 |
            | Dexterity    |          10 |
            | Constitution |          10 |
            | Intelligence |          10 |
            | Wisdom       |          10 |
            | Charisma     |          10 |

@ignore @backlog
Rule: The user can change the ability score generation method for a character

    Scenario Outline: choose generation method
        Given the user creates a character
        When the user chooses the "<method>" ability score generation method for the character
        Then the character should have the "<method>" ability score generation method

        Examples:
            | method         |
            | standard array |
            | rolling dice   |
            | point buy      |

@ignore @backlog
Rule: When the user chooses the standard array generation method, the array is assigned to the ability scores

    Scenario: standard array choice assigns array
        Given the user creates a character
        When the user chooses the "standard array" ability score generation method for the character
        Then the character should have the following ability scores:
            | ability      | base score |
            | Strength     |         15 |
            | Dexterity    |         14 |
            | Constitution |         13 |
            | Intelligence |         12 |
            | Wisdom       |         10 |
            | Charisma     |          8 |

@ignore @backlog
Rule: When the user chooses the rolling dice generation method, the ability scores do not change

    Scenario: rolling dice choice do not reset
        Given the user has a character with the "standard array" ability score generation method
        And the character has the following ability scores:
            | ability      | base score |
            | Strength     |          8 |
            | Dexterity    |         12 |
            | Constitution |         13 |
            | Intelligence |         15 |
            | Wisdom       |         14 |
            | Charisma     |         10 |
        When the user chooses the "rolling dice" ability score generation method for the character
        Then the character should have the following ability scores:
            | ability      | base score |
            | Strength     |          8 |
            | Dexterity    |         12 |
            | Constitution |         13 |
            | Intelligence |         15 |
            | Wisdom       |         14 |
            | Charisma     |         10 |

@ignore @backlog
Rule: The point buy generation method should have a total of 27 points available

    Scenario: starting amount of point buy points
        Given the user creates a character
        And the character has the following ability scores:
            | ability      | base score |
            | Strength     |          8 |
            | Dexterity    |          8 |
            | Constitution |          8 |
            | Intelligence |          8 |
            | Wisdom       |          8 |
            | Charisma     |          8 |
        When the user chooses the "point buy" ability score generation method for the character
        Then the character should have 27 point buy points available

@ignore @backlog
Rule: When the user chooses the point buy generation method, the points are recalculated based on the current ability scores
    
    Scenario: point buy choice recalculates
        Given the user has a character with the "rolling dice" ability score generation method
        And the character has the following ability scores:
            | ability      | base score |
            | Strength     |         15 |
            | Dexterity    |         12 |
            | Constitution |         12 |
            | Intelligence |          9 |
            | Wisdom       |         13 |
            | Charisma     |          6 |
        When the user chooses the "point buy" ability score generation method for the character
        Then the character should have the following ability scores:
            | ability      | base score |
            | Strength     |         15 |
            | Dexterity    |         12 |
            | Constitution |         12 |
            | Intelligence |          9 |
            | Wisdom       |         13 |
            | Charisma     |          6 |
        And the character should have 99 point buy points available










Rule: The user can changes the value of ability scores

    Scenario: change ability score
        Given the user creates a character
        And the character has the following ability scores:
            | ability      | base score |
            | Strength     |         12 |
            | Dexterity    |          8 |
            | Constitution |          8 |
            | Intelligence |          8 |
            | Wisdom       |          8 |
            | Charisma     |          8 |
        When the user changes the character's "Strength" ability score to 18
        Then the character should have the following ability scores:
            | ability      | base score |
            | Strength     |         18 |
            | Dexterity    |          8 |
            | Constitution |          8 |
            | Intelligence |          8 |
            | Wisdom       |          8 |
            | Charisma     |          8 |

@ignore @backlog
Rule: The user can swap ability scores
this has to be an atomic operation

    Scenario: swap ability score
        Given the user creates a character
        And the character has the following ability scores:
            | ability      | base score |
            | Strength     |         10 |
            | Dexterity    |         14 |
            | Constitution |          8 |
            | Intelligence |          8 |
            | Wisdom       |          8 |
            | Charisma     |          8 |
        When the user swaps the character's "Strength" ability score with the "Dexterity" ability score
        Then the character should have the following ability scores:
            | ability      | base score |
            | Strength     |         14 |
            | Dexterity    |         10 |
            | Constitution |          8 |
            | Intelligence |          8 |
            | Wisdom       |          8 |
            | Charisma     |          8 |

