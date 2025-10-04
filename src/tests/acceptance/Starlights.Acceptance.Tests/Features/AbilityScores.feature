Feature: Ability Scores
	
	As a player
	I want the ability modifiers of my character to reflect score changes
	So that the abilities of my character are accurately represented

Background:
	Given a new character

Rule: the ability modifier should be updated when the score changes

Example: updated with a score increase	
	Given the character's Intelligence score is 12
	When the character's Intelligence score is changed to 15
    Then the character's Intelligence modifier should be 2

@otel
Example: updated with a score decrease
	Given the character's Wisdom score is 14
	When the character's Wisdom score is changed to 8
    Then the character's Wisdom modifier should be -1

Rule: the ability modifier should follow the standard formula (score - 10) / 2 rounded down

Scenario Outline: computing the modifier for a variety of Strength scores
	Given the character's Strength score is 10
	When the character's Strength score is changed to <Score>
	Then the character's Strength modifier should be <Modifier>

	Examples:
		| Score | Modifier |
		| 1     | -5       |
		| 2     | -4       |
		| 3     | -4       |
		| 4     | -3       |
		| 5     | -3       |
		| 6     | -2       |
		| 7     | -2       |
		| 8     | -1       |
		| 9     | -1       |
		| 10    | 0        |
		| 11    | 0        |
		| 12    | 1        |
		| 13    | 1        |
		| 14    | 2        |
		| 15    | 2        |
		| 16    | 3        |
		| 17    | 3        |
		| 18    | 4        |
		| 19    | 4        |
		| 20    | 5        |
		| 21    | 5        |
		| 22    | 6        |
		| 23    | 6        |
		| 24    | 7        |
		| 25    | 7        |
		| 26    | 8        |
		| 27    | 8        |
		| 28    | 9        |
		| 29    | 9        |
		| 30    | 10       |

@ignore @wip
Rule: the ability score maximum value for a new character is capped at 20

Scenario: attempting to set an ability score above the maximum
	Given the character's Charisma score is 18
	When the character's Charisma score is changed to 22
    Then the character's Charisma score should be 20
    And the character's Charisma modifier should be 5



