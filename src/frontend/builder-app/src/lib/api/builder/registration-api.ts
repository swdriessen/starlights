import { useMutation, useQuery, useQueryClient, type UseMutationResult, type UseQueryResult } from "@tanstack/react-query";
import { fetchJson, postJson } from "@/lib/api";
import type { RegistrationModel, SelectionRuleDataModel, SelectionRuleOptionDataModel } from "./types";

// get selection rules
type GetSelectionRulesResponse = { rules: SelectionRuleDataModel[] };

export function useSelectionRuleDataModels(characterId: string, type: string): UseQueryResult<GetSelectionRulesResponse, Error> {
  return useQuery<GetSelectionRulesResponse, Error>({
    queryKey: ["character-selection-rules", characterId, type],
    queryFn: () => fetchJson<GetSelectionRulesResponse>(`/api/characters/${characterId}/builder/selection-rules?type=${type}`),
    staleTime: 0,
    refetchOnWindowFocus: true,
  });
}

// get selection rule options
type GetSelectionRuleOptionsResponse = { options: SelectionRuleOptionDataModel[] };

export function useSelectionRuleOptionModels(characterId: string, selectionRuleId: string): UseQueryResult<GetSelectionRuleOptionsResponse, Error> {
  return useQuery<GetSelectionRuleOptionsResponse, Error>({
    queryKey: ["character-selection-rule-options", characterId, selectionRuleId],
    queryFn: () => fetchJson<GetSelectionRuleOptionsResponse>(`/api/characters/${characterId}/builder/selection-rules/${selectionRuleId}/options`),
    staleTime: 0,
    refetchOnWindowFocus: true,
  });
}

// register selection

export type RegisterSelectionRequest = { parentRegistration: string; elementId: string };
export type RegisterSelectionResponse = { registrationId: string };

export function useRegisterSelectionMutation(
  characterId: string,
  selectionRuleId: string
): UseMutationResult<RegisterSelectionResponse, Error, RegisterSelectionRequest> {
  const queryClient = useQueryClient();

  return useMutation<RegisterSelectionResponse, Error, RegisterSelectionRequest>({
    mutationFn: (payload) =>
      postJson<RegisterSelectionRequest, RegisterSelectionResponse>(
        `/api/characters/${characterId}/builder/selection-rules/${selectionRuleId}/register`,
        payload
      ),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["character-selection-rules", characterId, selectionRuleId] });
      queryClient.invalidateQueries({ queryKey: ["character-registrations", characterId] });
    },
  });
}

// get registrations
export type RegistrationModels = { registrations: RegistrationModel[] };

export function useRegistrationModels(characterId: string): UseQueryResult<RegistrationModels, Error> {
  return useQuery<RegistrationModels, Error>({
    queryKey: ["character-registrations", characterId],
    queryFn: () => fetchJson<RegistrationModels>(`/api/characters/${characterId}/registrations`),
    staleTime: 0,
    refetchOnWindowFocus: true,
  });
}
