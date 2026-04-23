import { queryOptions } from "@tanstack/react-query";
import type { HttpClient } from "../../client/http-client.ts";
import type {
  AbilityScores,
  CharacterCards,
  CharacterCreationOptions,
  CharacterDetailsResponse,
  CharacterPortraitOptions,
  SavingThrows,
  Skills,
} from "./types.ts";
import {
  getAbilityScores,
  getCharacterCreationOptions,
  getCharacterDetails,
  getCharacterPortraitOptions,
  getCharacters,
  getSavingThrows,
  getSkills,
} from "./endpoints.ts";
import { characterQueryKeys } from "./query-keys.ts";

/**
 * Factory that returns queryOptions builders for all character read queries.
 * Apps spread these into useQuery and add their own staleTime/refetchOnWindowFocus.
 *
 * @example
 * const opts = characterQueryOptions(apiClient);
 * const { data } = useQuery({ ...opts.list(), staleTime: 5_000 });
 */
export function characterQueryOptions(client: HttpClient) {
  return {
    list: () =>
      queryOptions<CharacterCards, Error>({
        queryKey: characterQueryKeys.list(),
        queryFn: ({ signal }) => getCharacters(client, { signal }),
      }),

    detail: (characterId: string) =>
      queryOptions<CharacterDetailsResponse, Error>({
        queryKey: characterQueryKeys.detail(characterId),
        queryFn: ({ signal }) => getCharacterDetails(client, characterId, { signal }),
      }),

    creationOptions: () =>
      queryOptions<CharacterCreationOptions, Error>({
        queryKey: characterQueryKeys.creationOptions(),
        queryFn: ({ signal }) => getCharacterCreationOptions(client, { signal }),
      }),

    portraitOptions: () =>
      queryOptions<CharacterPortraitOptions, Error>({
        queryKey: characterQueryKeys.portraitOptions(),
        queryFn: ({ signal }) => getCharacterPortraitOptions(client, { signal }),
      }),

    abilityScores: (characterId: string) =>
      queryOptions<AbilityScores, Error>({
        queryKey: characterQueryKeys.abilityScores(characterId),
        queryFn: ({ signal }) => getAbilityScores(client, characterId, { signal }),
      }),

    savingThrows: (characterId: string) =>
      queryOptions<SavingThrows, Error>({
        queryKey: characterQueryKeys.savingThrows(characterId),
        queryFn: ({ signal }) => getSavingThrows(client, characterId, { signal }),
      }),

    skills: (characterId: string) =>
      queryOptions<Skills, Error>({
        queryKey: characterQueryKeys.skills(characterId),
        queryFn: ({ signal }) => getSkills(client, characterId, { signal }),
      }),
  };
}
