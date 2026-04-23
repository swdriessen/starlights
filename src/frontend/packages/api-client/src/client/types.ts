export interface ClientConfig {
  /**
   * Base URL for all requests (e.g. "https://localhost:7246").
   * Paths starting with "http" are used as-is.
   */
  baseUrl: string;
  /**
   * Optional callback returning headers to merge into every request.
   * Use this to inject Authorization tokens or other shared headers.
   */
  getHeaders?: () => Record<string, string>;
}

export interface RequestOptions {
  /** Passed to fetch to support TanStack Query cancellation. */
  signal?: AbortSignal;
  /** Additional per-request headers, merged over ClientConfig.getHeaders(). */
  headers?: Record<string, string>;
  /**
   * When set, the response is considered an error unless the status exactly matches.
   * Useful for mutations that return 201 instead of 200.
   */
  expectedStatus?: number;
}
