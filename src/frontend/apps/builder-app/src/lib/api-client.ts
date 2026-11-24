import { ApiError, createApiClient, createHttpClient } from "@starlights/api-client";

/**
 * Singleton HTTP client instance for builder-app.
 *
 * Browser requests stay on the UI origin; Vite proxies /api during development.
 */
function onUnauthorized() {
  window.dispatchEvent(new Event("starlights:session-ended"));
}

export const generatedApi = createApiClient("", onUnauthorized);

export const apiClient = createHttpClient({
  baseUrl: "",
  onUnauthorized,
  getHeaders: async (method) => {
    if (["GET", "HEAD", "OPTIONS"].includes(method)) return {};
    const { data, error, response } = await generatedApi.GET("/api/identity/antiforgery");
    if (!response.ok || !data?.headerName || !data.requestToken) {
      throw new ApiError(response.status, response.statusText, JSON.stringify(error ?? "Missing antiforgery token"));
    }
    return { [data.headerName]: data.requestToken };
  },
});
