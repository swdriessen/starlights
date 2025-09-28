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
