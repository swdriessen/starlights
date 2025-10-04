Feature: Proficiency Rules
	
	As a player
	I want my character's proficiency bonus to scale with their level
	So that my character's skills and attacks improve as they gain experience

    Note: a character's level is based on the combined levels of all classes
 
Rule: Proficiency bonus is determined by character level

    Scenario Outline: proficiency bonus at first level
        Given the player creates a new character
        When the player selects the "Barbarian" class as their starting class
        Then the character's proficiency bonus should be 2

    Scenario Outline: proficiency bonus at higher levels
        Given the player creates a new character
        And the player selects the "Rogue" class as their starting class
        When the player changes the level of the "Rogue" class to <Level>
        Then the character's level should be <Level>
        And the character's proficiency bonus should be <Proficiency Bonus>

        Examples:
        | Level | Proficiency Bonus |
        | 4     | 2                 |
        | 5     | 3                 |
        | 9     | 4                 |
        | 13    | 5                 |
        | 17    | 6                 |
        | 20    | 6                 |

    @ignore @backlog
    Scenario: proficiency bonus for multiclass characters    
