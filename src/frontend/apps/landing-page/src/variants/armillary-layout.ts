import type { Layout } from "./sky"

/** An armillary: the builder at the pole, tools around the inner ring, rulesets on the outer. */
export const armillaryLayout: Layout = {
  builder: { x: 50, y: 50, label: "bottom" },
  compendium: { x: 33, y: 33, label: "left" },
  campaigns: { x: 62, y: 28, label: "right" },
  homebrew: { x: 35, y: 69, label: "left" },
  dnd5e: { x: 67, y: 62, label: "right" },
  daggerheart: { x: 83, y: 38, label: "right" },
  pathfinder: { x: 80, y: 80, label: "right" },
  dnd4e: { x: 21, y: 84, label: "right" },
  more: { x: 15, y: 14, label: "right" },
}
