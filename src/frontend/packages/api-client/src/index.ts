// --- HTTP Client ---
export { createHttpClient } from "./client/http-client.ts";
export { ApiError } from "./client/api-error.ts";
export type { ClientConfig, RequestOptions } from "./client/types.ts";
export type { HttpClient } from "./client/http-client.ts";

// --- Platform domain ---
export { getPlatformStatus } from "./domains/platform/endpoints.ts";
export { platformQueryKeys } from "./domains/platform/query-keys.ts";
export { platformQueryOptions } from "./domains/platform/query-options.ts";
export type { Status } from "./domains/platform/types.ts";

// --- Characters domain ---
export {
  createCharacter,
  deleteCharacter,
  getAbilityScores,
  getCharacterCreationOptions,
  getCharacterDetails,
  getCharacterPortraitOptions,
  getCharacters,
  getSavingThrows,
  getSkills,
  updateAdditionalAbilityScore,
  updateBaseAbilityScore,
} from "./domains/characters/endpoints.ts";
export { characterQueryKeys } from "./domains/characters/query-keys.ts";
export { characterQueryOptions } from "./domains/characters/query-options.ts";
export type {
  AbilityScore,
  AbilityScores,
  CharacterCard,
  CharacterCards,
  CharacterCreationOption,
  CharacterCreationOptions,
  CharacterDetails,
  CharacterDetailsResponse,
  CharacterPortraitOption,
  CharacterPortraitOptions,
  CreateCharacterRequest,
  CreateCharacterResponse,
  SavingThrow,
  SavingThrows,
  Skill,
  Skills,
  UpdateAbilityScoreRequest,
  UpdateAbilityScoreResponse,
} from "./domains/characters/types.ts";

// --- Builder domain ---
export {
  getCharacterClasses,
  getRegistrations,
  getSelectionRuleOptions,
  getSelectionRules,
  getStatistics,
  registerSelection,
  unregisterSelection,
  updateClassLevel,
} from "./domains/builder/endpoints.ts";
export type {
  CharacterClassesResponse,
  GetSelectionRuleOptionsResponse,
  GetSelectionRulesResponse,
  RegistrationModels,
  StatisticsResponse,
} from "./domains/builder/endpoints.ts";
export { builderQueryKeys } from "./domains/builder/query-keys.ts";
export { builderQueryOptions } from "./domains/builder/query-options.ts";
export type {
  CharacterClass,
  RegisterSelectionRequest,
  RegisterSelectionResponse,
  RegistrationModel,
  SelectionRuleDataModel,
  SelectionRuleOptionDataModel,
  StatisticGroupDataModel,
  StatisticValueDataModel,
  UnregisterSelectionRequest,
  UpdateClassLevelRequest,
} from "./domains/builder/types.ts";
