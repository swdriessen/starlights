import { useQuery, type UseQueryResult } from "@tanstack/react-query";
import { platformQueryOptions, type Status } from "@starlights/api-client";
import { apiClient } from "./api-client";

export type { Status };

const opts = platformQueryOptions(apiClient);

export function usePlatformStatus(keyExtra?: unknown): UseQueryResult<Status, Error> {
  return useQuery({
    ...opts.status(),
    queryKey: keyExtra !== undefined ? [...opts.status().queryKey, keyExtra] : opts.status().queryKey,
    staleTime: 1_000,
    refetchOnWindowFocus: true,
  });
}
