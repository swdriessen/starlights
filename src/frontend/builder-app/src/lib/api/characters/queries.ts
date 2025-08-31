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

export type CharacterCard = { characterId: string; name: string; portraitUrl?: string };
export type CharacterCards = { characters: CharacterCard[] };

export function useCharacterCards(): UseQueryResult<CharacterCards, Error> {
  return useQuery<CharacterCards, Error>({
    queryKey: ["character-cards"],
    queryFn: () => fetchJson<CharacterCards>("/api/characters"),
    staleTime: 5_000,
    refetchOnWindowFocus: true,
  });
}
