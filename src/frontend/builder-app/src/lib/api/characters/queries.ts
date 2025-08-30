import { useQuery, type UseQueryResult } from "@tanstack/react-query";
import { fetchJson } from "@/lib/api";

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
