// --- Selection rules ---

export type SelectionRuleDataModel = {
  registrationId: string;
  registrationSelectionRuleId: string;
  name: string;
  type: string;
  activeRegistration?: boolean;
};

export type SelectionRuleOptionDataModel = {
  elementId: string;
  name: string;
};

// --- Registrations ---

export type RegistrationModel = {
  registrationId: string;
  name: string;
  type: string;
  associatedElementId: string;
  parentRegistrationId?: string;
  characterId: string;
  children?: RegistrationModel[];
};

// --- Classes ---

export type CharacterClass = {
  characterClassId: string;
  registrationId: string;
  name: string;
  level: number;
  isPrimary: boolean;
};

// --- Statistics ---

export type StatisticValueDataModel = {
  source: string;
  value: number;
  displayName?: string;
  ruleId?: string;
};

export type StatisticGroupDataModel = {
  groupName: string;
  totalValue: number;
  values: StatisticValueDataModel[];
  isFinalized: boolean;
};

// --- Mutations ---

export type RegisterSelectionRequest = {
  parentRegistration: string;
  elementId: string;
};

export type RegisterSelectionResponse = {
  registrationId: string;
};

export type UnregisterSelectionRequest = {
  parentRegistration: string;
  elementId: string;
};

export type UpdateClassLevelRequest = {
  newLevel: number;
};
