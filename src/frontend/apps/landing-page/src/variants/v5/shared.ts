import type { BuildState, Derived } from "./compute"

export type Focus = {
  kind:
    | "species"
    | "background"
    | "feat"
    | "style"
    | "mastery"
    | "ability"
    | "class"
  id: string
} | null

/** Shared input styling. */
export const field =
  "w-full rounded-sm border border-rule bg-plate px-3 py-2.5 text-sm text-ink placeholder:text-mute/70 outline-none transition-colors hover:border-ink/30 focus:border-gold"

/** What every stage view receives from the shell. */
export type StageProps = {
  s: BuildState
  d: Derived
  update: (fn: (prev: BuildState) => BuildState) => void
  /** Preview a thing in the description panel while pointing at it. */
  preview: (focus: Focus) => void
  /** Keep a thing in the description panel after choosing it. */
  pin: (focus: Focus) => void
  go: (stage: import("./data").StageId) => void
}
