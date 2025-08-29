const API_BASE = import.meta.env.VITE_API_BASE || "https://localhost:7246";

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
