import type { HttpClient } from "../../client/http-client.ts";
import type { RequestOptions } from "../../client/types.ts";
import type {
  CharacterClass,
  RegisterSelectionRequest,
  RegisterSelectionResponse,
  RegistrationModel,
  SelectionRuleDataModel,
  SelectionRuleOptionDataModel,
  StatisticGroupDataModel,
  UnregisterSelectionRequest,
  UpdateClassLevelRequest,
} from "./types.ts";

export type GetSelectionRulesResponse = { rules: SelectionRuleDataModel[] };
export type GetSelectionRuleOptionsResponse = { options: SelectionRuleOptionDataModel[] };
export type RegistrationModels = { registrations: RegistrationModel[] };
export type CharacterClassesResponse = { classes: CharacterClass[] };
export type StatisticsResponse = { statistics: StatisticGroupDataModel[] };

export function getSelectionRules(client: HttpClient, characterId: string, type: string, options?: RequestOptions): Promise<GetSelectionRulesResponse> {
  return client.get<GetSelectionRulesResponse>(
    `/api/characters/${encodeURIComponent(characterId)}/builder/selection-rules?type=${encodeURIComponent(type)}`,
    options,
  );
}

export function getSelectionRuleOptions(
  client: HttpClient,
  characterId: string,
  selectionRuleId: string,
  options?: RequestOptions,
): Promise<GetSelectionRuleOptionsResponse> {
  return client.get<GetSelectionRuleOptionsResponse>(
    `/api/characters/${encodeURIComponent(characterId)}/builder/selection-rules/${encodeURIComponent(selectionRuleId)}/options`,
    options,
  );
}

export function registerSelection(
  client: HttpClient,
  characterId: string,
  selectionRuleId: string,
  payload: RegisterSelectionRequest,
  options?: RequestOptions,
): Promise<RegisterSelectionResponse> {
  return client.post<RegisterSelectionRequest, RegisterSelectionResponse>(
    `/api/characters/${encodeURIComponent(characterId)}/builder/selection-rules/${encodeURIComponent(selectionRuleId)}/register`,
    payload,
    options,
  );
}

export function unregisterSelection(
  client: HttpClient,
  characterId: string,
  selectionRuleId: string,
  payload: UnregisterSelectionRequest,
  options?: RequestOptions,
): Promise<void> {
  return client.post<UnregisterSelectionRequest, void>(
    `/api/characters/${encodeURIComponent(characterId)}/builder/selection-rules/${encodeURIComponent(selectionRuleId)}/unregister`,
    payload,
    options,
  );
}

export function getRegistrations(client: HttpClient, characterId: string, options?: RequestOptions): Promise<RegistrationModels> {
  return client.get<RegistrationModels>(`/api/characters/${encodeURIComponent(characterId)}/registrations`, options);
}

export function getCharacterClasses(client: HttpClient, characterId: string, options?: RequestOptions): Promise<CharacterClassesResponse> {
  return client.get<CharacterClassesResponse>(`/api/characters/${encodeURIComponent(characterId)}/classes`, options);
}

export function getStatistics(client: HttpClient, characterId: string, options?: RequestOptions): Promise<StatisticsResponse> {
  return client.get<StatisticsResponse>(`/api/characters/${encodeURIComponent(characterId)}/statistics`, options);
}

export function updateClassLevel(
  client: HttpClient,
  characterId: string,
  characterClassId: string,
  payload: UpdateClassLevelRequest,
  options?: RequestOptions,
): Promise<void> {
  return client.post<UpdateClassLevelRequest, void>(
    `/api/characters/${encodeURIComponent(characterId)}/classes/${encodeURIComponent(characterClassId)}/level`,
    payload,
    options,
  );
}
