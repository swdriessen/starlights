import type { HttpClient } from "../../client/http-client.ts";
import type { RequestOptions } from "../../client/types.ts";
import type { Status } from "./types.ts";

export function getPlatformStatus(client: HttpClient, options?: RequestOptions): Promise<Status> {
  return client.get<Status>("/api/platform/status", options);
}
