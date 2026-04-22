import { useMutation, useQuery, useQueryClient, type UseMutationResult, type UseQueryResult } from "@tanstack/react-query";
import {
  characterQueryKeys,
  characterQueryOptions,
  createCharacter,
  deleteCharacter,
  updateAdditionalAbilityScore,
  updateBaseAbilityScore,
  type AbilityScore,
  type AbilityScores,
  type CharacterCard,
  type CharacterCards,
  type CharacterCreationOption,
  type CharacterCreationOptions,
  type CharacterDetails,
  type CharacterDetailsResponse,
  type CharacterPortraitOption,
  type CharacterPortraitOptions,
  type CreateCharacterRequest,
  type CreateCharacterResponse,
  type SavingThrow,
  type SavingThrows,
  type Skill,
  type Skills,
  type UpdateAbilityScoreRequest,
  type UpdateAbilityScoreResponse,
} from "@starlights/api-client";
import { apiClient } from "@/lib/api-client";

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
};

const opts = characterQueryOptions(apiClient);

export function useCharacterCreationOptions(): UseQueryResult<CharacterCreationOptions, Error> {
  return useQuery({ ...opts.creationOptions(), staleTime: Infinity, refetchOnWindowFocus: false });
}

export function useCharacterPortraitOptions(): UseQueryResult<CharacterPortraitOptions, Error> {
  return useQuery({ ...opts.portraitOptions(), staleTime: Infinity, refetchOnWindowFocus: false });
}

export function useCharacterCards(): UseQueryResult<CharacterCards, Error> {
  return useQuery({ ...opts.list(), staleTime: 5_000, refetchOnWindowFocus: true });
}

export function useCharacterDetails(characterId: string): UseQueryResult<CharacterDetailsResponse, Error> {
  return useQuery({ ...opts.detail(characterId), staleTime: 60_000, refetchOnWindowFocus: false });
}

export function useAbilityScores(characterId: string): UseQueryResult<AbilityScores, Error> {
  return useQuery({ ...opts.abilityScores(characterId), staleTime: 60_000, refetchOnWindowFocus: false });
}

export function useSavingThrows(characterId: string): UseQueryResult<SavingThrows, Error> {
  return useQuery({ ...opts.savingThrows(characterId), staleTime: 60_000, refetchOnWindowFocus: true });
}

export function useSkills(characterId: string): UseQueryResult<Skills, Error> {
  return useQuery({ ...opts.skills(characterId), staleTime: 60_000, refetchOnWindowFocus: true });
}

export function useCreateCharacter(): UseMutationResult<CreateCharacterResponse, Error, CreateCharacterRequest> {
  const qc = useQueryClient();
  return useMutation<CreateCharacterResponse, Error, CreateCharacterRequest>({
    mutationFn: (payload) => createCharacter(apiClient, payload),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: characterQueryKeys.list() }).catch(() => {});
    },
  });
}

export function useDeleteCharacter(): UseMutationResult<void, Error, string> {
  const qc = useQueryClient();
  return useMutation<void, Error, string>({
    mutationFn: (id) => deleteCharacter(apiClient, id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: characterQueryKeys.list() }).catch(() => {});
    },
  });
}

export function useUpdateBaseAbilityScore(characterId: string): UseMutationResult<UpdateAbilityScoreResponse, Error, UpdateAbilityScoreRequest> {
  const qc = useQueryClient();
  return useMutation<UpdateAbilityScoreResponse, Error, UpdateAbilityScoreRequest>({
    mutationFn: ({ abilityScoreId, value }) => updateBaseAbilityScore(apiClient, characterId, abilityScoreId, value),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: characterQueryKeys.abilityScores(characterId) }).catch(() => {});
      setTimeout(() => {
        qc.invalidateQueries({ queryKey: characterQueryKeys.savingThrows(characterId) }).catch(() => {});
        qc.invalidateQueries({ queryKey: characterQueryKeys.skills(characterId) }).catch(() => {});
      }, 1000);
    },
  });
}

export function useUpdateAdditionalAbilityScore(characterId: string): UseMutationResult<UpdateAbilityScoreResponse, Error, UpdateAbilityScoreRequest> {
  const qc = useQueryClient();
  return useMutation<UpdateAbilityScoreResponse, Error, UpdateAbilityScoreRequest>({
    mutationFn: ({ abilityScoreId, value }) => updateAdditionalAbilityScore(apiClient, characterId, abilityScoreId, value),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: characterQueryKeys.abilityScores(characterId) }).catch(() => {});
      setTimeout(() => {
        qc.invalidateQueries({ queryKey: characterQueryKeys.savingThrows(characterId) }).catch(() => {});
        qc.invalidateQueries({ queryKey: characterQueryKeys.skills(characterId) }).catch(() => {});
      }, 1000);
    },
  });
}
