// types for selection rules and their options

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

export type RegistrationModel = {
  registrationId: string;
  name: string;
  type: string;
  associatedElementId: string;
  parentRegistrationId?: string;
  characterId: string;
  children?: RegistrationModel[];
};

export type CharacterClass = {
  characterClassId: string;
  registrationId: string;
  name: string;
  level: number;
  isPrimary: boolean;
};

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
