import { useQuery, type UseQueryResult } from "@tanstack/react-query";
import { fetchJson } from "./api";

export type Status = { message: string; timestamp: string };

export function usePlatformStatus(keyExtra?: unknown): UseQueryResult<Status, Error> {
  return useQuery<Status, Error>({
    queryKey: ["platform", "status", keyExtra].filter((x) => x !== undefined),
    queryFn: () => fetchJson<Status>("/api/platform/status"),
    staleTime: 5_000,
    refetchOnWindowFocus: true,
  });
}
