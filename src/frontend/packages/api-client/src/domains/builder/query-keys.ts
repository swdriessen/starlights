/**
 * Hierarchical query key factory for the builder domain.
 *
 * All keys are prefixed with ["characters", "detail", characterId] so that
 * invalidating characterQueryKeys.detail(id) also clears all builder data
 * for that character in one call.
 */
export const builderQueryKeys = {
  selectionRules: (characterId: string, type: string) => ["characters", "detail", characterId, "builder", "selection-rules", type] as const,

  selectionRuleOptions: (characterId: string, selectionRuleId: string) =>
    ["characters", "detail", characterId, "builder", "selection-rule-options", selectionRuleId] as const,

  registrations: (characterId: string) => ["characters", "detail", characterId, "registrations"] as const,

  classes: (characterId: string) => ["characters", "detail", characterId, "classes"] as const,

  statistics: (characterId: string) => ["characters", "detail", characterId, "statistics"] as const,
};
