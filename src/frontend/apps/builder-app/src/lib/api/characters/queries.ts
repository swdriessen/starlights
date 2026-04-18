import { useMutation, useQuery, useQueryClient, type UseMutationResult, type UseQueryResult } from "@tanstack/react-query";
import { API_BASE, fetchJson, postJson } from "@/lib/api";

export type CharacterCreationOption = { id: string; name: string; shortDescription: string };
export type CharacterCreationOptions = { options: CharacterCreationOption[] };

export function useCharacterCreationOptions(): UseQueryResult<CharacterCreationOptions, Error> {
  return useQuery<CharacterCreationOptions, Error>({
    queryKey: ["character-creation-options"],
    queryFn: () => fetchJson<CharacterCreationOptions>("/api/characters/creation-options"),
    staleTime: Infinity,
    refetchOnWindowFocus: false,
  });
}

export type CharacterPortraitOption = { url: string; description: string };
export type CharacterPortraitOptions = { portraits: CharacterPortraitOption[] };

export function useCharacterPortraitOptions(): UseQueryResult<CharacterPortraitOptions, Error> {
  return useQuery<CharacterPortraitOptions, Error>({
    queryKey: ["character-portrait-options"],
    queryFn: () => fetchJson<CharacterPortraitOptions>("/api/characters/portrait-options"),
    staleTime: Infinity,
    refetchOnWindowFocus: false,
  });
}

export type CharacterCard = {
  characterId: string;
  name: string;
  portraitUrl?: string;
  level: number;
  build: string;
  isFavorite: boolean;
};
export type CharacterCards = { characters: CharacterCard[] };

export function useCharacterCards(): UseQueryResult<CharacterCards, Error> {
  return useQuery<CharacterCards, Error>({
    queryKey: ["character-cards"],
    queryFn: () => fetchJson<CharacterCards>("/api/characters"),
    staleTime: 5_000,
    refetchOnWindowFocus: true,
  });
}

// get character details by id
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
export function useCharacterDetails(characterId: string): UseQueryResult<CharacterDetailsResponse, Error> {
  return useQuery<CharacterDetailsResponse, Error>({
    queryKey: ["character-details", characterId],
    queryFn: () => fetchJson<CharacterDetailsResponse>(`/api/characters/${characterId}`),
    staleTime: 60_000,
    refetchOnWindowFocus: false,
  });
}

// Creation
export type CreateCharacterRequest = {
  CharacterCreationOptionId: string;
  Name: string;
  PortraitUrl?: string | null;
};

export type CreateCharacterResponse = {
  Id: string;
};

export function useCreateCharacter(): UseMutationResult<CreateCharacterResponse, Error, CreateCharacterRequest> {
  const qc = useQueryClient();
  return useMutation<CreateCharacterResponse, Error, CreateCharacterRequest>({
    mutationFn: async (payload) => {
      const raw = await postJson<CreateCharacterRequest, unknown>("/api/characters", payload, { expectedStatus: 201 });
      const anyRaw = raw as { Id?: string; id?: string } | null | undefined;
      const Id = anyRaw?.Id ?? anyRaw?.id;
      if (!Id) throw new Error("Create response did not include an Id");
      return { Id } satisfies CreateCharacterResponse;
    },
    onSuccess: () => {
      // Invalidate list to refetch with the new character present
      qc.invalidateQueries({ queryKey: ["character-cards"] }).catch(() => {});
    },
  });
}

// Deletion
export function useDeleteCharacter(): UseMutationResult<void, Error, string> {
  const qc = useQueryClient();
  return useMutation<void, Error, string>({
    mutationFn: async (id: string) => {
      const path = `/api/characters/${encodeURIComponent(id)}`;
      const url = path.startsWith("http") ? path : `${API_BASE}${path}`;
      const res = await fetch(url, { method: "DELETE" });
      if (!res.ok) {
        let details = "";
        try {
          details = await res.text();
        } catch {
          // ignore
        }
        const suffix = details ? ` - ${details}` : "";
        throw new Error(`HTTP ${res.status} ${res.statusText}${suffix}`);
      }
      // Most APIs return 204 No Content for DELETE
      return;
    },
    onSuccess: () => {
      // Refresh the character list after deletion
      qc.invalidateQueries({ queryKey: ["character-cards"] }).catch(() => {});
    },
  });
}

// get /{characterId:guid}/ability-scores
export function useAbilityScores(characterId: string): UseQueryResult<AbilityScores, Error> {
  return useQuery<AbilityScores, Error>({
    queryKey: ["ability-scores", characterId],
    queryFn: () => fetchJson<AbilityScores>(`/api/characters/${characterId}/ability-scores`),
    staleTime: 60_000,
    refetchOnWindowFocus: false,
  });
}

export type AbilityScore = {
  id: string; // map from abilityScoreId:guid
  abilityScoreId: string;
  name: string;
  abbreviation: string;
  baseScore: number;
  additionalScore: number;
  calculatedScore: number;
  calculatedModifier: number;
};
export type AbilityScores = { abilityScores: AbilityScore[] };

// post to /{characterId:guid}/ability-scores/{abilityScoreId:guid}/base
// create mutation hook to update base score
export type UpdateBaseAbilityScoreRequest = { abilityScoreId: string; value: number };
export type UpdateBaseAbilityScoreResponse = AbilityScore;

export function useUpdateBaseAbilityScore(characterId: string): UseMutationResult<UpdateBaseAbilityScoreResponse, Error, UpdateBaseAbilityScoreRequest> {
  const qc = useQueryClient();
  return useMutation<UpdateBaseAbilityScoreResponse, Error, UpdateBaseAbilityScoreRequest>({
    mutationFn: async ({ abilityScoreId, value }) => {
      const res = await postJson<{ value: number }, UpdateBaseAbilityScoreResponse>(
        `/api/characters/${encodeURIComponent(characterId)}/ability-scores/${encodeURIComponent(abilityScoreId)}/base`,
        { value }
      );
      return res;
    },
    onSuccess: () => {
      // Invalidate the ability scores query to refetch with the updated data
      qc.invalidateQueries({ queryKey: ["ability-scores", characterId] }).catch(() => {});

      // wait 1s to invlalidate the saving throws and skills, as they might take a bit longer to update
      setTimeout(() => {
        qc.invalidateQueries({ queryKey: ["saving-throws", characterId] }).catch(() => {});
        qc.invalidateQueries({ queryKey: ["skills", characterId] }).catch(() => {});
      }, 1000);
    },
  });
}

// for additional score
export type UpdateAdditionalAbilityScoreRequest = { abilityScoreId: string; value: number };
export type UpdateAdditionalAbilityScoreResponse = AbilityScore;
export function useUpdateAdditionalAbilityScore(
  characterId: string
): UseMutationResult<UpdateAdditionalAbilityScoreResponse, Error, UpdateAdditionalAbilityScoreRequest> {
  const qc = useQueryClient();
  return useMutation<UpdateAdditionalAbilityScoreResponse, Error, UpdateAdditionalAbilityScoreRequest>({
    mutationFn: async ({ abilityScoreId, value }) => {
      const res = await postJson<{ value: number }, UpdateAdditionalAbilityScoreResponse>(
        `/api/characters/${encodeURIComponent(characterId)}/ability-scores/${encodeURIComponent(abilityScoreId)}/additional`,
        { value }
      );
      return res;
    },
    onSuccess: () => {
      // Invalidate the ability scores query to refetch with the updated data
      qc.invalidateQueries({ queryKey: ["ability-scores", characterId] }).catch(() => {});

      // wait 1s to invlalidate the saving throws and skills, as they might take a bit longer to update
      setTimeout(() => {
        qc.invalidateQueries({ queryKey: ["saving-throws", characterId] }).catch(() => {});
        qc.invalidateQueries({ queryKey: ["skills", characterId] }).catch(() => {});
      }, 1000);
    },
  });
}

// get saving throws

export type SavingThrow = {
  id: string; // map from savingThrowId:guid
  savingThrowId: string;
  name: string;
  abilityScoreId: string;
  abilityScoreAbbreviation: string;
  abilityScoreModifier: number;
  calculatedBonus: number;
  additionalBonus: number;
};
export type SavingThrows = { savingThrows: SavingThrow[] };

export function useSavingThrows(characterId: string): UseQueryResult<SavingThrows, Error> {
  return useQuery<SavingThrows, Error>({
    queryKey: ["saving-throws", characterId],
    queryFn: () => fetchJson<SavingThrows>(`/api/characters/${characterId}/saving-throws`),
    staleTime: 60_000,
    refetchOnWindowFocus: true,
  });
}

// get skills

export type Skill = {
  id: string; // map from skillId:guid
  skillId: string;
  name: string;
  abilityScoreId: string;
  abilityScoreAbbreviation: string;
  abilityScoreModifier: number;
  calculatedBonus: number;
  additionalBonus: number;
};
export type Skills = { skills: Skill[] };

export function useSkills(characterId: string): UseQueryResult<Skills, Error> {
  return useQuery<Skills, Error>({
    queryKey: ["skills", characterId],
    queryFn: () => fetchJson<Skills>(`/api/characters/${characterId}/skills`),
    staleTime: 60_000,
    refetchOnWindowFocus: true,
  });
}
