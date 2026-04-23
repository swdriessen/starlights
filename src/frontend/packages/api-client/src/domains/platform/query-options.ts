import { queryOptions } from "@tanstack/react-query";
import type { HttpClient } from "../../client/http-client.ts";
import { getPlatformStatus } from "./endpoints.ts";
import { platformQueryKeys } from "./query-keys.ts";
import type { Status } from "./types.ts";

export function platformQueryOptions(client: HttpClient) {
  return {
    status: () =>
      queryOptions<Status, Error>({
        queryKey: platformQueryKeys.status(),
        queryFn: ({ signal }) => getPlatformStatus(client, { signal }),
      }),
  };
}
