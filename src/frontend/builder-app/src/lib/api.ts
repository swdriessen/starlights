export const API_BASE = import.meta.env.VITE_API_BASE || "https://localhost:7246";

export async function fetchJson<T>(path: string, init?: RequestInit): Promise<T> {
  const url = path.startsWith("http") ? path : `${API_BASE}${path}`;
  const res = await fetch(url, init);
  if (!res.ok) {
    let details = "";
    try {
      details = await res.text();
    } catch {
      // ignore
    }
    const suffix = details ? ` - ${details}` : "";
    throw new Error(`HTTP ${res.status} ${res.statusText}${suffix}`);
  }
  return res.json() as Promise<T>;
}

export async function postJson<TRequest, TResponse>(path: string, payload: TRequest, init?: RequestInit & { expectedStatus?: number }): Promise<TResponse> {
  const url = path.startsWith("http") ? path : `${API_BASE}${path}`;
  const { expectedStatus, headers, ...rest } = init ?? {};
  const res = await fetch(url, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      ...(headers ?? {}),
    },
    body: JSON.stringify(payload),
    ...rest,
  });

  if ((expectedStatus != null && res.status !== expectedStatus) || !res.ok) {
    let details = "";
    try {
      details = await res.text();
    } catch {
      // ignore
    }
    const suffix = details ? ` - ${details}` : "";
    throw new Error(`HTTP ${res.status} ${res.statusText}${suffix}`);
  }

  // Some APIs may return empty body on 201; guard it.
  try {
    return (await res.json()) as TResponse;
  } catch {
    return undefined as unknown as TResponse;
  }
}
