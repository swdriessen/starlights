// --- Character creation ---

export type CharacterCreationOption = {
  id: string;
  name: string;
  shortDescription: string;
};

export type CharacterCreationOptions = {
  options: CharacterCreationOption[];
};

// --- Character portrait ---

export type CharacterPortraitOption = {
  url: string;
  description: string;
};

export type CharacterPortraitOptions = {
  portraits: CharacterPortraitOption[];
};

// --- Character list ---

export type CharacterCard = {
  characterId: string;
  name: string;
  portraitUrl?: string;
  level: number;
  build: string;
  isFavorite: boolean;
};

export type CharacterCards = {
  characters: CharacterCard[];
};

// --- Character detail ---

export type CharacterDetails = {
  characterId: string;
  name: string;
  portraitUrl?: string;
  build: string;
  level: number;
  isFavorite: boolean;
};

export type CharacterDetailsResponse = {
  character: CharacterDetails;
};

// --- Ability scores ---

export type AbilityScore = {
  id: string;
  abilityScoreId: string;
  name: string;
  abbreviation: string;
  baseScore: number;
  additionalScore: number;
  calculatedScore: number;
  calculatedModifier: number;
};

export type AbilityScores = {
  abilityScores: AbilityScore[];
};

// --- Saving throws ---

export type SavingThrow = {
  id: string;
  savingThrowId: string;
  name: string;
  abilityScoreId: string;
  abilityScoreAbbreviation: string;
  abilityScoreModifier: number;
  calculatedBonus: number;
  additionalBonus: number;
};

export type SavingThrows = {
  savingThrows: SavingThrow[];
};

// --- Skills ---

export type Skill = {
  id: string;
  skillId: string;
  name: string;
  abilityScoreId: string;
  abilityScoreAbbreviation: string;
  abilityScoreModifier: number;
  calculatedBonus: number;
  additionalBonus: number;
};

export type Skills = {
  skills: Skill[];
};

// --- Mutations: Create ---

export type CreateCharacterRequest = {
  CharacterCreationOptionId: string;
  Name: string;
  PortraitUrl?: string | null;
};

export type CreateCharacterResponse = {
  Id: string;
};

// --- Mutations: Ability score updates ---

export type UpdateAbilityScoreRequest = {
  abilityScoreId: string;
  value: number;
};

export type UpdateAbilityScoreResponse = AbilityScore;
