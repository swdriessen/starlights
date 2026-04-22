export const platformQueryKeys = {
  all: () => ["platform"] as const,
  status: () => ["platform", "status"] as const,
};
