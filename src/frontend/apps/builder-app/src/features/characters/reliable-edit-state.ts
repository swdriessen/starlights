export type RevisionSnapshot = { revision: number; status: string };

export function readInteger(value: unknown): number {
  if (typeof value !== "number" && (typeof value !== "string" || !/^-?\d+$/.test(value)))
    throw new Error("The Character response contains an invalid integer.");
  const integer = Number(value);
  if (!Number.isSafeInteger(integer))
    throw new Error("The Character response contains an integer that cannot be represented safely.");
  return integer;
}

export function readRevision(value: unknown): number {
  const revision = readInteger(value);
  if (revision < 0) throw new Error("The Character revision is invalid.");
  return revision;
}

export type EditDraft<T> = { key: string; operationId: string; expectedRevision: number; action: T };
export type ActiveEdit<T> = EditDraft<T> & {
  phase: "sending" | "unconfirmed" | "conflict" | "rejected";
  attempt: number;
  message?: string;
};
export type EditState<S extends RevisionSnapshot, T> = {
  saved: S;
  drafts: EditDraft<T>[];
  active: ActiveEdit<T> | null;
};
export type EditEvent<S, T> =
  | { type: "edit"; key: string; operationId: string; action: T }
  | { type: "start" }
  | { type: "snapshot"; snapshot: S }
  | { type: "accepted"; operationId: string; snapshot: S; acceptedRevision?: number }
  | { type: "failed"; operationId: string; phase: "unconfirmed" | "conflict" | "rejected"; message: string }
  | { type: "retry" }
  | { type: "reapply"; operationId: string }
  | { type: "discard" };

function newest<S extends RevisionSnapshot>(current: S, incoming: S): S {
  if (incoming.revision < current.revision) return current;
  // A delayed pending response cannot undo a terminal result at the same revision.
  if (incoming.revision === current.revision && current.status !== "pending" && incoming.status === "pending") return current;
  return incoming;
}

export function editReducer<S extends RevisionSnapshot, T>(
  state: EditState<S, T>, event: EditEvent<S, T>,
): EditState<S, T> {
  switch (event.type) {
    case "edit": {
      const previous = state.drafts.find((draft) => draft.key === event.key);
      const draft: EditDraft<T> = {
        key: event.key, operationId: event.operationId, action: event.action,
        expectedRevision: previous?.expectedRevision ?? state.active?.expectedRevision ?? state.saved.revision,
      };
      return { ...state, drafts: [...state.drafts.filter((item) => item.key !== event.key), draft] };
    }
    case "start": {
      if (state.active || state.drafts.length === 0) return state;
      const [draft, ...drafts] = state.drafts;
      return { ...state, drafts, active: { ...draft, phase: "sending", attempt: 0 } };
    }
    case "snapshot":
      return { ...state, saved: newest(state.saved, event.snapshot) };
    case "accepted": {
      if (state.active?.operationId !== event.operationId) return state;
      const base = state.active.expectedRevision;
      return {
        saved: newest(state.saved, event.snapshot),
        active: null,
        drafts: state.drafts.map((draft) => draft.expectedRevision === base
          ? { ...draft, expectedRevision: event.acceptedRevision ?? event.snapshot.revision } : draft),
      };
    }
    case "failed":
      return state.active?.operationId === event.operationId
        ? { ...state, active: { ...state.active, phase: event.phase, message: event.message } } : state;
    case "retry":
      return state.active?.phase === "unconfirmed"
        ? { ...state, active: { ...state.active, phase: "sending", attempt: state.active.attempt + 1 } } : state;
    case "reapply":
      if (!state.active || !["conflict", "rejected"].includes(state.active.phase)) return state;
      return {
        ...state,
        active: null,
        drafts: [
          ...(!state.drafts.some((draft) => draft.key === state.active?.key)
            ? [{ key: state.active.key, action: state.active.action, operationId: event.operationId, expectedRevision: state.saved.revision }] : []),
          ...state.drafts.map((draft) => ({ ...draft, expectedRevision: state.saved.revision })),
        ],
      };
    case "discard":
      // An unconfirmed operation may already be committed; resolve it before discarding intent.
      return state.active?.phase === "unconfirmed" || state.active?.phase === "sending"
        ? state : { ...state, active: null, drafts: [] };
  }
}
