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

  function mergeHeaders(extra?: Record<string, string>): Record<string, string> {
    return {
      ...(config.getHeaders?.() ?? {}),
      ...(extra ?? {}),
    };
  }

  async function handleResponse<T>(res: Response, expectedStatus?: number): Promise<T> {
    const isOk = expectedStatus != null ? res.status === expectedStatus : res.ok;
    if (!isOk) {
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
    get<T>(path: string, options?: RequestOptions): Promise<T> {
      return fetch(buildUrl(path), {
        method: "GET",
        headers: mergeHeaders(options?.headers),
        signal: options?.signal,
      }).then((res) => handleResponse<T>(res, options?.expectedStatus));
    },

    post<TReq, TRes>(path: string, payload: TReq, options?: RequestOptions): Promise<TRes> {
      return fetch(buildUrl(path), {
        method: "POST",
        headers: { "Content-Type": "application/json", ...mergeHeaders(options?.headers) },
        body: JSON.stringify(payload),
        signal: options?.signal,
      }).then((res) => handleResponse<TRes>(res, options?.expectedStatus));
    },

    put<TReq, TRes>(path: string, payload: TReq, options?: RequestOptions): Promise<TRes> {
      return fetch(buildUrl(path), {
        method: "PUT",
        headers: { "Content-Type": "application/json", ...mergeHeaders(options?.headers) },
        body: JSON.stringify(payload),
        signal: options?.signal,
      }).then((res) => handleResponse<TRes>(res, options?.expectedStatus));
    },

    patch<TReq, TRes>(path: string, payload: TReq, options?: RequestOptions): Promise<TRes> {
      return fetch(buildUrl(path), {
        method: "PATCH",
        headers: { "Content-Type": "application/json", ...mergeHeaders(options?.headers) },
        body: JSON.stringify(payload),
        signal: options?.signal,
      }).then((res) => handleResponse<TRes>(res, options?.expectedStatus));
    },

    delete<T = void>(path: string, options?: RequestOptions): Promise<T> {
      return fetch(buildUrl(path), {
        method: "DELETE",
        headers: mergeHeaders(options?.headers),
        signal: options?.signal,
      }).then((res) => handleResponse<T>(res, options?.expectedStatus));
    },
  };
}
