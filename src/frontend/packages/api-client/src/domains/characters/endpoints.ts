import type { HttpClient } from "../../client/http-client.ts";
import type { RequestOptions } from "../../client/types.ts";
import type {
  AbilityScores,
  CharacterCards,
  CharacterCreationOptions,
  CharacterDetailsResponse,
  CharacterPortraitOptions,
  CreateCharacterRequest,
  CreateCharacterResponse,
  SavingThrows,
  Skills,
  UpdateAbilityScoreResponse,
} from "./types.ts";

export function getCharacters(client: HttpClient, options?: RequestOptions): Promise<CharacterCards> {
  return client.get<CharacterCards>("/api/characters", options);
}

export function getCharacterDetails(client: HttpClient, characterId: string, options?: RequestOptions): Promise<CharacterDetailsResponse> {
  return client.get<CharacterDetailsResponse>(`/api/characters/${encodeURIComponent(characterId)}`, options);
}

export function getCharacterCreationOptions(client: HttpClient, options?: RequestOptions): Promise<CharacterCreationOptions> {
  return client.get<CharacterCreationOptions>("/api/characters/creation-options", options);
}

export function getCharacterPortraitOptions(client: HttpClient, options?: RequestOptions): Promise<CharacterPortraitOptions> {
  return client.get<CharacterPortraitOptions>("/api/characters/portrait-options", options);
}

export function getAbilityScores(client: HttpClient, characterId: string, options?: RequestOptions): Promise<AbilityScores> {
  return client.get<AbilityScores>(`/api/characters/${encodeURIComponent(characterId)}/ability-scores`, options);
}

export function getSavingThrows(client: HttpClient, characterId: string, options?: RequestOptions): Promise<SavingThrows> {
  return client.get<SavingThrows>(`/api/characters/${encodeURIComponent(characterId)}/saving-throws`, options);
}

export function getSkills(client: HttpClient, characterId: string, options?: RequestOptions): Promise<Skills> {
  return client.get<Skills>(`/api/characters/${encodeURIComponent(characterId)}/skills`, options);
}

export function createCharacter(client: HttpClient, payload: CreateCharacterRequest, options?: RequestOptions): Promise<CreateCharacterResponse> {
  return client
    .post<CreateCharacterRequest, unknown>("/api/characters", payload, {
      ...options,
      expectedStatus: 201,
    })
    .then((raw) => {
      const any = raw as { Id?: string; id?: string } | null | undefined;
      const Id = any?.Id ?? any?.id;
      if (!Id) throw new Error("Create character response did not include an Id");
      return { Id };
    });
}

export function deleteCharacter(client: HttpClient, characterId: string, options?: RequestOptions): Promise<void> {
  return client.delete<void>(`/api/characters/${encodeURIComponent(characterId)}`, options);
}

export function updateBaseAbilityScore(
  client: HttpClient,
  characterId: string,
  abilityScoreId: string,
  value: number,
  options?: RequestOptions,
): Promise<UpdateAbilityScoreResponse> {
  return client.post<{ value: number }, UpdateAbilityScoreResponse>(
    `/api/characters/${encodeURIComponent(characterId)}/ability-scores/${encodeURIComponent(abilityScoreId)}/base`,
    { value },
    options,
  );
}

export function updateAdditionalAbilityScore(
  client: HttpClient,
  characterId: string,
  abilityScoreId: string,
  value: number,
  options?: RequestOptions,
): Promise<UpdateAbilityScoreResponse> {
  return client.post<{ value: number }, UpdateAbilityScoreResponse>(
    `/api/characters/${encodeURIComponent(characterId)}/ability-scores/${encodeURIComponent(abilityScoreId)}/additional`,
    { value },
    options,
  );
}
