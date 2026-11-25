/** Set VITE_BUILDER_URL to the running builder app; until then CTAs jump to the builder section. */
export const BUILDER_URL: string | undefined = import.meta.env.VITE_BUILDER_URL
export const builderHref = BUILDER_URL ?? "#builder"
export const builderCta = BUILDER_URL ? "Open the builder" : "Meet the builder"
export const REPO_URL = "https://github.com/swdriessen/starlights"

export type Status = "building" | "planned" | "exploring" | "daydream"

export const statusLabel: Record<Status, string> = {
  building: "In development",
  planned: "Planned",
  exploring: "Exploring",
  daydream: "Daydream",
}

export const statusHint: Record<Status, string> = {
  building: "Being built right now",
  planned: "Designed, not started",
  exploring: "Checking it is feasible",
  daydream: "Someday, maybe",
}

export const statusColor: Record<Status, string> = {
  building: "var(--l-gold)",
  planned: "var(--l-ink)",
  exploring: "var(--l-cool)",
  daydream: "var(--l-mute)",
}

export type Tile = {
  id: string
  plate: string
  title: string
  tag?: string
  blurb: string
  status: Status
  image: string
  alt: string
  position: string
}

export const tiles: Tile[] = [
  {
    id: "builder",
    plate: "01",
    title: "Character Builder",
    tag: "SRD 5.2.1",
    blurb:
      "Create a hero, level them up, and change your mind without losing work. The rules do the arithmetic; you make the choices.",
    status: "building",
    image: "/images/builder.webp",
    alt: "A hooded ranger with a longbow and lantern stands on a cliff above a misty valley and a ruined tower, under an aurora sky.",
    position: "30% 50%",
  },
  {
    id: "campaigns",
    plate: "02",
    title: "Campaign Ledger",
    blurb:
      "Quests, NPCs and session notes, kept next to the characters who live them.",
    status: "planned",
    image: "/images/campaigns.webp",
    alt: "A candle-lit tower room where a hand-drawn map is spread on a table with adventurer miniatures and a small dragon.",
    position: "50% 55%",
  },
  {
    id: "compendium",
    plate: "03",
    title: "Compendium",
    blurb:
      "Spells, items, beasts and lore. Searchable, and one tap from the builder.",
    status: "planned",
    image: "/images/compendium.webp",
    alt: "A robed scholar in a vast round library as spectral blue wisps lift glowing books into the air.",
    position: "55% 40%",
  },
]

export type Node = {
  id: string
  kind: "Tool" | "Ruleset" | "Someday"
  name: string
  status: Status
  summary: string
  /** Position on the chart, in percent. */
  x: number
  y: number
  /** Which side the label sits on. */
  label: "left" | "right" | "top" | "bottom"
}

export const nodes: Node[] = [
  {
    id: "builder",
    kind: "Tool",
    name: "Character Builder",
    status: "building",
    summary:
      "Where it starts. Create, level and keep characters. The first release targets the 5e SRD 5.2.1: every class, levels 1 to 20.",
    x: 34,
    y: 46,
    label: "bottom",
  },
  {
    id: "campaigns",
    kind: "Tool",
    name: "Campaign Ledger",
    status: "planned",
    summary:
      "Quests, NPCs and sessions, shared with the characters in your party.",
    x: 58,
    y: 16,
    label: "top",
  },
  {
    id: "compendium",
    kind: "Tool",
    name: "Compendium",
    status: "planned",
    summary:
      "Spells, items, beasts and lore, searchable and linked from the builder.",
    x: 14,
    y: 20,
    label: "top",
  },
  {
    id: "homebrew",
    kind: "Tool",
    name: "Homebrew Editor",
    status: "planned",
    summary:
      "Write your own species, classes, spells and items and use them beside official content. Converting old Aurora content is on the wish list.",
    x: 11,
    y: 70,
    label: "bottom",
  },
  {
    id: "dnd5e",
    kind: "Ruleset",
    name: "D&D 5e",
    status: "building",
    summary:
      "The reference ruleset (SRD 5.2.1), and the yardstick every other one is measured against.",
    x: 57,
    y: 58,
    label: "bottom",
  },
  {
    id: "daggerheart",
    kind: "Ruleset",
    name: "Daggerheart",
    status: "exploring",
    summary:
      "A candidate for the second ruleset. Each game lives in its own module, so adding one doesn't disturb the others.",
    x: 83,
    y: 40,
    label: "bottom",
  },
  {
    id: "pathfinder",
    kind: "Ruleset",
    name: "Pathfinder 2e",
    status: "exploring",
    summary:
      "Under consideration, and a good test of how independent those modules really are.",
    x: 80,
    y: 80,
    label: "bottom",
  },
  {
    id: "dnd4e",
    kind: "Ruleset",
    name: "D&D 4e",
    status: "daydream",
    summary:
      "A daydream for the long term. The powers-and-encounters era deserves a builder too.",
    x: 41,
    y: 86,
    label: "top",
  },
  {
    id: "more",
    kind: "Someday",
    name: "And more\u2026",
    status: "daydream",
    summary:
      "Room is left in the sky on purpose. Play-time tools, companions, a marketplace: nothing is decided, and ideas are welcome.",
    x: 90,
    y: 12,
    label: "bottom",
  },
]

/** Pairs of node ids joined by a line (or an aurora ribbon when the far end is only a hope). */
export const links: [string, string][] = [
  ["builder", "compendium"],
  ["builder", "campaigns"],
  ["builder", "homebrew"],
  ["homebrew", "compendium"],
  ["builder", "dnd5e"],
  ["dnd5e", "daggerheart"],
  ["dnd5e", "pathfinder"],
  ["dnd5e", "dnd4e"],
  ["campaigns", "more"],
]
