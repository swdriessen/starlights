import { ApiError } from "@starlights/api-client";
import { useCallback, useEffect, useReducer, useState } from "react";
import { editReducer, type EditState, type RevisionSnapshot } from "./reliable-edit-state";

export function useReliableEdits<S extends RevisionSnapshot, T>(
  snapshot: S,
  submit: (edit: { expectedRevision: number; operationId: string; action: T }) => Promise<S & { acceptedRevision?: number | null }>,
  refresh: () => Promise<S>,
  onAccepted: () => void,
) {
  const [state, dispatch] = useReducer(editReducer<S, T>, { saved: snapshot, drafts: [], active: null } satisfies EditState<S, T>);
  const [refreshError, setRefreshError] = useState<string | null>(null);

  useEffect(() => { dispatch({ type: "snapshot", snapshot }); }, [snapshot]);

  useEffect(() => {
    if (state.active || state.drafts.length === 0) return;
    const timer = setTimeout(() => dispatch({ type: "start" }), 500);
    return () => clearTimeout(timer);
  }, [state.active, state.drafts]);

  useEffect(() => {
    const edit = state.active;
    if (!edit || edit.phase !== "sending") return;
    let disposed = false;
    void submit(edit).then((result) => {
      if (disposed) return;
      dispatch({ type: "accepted", operationId: edit.operationId, snapshot: result, acceptedRevision: result.acceptedRevision ?? undefined });
      onAccepted();
    }, (error: unknown) => {
      if (disposed) return;
      const rejected = error instanceof ApiError && error.status >= 400 && error.status < 500;
      const conflict = error instanceof ApiError && error.status === 409;
      dispatch({
        type: "failed", operationId: edit.operationId,
        phase: conflict ? "conflict" : rejected ? "rejected" : "unconfirmed",
        message: conflict ? "A newer saved revision conflicts with this edit. Your input has not been discarded."
          : rejected ? "The server rejected this edit. Check your input before reapplying it. Saved facts were not overwritten."
          : "Saving could not be confirmed. The server may already have accepted this operation; retrying will use the same operation identifier.",
      });
    });
    return () => { disposed = true; };
  }, [state.active, submit, onAccepted]);

  const reload = useCallback(async () => {
    try {
      const latest = await refresh();
      dispatch({ type: "snapshot", snapshot: latest });
      setRefreshError(null);
      return latest;
    } catch (error) {
      setRefreshError("The latest saved revision could not be loaded. Your input is still here.");
      throw error;
    }
  }, [refresh]);

  const retry = useCallback(async () => {
    await reload();
    dispatch({ type: "retry" });
  }, [reload]);

  const reapply = useCallback(async () => {
    await reload();
    dispatch({ type: "reapply", operationId: crypto.randomUUID() });
  }, [reload]);

  useEffect(() => {
    const reconnect = () => { void retry().catch(() => { /* reload exposes the failure in the editor. */ }); };
    window.addEventListener("online", reconnect);
    return () => window.removeEventListener("online", reconnect);
  }, [retry]);

  useEffect(() => {
    if (!state.active && state.drafts.length === 0) return;
    const warn = (event: BeforeUnloadEvent) => { event.preventDefault(); };
    window.addEventListener("beforeunload", warn);
    return () => window.removeEventListener("beforeunload", warn);
  }, [state.active, state.drafts.length]);

  return {
    state, refreshError, reload, retry, reapply,
    edit: (key: string, action: T) => dispatch({ type: "edit", key, action, operationId: crypto.randomUUID() }),
    discard: () => dispatch({ type: "discard" }),
  };
}
