import { useMutation, useQuery, useQueryClient, type UseMutationResult, type UseQueryResult } from "@tanstack/react-query";
import {
  builderQueryKeys,
  builderQueryOptions,
  characterQueryKeys,
  registerSelection,
  unregisterSelection,
  updateClassLevel,
  type CharacterClass,
  type CharacterClassesResponse,
  type GetSelectionRuleOptionsResponse,
  type GetSelectionRulesResponse,
  type RegisterSelectionRequest,
  type RegisterSelectionResponse,
  type RegistrationModel,
  type RegistrationModels,
  type SelectionRuleDataModel,
  type SelectionRuleOptionDataModel,
  type StatisticGroupDataModel,
  type StatisticsResponse,
  type UnregisterSelectionRequest,
  type UpdateClassLevelRequest,
} from "@starlights/api-client";
import { apiClient } from "@/lib/api-client";

export type {
  CharacterClass,
  CharacterClassesResponse,
  GetSelectionRuleOptionsResponse,
  GetSelectionRulesResponse,
  RegisterSelectionRequest,
  RegisterSelectionResponse,
  RegistrationModel,
  RegistrationModels,
  SelectionRuleDataModel,
  SelectionRuleOptionDataModel,
  StatisticGroupDataModel,
  StatisticsResponse,
  UnregisterSelectionRequest,
  UpdateClassLevelRequest,
};

const opts = builderQueryOptions(apiClient);

export function useSelectionRuleDataModels(characterId: string, type: string): UseQueryResult<GetSelectionRulesResponse, Error> {
  return useQuery({ ...opts.selectionRules(characterId, type), staleTime: 0, refetchOnWindowFocus: true });
}

export function useSelectionRuleOptionModels(characterId: string, selectionRuleId: string): UseQueryResult<GetSelectionRuleOptionsResponse, Error> {
  return useQuery({ ...opts.selectionRuleOptions(characterId, selectionRuleId), staleTime: 0, refetchOnWindowFocus: true });
}

export function useRegisterSelectionMutation(
  characterId: string,
  selectionRuleId: string,
): UseMutationResult<RegisterSelectionResponse, Error, RegisterSelectionRequest> {
  const queryClient = useQueryClient();
  return useMutation<RegisterSelectionResponse, Error, RegisterSelectionRequest>({
    mutationFn: (payload) => registerSelection(apiClient, characterId, selectionRuleId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: builderQueryKeys.selectionRules(characterId, selectionRuleId) });
      queryClient.invalidateQueries({ queryKey: builderQueryKeys.registrations(characterId) });
      queryClient.invalidateQueries({ queryKey: builderQueryKeys.classes(characterId) });
      queryClient.invalidateQueries({ queryKey: characterQueryKeys.detail(characterId) });
    },
  });
}

export function useUnregisterSelectionMutation(characterId: string, selectionRuleId: string): UseMutationResult<void, Error, UnregisterSelectionRequest> {
  const queryClient = useQueryClient();
  return useMutation<void, Error, UnregisterSelectionRequest>({
    mutationFn: (payload) => unregisterSelection(apiClient, characterId, selectionRuleId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: builderQueryKeys.selectionRules(characterId, selectionRuleId) });
      queryClient.invalidateQueries({ queryKey: builderQueryKeys.registrations(characterId) });
      queryClient.invalidateQueries({ queryKey: builderQueryKeys.classes(characterId) });
      queryClient.invalidateQueries({ queryKey: characterQueryKeys.detail(characterId) });
    },
  });
}

export function useRegistrationModels(characterId: string): UseQueryResult<RegistrationModels, Error> {
  return useQuery({ ...opts.registrations(characterId), staleTime: 0, refetchOnWindowFocus: true });
}

export function useStatistics(characterId: string): UseQueryResult<StatisticsResponse, Error> {
  return useQuery({ ...opts.statistics(characterId), staleTime: 0, refetchOnWindowFocus: true });
}

export function useCharacterClasses(characterId: string): UseQueryResult<CharacterClassesResponse, Error> {
  return useQuery({ ...opts.classes(characterId), staleTime: 0, refetchOnWindowFocus: true });
}

export function useUpdateClassLevelMutation(characterId: string, characterClassId: string): UseMutationResult<void, Error, UpdateClassLevelRequest> {
  const queryClient = useQueryClient();
  return useMutation<void, Error, UpdateClassLevelRequest>({
    mutationFn: (payload) => updateClassLevel(apiClient, characterId, characterClassId, payload),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: builderQueryKeys.registrations(characterId) });
      queryClient.invalidateQueries({ queryKey: builderQueryKeys.classes(characterId) });
      queryClient.invalidateQueries({ queryKey: characterQueryKeys.detail(characterId) });
    },
  });
}
