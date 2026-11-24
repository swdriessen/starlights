import createClient from "openapi-fetch";
import type { paths } from "../generated/schema";
import { ApiError } from "./api-error";

/** Creates the OpenAPI-typed client with same-origin cookies and antiforgery on writes. */
export function createApiClient(baseUrl = "", onUnauthorized?: () => void) {
  const client = createClient<paths>({ baseUrl, credentials: "same-origin", cache: "no-store" });
  client.use({
    async onRequest({ request }) {
      if (!["GET", "HEAD", "OPTIONS"].includes(request.method)) {
        const { data, error, response } = await client.GET("/api/identity/antiforgery");
        if (!response.ok || !data?.headerName || !data.requestToken) {
          throw new ApiError(response.status, response.statusText, JSON.stringify(error ?? "Missing antiforgery token"));
        }
        request.headers.set(data.headerName, data.requestToken);
      }
      return request;
    },
    onResponse({ response }) {
      if (response.status === 401) onUnauthorized?.();
    },
  });
  return client;
}
