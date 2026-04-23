/**
 * Hierarchical query key factory for the characters domain.
 *
 * The prefix structure enables surgical or broad invalidation:
 *  - invalidate all character data:           characterQueryKeys.all()
 *  - invalidate all detail data for one char: characterQueryKeys.detail(id)
 *  - invalidate only ability scores:          characterQueryKeys.abilityScores(id)
 */
export const characterQueryKeys = {
  all: () => ["characters"] as const,

  // List queries
  list: () => ["characters", "list"] as const,
  creationOptions: () => ["characters", "creation-options"] as const,
  portraitOptions: () => ["characters", "portrait-options"] as const,

  // Single character — parent key for all sub-resources
  details: () => ["characters", "detail"] as const,
  detail: (id: string) => ["characters", "detail", id] as const,

  // Sub-resources — all start with detail(id) so they invalidate together
  abilityScores: (id: string) => ["characters", "detail", id, "ability-scores"] as const,
  savingThrows: (id: string) => ["characters", "detail", id, "saving-throws"] as const,
  skills: (id: string) => ["characters", "detail", id, "skills"] as const,
};
