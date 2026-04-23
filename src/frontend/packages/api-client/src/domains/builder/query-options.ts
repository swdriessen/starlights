import { queryOptions } from "@tanstack/react-query";
import type { HttpClient } from "../../client/http-client.ts";
import type {
  CharacterClassesResponse,
  GetSelectionRuleOptionsResponse,
  GetSelectionRulesResponse,
  RegistrationModels,
  StatisticsResponse,
} from "./endpoints.ts";
import { getCharacterClasses, getRegistrations, getSelectionRuleOptions, getSelectionRules, getStatistics } from "./endpoints.ts";
import { builderQueryKeys } from "./query-keys.ts";

/**
 * Factory that returns queryOptions builders for all builder read queries.
 * Apps spread these into useQuery and add their own staleTime/refetchOnWindowFocus.
 *
 * @example
 * const opts = builderQueryOptions(apiClient);
 * const { data } = useQuery({ ...opts.selectionRules(characterId, type), staleTime: 0 });
 */
export function builderQueryOptions(client: HttpClient) {
  return {
    selectionRules: (characterId: string, type: string) =>
      queryOptions<GetSelectionRulesResponse, Error>({
        queryKey: builderQueryKeys.selectionRules(characterId, type),
        queryFn: ({ signal }) => getSelectionRules(client, characterId, type, { signal }),
      }),

    selectionRuleOptions: (characterId: string, selectionRuleId: string) =>
      queryOptions<GetSelectionRuleOptionsResponse, Error>({
        queryKey: builderQueryKeys.selectionRuleOptions(characterId, selectionRuleId),
        queryFn: ({ signal }) => getSelectionRuleOptions(client, characterId, selectionRuleId, { signal }),
      }),

    registrations: (characterId: string) =>
      queryOptions<RegistrationModels, Error>({
        queryKey: builderQueryKeys.registrations(characterId),
        queryFn: ({ signal }) => getRegistrations(client, characterId, { signal }),
      }),

    classes: (characterId: string) =>
      queryOptions<CharacterClassesResponse, Error>({
        queryKey: builderQueryKeys.classes(characterId),
        queryFn: ({ signal }) => getCharacterClasses(client, characterId, { signal }),
      }),

    statistics: (characterId: string) =>
      queryOptions<StatisticsResponse, Error>({
        queryKey: builderQueryKeys.statistics(characterId),
        queryFn: ({ signal }) => getStatistics(client, characterId, { signal }),
      }),
  };
}
