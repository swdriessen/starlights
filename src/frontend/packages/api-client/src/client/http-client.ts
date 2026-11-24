import { parseApiError } from "./api-error.ts";
import type { ClientConfig, RequestOptions } from "./types.ts";

export interface HttpClient {
  get<T>(path: string, options?: RequestOptions): Promise<T>;
  post<TReq, TRes>(path: string, payload: TReq, options?: RequestOptions): Promise<TRes>;
  put<TReq, TRes>(path: string, payload: TReq, options?: RequestOptions): Promise<TRes>;
  patch<TReq, TRes>(path: string, payload: TReq, options?: RequestOptions): Promise<TRes>;
  delete<T = void>(path: string, options?: RequestOptions): Promise<T>;
}

export function createHttpClient(config: ClientConfig): HttpClient {
  function buildUrl(path: string): string {
    return path.startsWith("http") ? path : `${config.baseUrl}${path}`;
  }

  async function mergeHeaders(method: string, extra?: Record<string, string>): Promise<Record<string, string>> {
    return {
      ...(await config.getHeaders?.(method) ?? {}),
      ...(extra ?? {}),
    };
  }

  async function handleResponse<T>(res: Response, expectedStatus?: number): Promise<T> {
    const isOk = expectedStatus != null ? res.status === expectedStatus : res.ok;
    if (!isOk) {
      if (res.status === 401) config.onUnauthorized?.();
      throw await parseApiError(res);
    }
    // 204 No Content or explicitly empty body
    if (res.status === 204) {
      return undefined as T;
    }
    const contentLength = res.headers.get("content-length");
    if (contentLength === "0") {
      return undefined as T;
    }
    try {
      return (await res.json()) as T;
    } catch {
      // Body was not JSON (e.g. empty 201)
      return undefined as T;
    }
  }

  return {
    async get<T>(path: string, options?: RequestOptions): Promise<T> {
      return fetch(buildUrl(path), {
        method: "GET",
        credentials: "same-origin",
        headers: await mergeHeaders("GET", options?.headers),
        signal: options?.signal,
      }).then((res) => handleResponse<T>(res, options?.expectedStatus));
    },

    async post<TReq, TRes>(path: string, payload: TReq, options?: RequestOptions): Promise<TRes> {
      return fetch(buildUrl(path), {
        method: "POST",
        credentials: "same-origin",
        headers: { "Content-Type": "application/json", ...await mergeHeaders("POST", options?.headers) },
        body: JSON.stringify(payload),
        signal: options?.signal,
      }).then((res) => handleResponse<TRes>(res, options?.expectedStatus));
    },

    async put<TReq, TRes>(path: string, payload: TReq, options?: RequestOptions): Promise<TRes> {
      return fetch(buildUrl(path), {
        method: "PUT",
        credentials: "same-origin",
        headers: { "Content-Type": "application/json", ...await mergeHeaders("PUT", options?.headers) },
        body: JSON.stringify(payload),
        signal: options?.signal,
      }).then((res) => handleResponse<TRes>(res, options?.expectedStatus));
    },

    async patch<TReq, TRes>(path: string, payload: TReq, options?: RequestOptions): Promise<TRes> {
      return fetch(buildUrl(path), {
        method: "PATCH",
        credentials: "same-origin",
        headers: { "Content-Type": "application/json", ...await mergeHeaders("PATCH", options?.headers) },
        body: JSON.stringify(payload),
        signal: options?.signal,
      }).then((res) => handleResponse<TRes>(res, options?.expectedStatus));
    },

    async delete<T = void>(path: string, options?: RequestOptions): Promise<T> {
      return fetch(buildUrl(path), {
        method: "DELETE",
        credentials: "same-origin",
        headers: await mergeHeaders("DELETE", options?.headers),
        signal: options?.signal,
      }).then((res) => handleResponse<T>(res, options?.expectedStatus));
    },
  };
}
