import { ApiError, type components } from "@starlights/api-client";
import { generatedApi } from "@/lib/api-client";
import { characterKey } from "./characters-api";
import { readInteger, readRevision } from "./reliable-edit-state";

type ApiCharacterBuild = components["schemas"]["CharacterBuildResponse"];
export type CharacterBuild = ReturnType<typeof result>;
export type CharacterEdit =
  | { kind: "name"; name: string }
  | { kind: "choice"; selectionId: string; answers: string[] }
  | { kind: "abilities"; inputs: Record<string, number> }
  | { kind: "claim"; selectionId: string }
  | { kind: "equipment"; armorId: string | null; weaponId: string | null };

export const characterBuildKey = (id: string) => [...characterKey(id), "build"] as const;

function result(data: ApiCharacterBuild | undefined, response: Response, error: unknown) {
  if (!response.ok) throw new ApiError(response.status, response.statusText, JSON.stringify(error ?? null));
  if (!data || !data.status
    || !data.selections || !data.abilities || !data.equipmentClaims || !data.ownedItems || !data.abilityInputOptions)
    throw new Error("The Character build response is incomplete or has an invalid revision.");
  if (!["pending", "ready", "failed"].includes(data.status)
    || (data.status === "failed" && !data.failure)
    || (data.status !== "ready" && (data.sheet != null || data.sheetReady))
    || (data.sheetReady && data.sheet == null))
    throw new Error("The Character calculation response has inconsistent status or missing results.");
  return {
    ...data, revision: readRevision(data.revision),
    acceptedRevision: data.acceptedRevision == null ? data.acceptedRevision : readRevision(data.acceptedRevision),
    selections: data.selections.map((selection) => ({ ...selection, count: readInteger(selection.count) })),
    abilityInputOptions: data.abilityInputOptions.map(readInteger),
    abilities: data.abilities.map((ability) => ({
      ...ability,
      input: ability.input == null ? null : readInteger(ability.input),
      score: ability.score == null ? null : readInteger(ability.score),
      modifier: ability.modifier == null ? null : readInteger(ability.modifier),
    })),
    ownedItems: data.ownedItems.map((item) => ({ ...item, quantity: readInteger(item.quantity) })),
    sheet: data.sheet == null ? data.sheet : {
      ...data.sheet, gold: readInteger(data.sheet.gold),
      statistics: data.sheet.statistics.map((statistic) => ({ ...statistic, value: statistic.value == null ? null : readInteger(statistic.value) })),
      attacks: data.sheet.attacks.map((attack) => ({ ...attack, toHit: readInteger(attack.toHit) })),
    },
  };
}

export async function getCharacterBuild(id: string, signal?: AbortSignal) {
  const { data, response, error } = await generatedApi.GET("/api/characters/{characterId}/build", {
    params: { path: { characterId: id } }, signal,
  });
  return result(data, response, error);
}

export async function submitCharacterEdit(id: string, edit: { operationId: string; expectedRevision: number; action: CharacterEdit }) {
  const params = { path: { characterId: id } };
  const common = { operationId: edit.operationId, expectedRevision: edit.expectedRevision };
  const action = edit.action;
  switch (action.kind) {
    case "name": {
      const { data, response, error } = await generatedApi.POST("/api/characters/{characterId}/name", { params, body: { ...common, name: action.name } });
      return result(data, response, error);
    }
    case "choice": {
      const { data, response, error } = await generatedApi.POST("/api/characters/{characterId}/choices", {
        params, body: { ...common, selectionId: action.selectionId, answers: action.answers },
      });
      return result(data, response, error);
    }
    case "abilities": {
      const { data, response, error } = await generatedApi.POST("/api/characters/{characterId}/ability-scores", { params, body: { ...common, inputs: action.inputs } });
      return result(data, response, error);
    }
    case "claim": {
      const { data, response, error } = await generatedApi.POST("/api/characters/{characterId}/equipment-claims", {
        params, body: { ...common, selectionId: action.selectionId },
      });
      return result(data, response, error);
    }
    case "equipment": {
      const { data, response, error } = await generatedApi.POST("/api/characters/{characterId}/equipment", {
        params, body: { ...common, armorId: action.armorId, weaponId: action.weaponId },
      });
      return result(data, response, error);
    }
  }
}
