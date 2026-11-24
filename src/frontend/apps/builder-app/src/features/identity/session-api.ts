import { ApiError, type components } from "@starlights/api-client";
import { generatedApi } from "@/lib/api-client";

export type Session = { userId: string; username: string };
export type SignIn = components["schemas"]["SignInRequest"];
export const sessionKey = ["identity", "session"] as const;

export async function getSession(signal?: AbortSignal): Promise<Session | null> {
  const { data, error, response } = await generatedApi.GET("/api/identity/session", { signal });
  if (response.status === 401) return null;
  if (!response.ok) throw new ApiError(response.status, response.statusText, JSON.stringify(error));
  if (!data?.userId || !data.username) throw new Error("The session response is incomplete.");
  return { userId: data.userId, username: data.username };
}

export async function signIn(credentials: SignIn): Promise<Session> {
  const { error, response } = await generatedApi.POST("/api/identity/sign-in", { body: credentials });
  if (!response.ok) throw new ApiError(response.status, response.statusText, JSON.stringify(error));
  const session = await getSession();
  if (!session) throw new Error("The server did not establish a session.");
  return session;
}

export async function signOut(): Promise<void> {
  const { error, response } = await generatedApi.POST("/api/identity/sign-out");
  if (!response.ok && response.status !== 401) throw new ApiError(response.status, response.statusText, JSON.stringify(error));
}
