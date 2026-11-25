export type VariantId = "v0" | "v1" | "v2" | "v3" | "v4" | "v5"

export const variants: { id: VariantId; path: string; name: string }[] = [
  { id: "v0", path: "/", name: "Starlit (base)" },
  { id: "v1", path: "/v1", name: "Codex" },
  { id: "v2", path: "/v2", name: "Hearth" },
  { id: "v3", path: "/v3", name: "Moonlit" },
  { id: "v4", path: "/v4", name: "Eclipse" },
  { id: "v5", path: "/v5", name: "Builder (mockup)" },
]
