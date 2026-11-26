import { DefaultHttpClient, HubConnectionBuilder, LogLevel, type HttpRequest } from "@microsoft/signalr";
import { useEffect, useState } from "react";
import { generatedApi } from "@/lib/api-client";

class ProtectedNotificationHttpClient extends DefaultHttpClient {
  override async send(request: HttpRequest) {
    if (!["GET", "HEAD", "OPTIONS"].includes(request.method?.toUpperCase() ?? "")) {
      const { data, response } = await generatedApi.GET("/api/identity/antiforgery");
      if (!response.ok || !data?.headerName || !data.requestToken)
        throw new Error("The notification connection could not obtain its request protection token.");
      request = { ...request, headers: { ...request.headers, [data.headerName]: data.requestToken } };
    }
    const response = await super.send(request);
    if (response.statusCode === 401) window.dispatchEvent(new Event("starlights:session-ended"));
    return response;
  }
}

export function useCharacterNotifications(characterId: string, reconnect: () => Promise<unknown>, changed: () => Promise<unknown>) {
  const [connectionError, setConnectionError] = useState<string | null>(null);
  const [attempt, setAttempt] = useState(0);

  useEffect(() => {
    let disposed = false;
    const logger = {
      log: (level: LogLevel, message: string) => {
        if (disposed) return;
        if (level >= LogLevel.Error) console.error(message);
        else if (level >= LogLevel.Warning) console.warn(message);
      },
    };
    const connection = new HubConnectionBuilder()
      .withUrl("/api/characters/notifications", {
        withCredentials: true,
        httpClient: new ProtectedNotificationHttpClient(logger),
      })
      .withAutomaticReconnect()
      .configureLogging(logger)
      .build();

    const synchronize = (action: () => Promise<unknown>) => {
      void action().catch(() => {
        if (!disposed) setConnectionError("The connection returned, but the latest revision could not be loaded. Retry synchronization.");
      });
    };
    connection.on("CharacterChanged", (notification: unknown) => {
      if (typeof notification !== "object" || notification === null || !("characterId" in notification)
        || !("revision" in notification) || typeof notification.revision !== "number") {
        setConnectionError("A revision notification was invalid. Reload the latest saved revision.");
        return;
      }
      if (notification.characterId === characterId) synchronize(changed);
    });
    connection.onreconnecting(() => {
      if (!disposed) setConnectionError("Live updates are disconnected. Your input stays here; saved state is checked over HTTP.");
    });
    connection.onreconnected(() => {
      if (!disposed) {
        setConnectionError(null);
        synchronize(reconnect);
      }
    });
    connection.onclose(() => {
      if (!disposed) setConnectionError("Live updates are unavailable. Retry the connection; saved state is still checked over HTTP.");
    });
    queueMicrotask(() => {
      if (disposed) return;
      void connection.start().then(() => {
        if (!disposed) {
          setConnectionError(null);
          synchronize(reconnect);
        }
      }, () => {
        if (!disposed) setConnectionError("Live updates could not connect. Your input stays here; retry the connection.");
      });
    });
    return () => {
      disposed = true;
      void connection.stop().catch((error: unknown) => console.error("Character notification connection could not stop.", error));
    };
  }, [characterId, attempt, reconnect, changed]);

  return { connectionError, reconnect: () => setAttempt((value) => value + 1) };
}
