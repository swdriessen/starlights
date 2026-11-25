import { ApiError, type components } from "@starlights/api-client";
import { generatedApi } from "@/lib/api-client";

export type CharacterSummary = components["schemas"]["CharacterDetailsDataModel"];
export type RulesetOption = components["schemas"]["RulesetOptionResponse"];
export const charactersKey = ["saved-characters"] as const;
export const characterKey = (id: string) => [...charactersKey, id] as const;

export async function listCharacters(offset: number, signal: AbortSignal) {
  const { data, error, response } = await generatedApi.GET("/api/characters", { params: { query: { offset } }, signal });
  checkResponse(response, error);
  if (!data?.characters) throw new Error("The Character collection response is incomplete.");
  const nextOffset = data.nextOffset == null ? null : Number(data.nextOffset);
  if (nextOffset !== null && (!Number.isSafeInteger(nextOffset) || nextOffset < 0))
    throw new Error("The Character collection returned an invalid page.");
  return { characters: data.characters, nextOffset };
}

export async function getCharacter(id: string, signal: AbortSignal) {
  const { data, error, response } = await generatedApi.GET("/api/characters/{characterId}", { params: { path: { characterId: id } }, signal });
  checkResponse(response, error);
  if (!data?.character || (data.character.rulesetId && !data.character.content))
    throw new Error("The saved Character response is incomplete.");
  return data.character;
}

export async function getRulesetOptions(signal: AbortSignal) {
  const { data, error, response } = await generatedApi.GET("/api/characters/rulesets", { signal });
  checkResponse(response, error);
  if (!data?.length) throw new Error("No supported Character creation content is available.");
  return data;
}

export async function createCharacter({ name, option }: { name: string; option: RulesetOption }) {
  // A transport failure can occur after commit. Never retry a create automatically.
  const { data, error, response } = await generatedApi.POST("/api/characters", {
    body: { name, rulesetId: option.rulesetId, ...option.content },
  });
  checkResponse(response, error);
  if (!data?.id) throw new Error("The creation response is incomplete.");
  return data.id;
}

function checkResponse(response: Response, error: unknown) {
  if (response.ok) return;
  throw new ApiError(response.status, response.statusText, JSON.stringify(error ?? null));
}
