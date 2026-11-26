import assert from "node:assert/strict";
import { test } from "node:test";
import { editReducer, readRevision } from "./src/features/characters/reliable-edit-state.ts";

const initial = () => ({ saved: { revision: 3, status: "ready", name: "Mira" }, drafts: [], active: null });
const edit = (state, operationId, name) => editReducer(state, { type: "edit", key: "name", operationId, action: { name } });
const start = (state) => editReducer(state, { type: "start" });

test("OpenAPI integer/string revisions normalize without accepting precision loss or malformed values", () => {
  assert.equal(readRevision("123"), 123);
  assert.equal(readRevision(123), 123);
  for (const value of ["9007199254740993", 9007199254740992, "", null, -1, 1.5, "1e3"])
    assert.throws(() => readRevision(value));
});

test("out-of-order reads and pending responses cannot replace newer or completed results", () => {
  const state = initial();
  assert.deepEqual(editReducer(state, { type: "snapshot", snapshot: { revision: 2, status: "ready", name: "Old" } }), state);
  assert.deepEqual(editReducer(state, { type: "snapshot", snapshot: { revision: 3, status: "pending", name: "Mira" } }), state);
});

test("save acknowledgment preserves typing made after submission and advances only our queued edits", () => {
  let state = start(edit(initial(), "first", "Mira I"));
  state = edit(state, "second", "Mira II");
  state = editReducer(state, { type: "accepted", operationId: "first", snapshot: { revision: 4, status: "pending", name: "Mira I" } });
  assert.equal(state.saved.name, "Mira I");
  assert.deepEqual(state.drafts, [{ key: "name", operationId: "second", expectedRevision: 4, action: { name: "Mira II" } }]);
});

test("reconnection keeps the original revision and id even after another tab saved", () => {
  let state = start(edit(initial(), "uncertain", "Mine"));
  state = editReducer(state, { type: "failed", operationId: "uncertain", phase: "unconfirmed", message: "Disconnected" });
  state = editReducer(state, { type: "snapshot", snapshot: { revision: 4, status: "ready", name: "Other tab" } });
  state = editReducer(state, { type: "retry" });
  assert.equal(state.active.operationId, "uncertain");
  assert.equal(state.active.expectedRevision, 3);
  assert.equal(state.active.action.name, "Mine");
  assert.equal(state.active.attempt, 1);
  assert.equal(state.saved.name, "Other tab");
});

test("remote notifications do not rebase drafts and conflict resolution requires an explicit reapply", () => {
  let state = edit(initial(), "first", "Mine");
  state = editReducer(state, { type: "snapshot", snapshot: { revision: 4, status: "ready", name: "Other" } });
  state = start(state);
  assert.equal(state.active.expectedRevision, 3);
  state = editReducer(state, { type: "failed", operationId: "first", phase: "conflict", message: "Conflict" });
  state = editReducer(state, { type: "reapply", operationId: "new-operation" });
  assert.equal(state.drafts[0].expectedRevision, 4);
  assert.equal(state.drafts[0].operationId, "new-operation");
  assert.equal(state.drafts[0].action.name, "Mine");
});

test("later typing wins over a conflicted input only when the user explicitly reapplies", () => {
  let state = start(edit(initial(), "first", "First"));
  state = edit(state, "second", "Latest");
  state = editReducer(state, { type: "failed", operationId: "first", phase: "conflict", message: "Conflict" });
  state = editReducer(state, { type: "reapply", operationId: "reapply" });
  assert.equal(state.drafts.length, 1);
  assert.equal(state.drafts[0].action.name, "Latest");
});

test("an old mutation response cannot acknowledge a different in-flight edit", () => {
  const state = start(edit(initial(), "current", "Current"));
  assert.deepEqual(editReducer(state, { type: "accepted", operationId: "old", snapshot: { revision: 10, status: "ready", name: "Old" } }), state);
});

test("unconfirmed saves cannot be silently discarded, and own acknowledgments do not hide a newer remote revision", () => {
  let state = start(edit(initial(), "first", "Mine"));
  state = editReducer(state, { type: "failed", operationId: "first", phase: "unconfirmed", message: "Unknown" });
  assert.deepEqual(editReducer(state, { type: "discard" }), state);
  state = editReducer(state, { type: "snapshot", snapshot: { revision: 5, status: "ready", name: "Newer" } });
  state = editReducer(state, { type: "accepted", operationId: "first", snapshot: { revision: 4, status: "ready", name: "Mine" } });
  assert.equal(state.saved.name, "Newer");
  assert.equal(state.saved.revision, 5);
});

test("an idempotent replay does not rebase queued input onto another tab's intervening revision", () => {
  let state = start(edit(initial(), "first", "Mine"));
  state = edit(state, "second", "My next edit");
  state = editReducer(state, {
    type: "accepted", operationId: "first", acceptedRevision: 4,
    snapshot: { revision: 5, status: "ready", name: "Other tab" },
  });
  assert.equal(state.saved.name, "Other tab");
  assert.equal(state.drafts[0].expectedRevision, 4);
});
