import { Dispatch } from 'react';

/**
 * Domain types
 */
export interface Template {
  templateName: string;
  version: string;
  timestamp: Date;
  size: number;
  fileName: string;
  fields: TemplateField[];
}
export interface TemplateField {
  name: string;
  isCollection: boolean;
  parent: string;
  content: string;
}

export interface Mapping {
  mappingName: string;
  mappingVersion: string;
  templateName: string;
  templateVersion: string;
  timestamp: Date;
  size: number;
  fileName: string;
}

/**
 * State
 */
export interface DocumentCreatorState {
  readonly baseUrl: string;
  readonly pending: number;
  readonly errorMessage?: string;
  readonly availableTemplates?: Template[];
  readonly availableMappings?: Mapping[];
  readonly activeTemplate?: Template;
  readonly activeMappingName?: string;
  readonly activeWorksheetId?: string;
}

/**
 * Action type constants
 */
export const INITIALIZE_OFFICE = "INITIALIZE_OFFICE";
export const SET_BASE_URL = "SET_BASE_URL";
export const RAISE_ERROR = "RAISE_ERROR";
export const RESET_ERROR = "RESET_ERROR";
export const REQUEST_TEMPLATES = "REQUEST_TEMPLATES";
export const RECEIVE_TEMPLATES = "RECEIVE_TEMPLATES";
export const REQUEST_TEMPLATE = "REQUEST_TEMPLATE";
export const RECEIVE_TEMPLATE = "RECEIVE_TEMPLATE";
export const ACTIVATE_WORKSHEET = "ACTIVATE_WORKSHEET";
export const REQUEST_MAPPINGS = "REQUEST_MAPPINGS";
export const RECEIVE_MAPPINGS = "RECEIVE_MAPPINGS";
export const SELECT_MAPPING = "SELECT_MAPPING";
/**
 * Action types
 */
interface InitializeOfficeAction {
  type: typeof INITIALIZE_OFFICE;
  dispatch: Dispatch<DocumentCreatorActionTypes>;
}
interface SetBaseUrlAction {
  type: typeof SET_BASE_URL;
  url: string;
}
interface RaiseErrorAction {
  type: typeof RAISE_ERROR;
  errorMessage: string;
  isHttp: boolean;
}
interface ResetErrorAction {
  type: typeof RESET_ERROR;
}
interface RequestTemplatesAction {
  type: typeof REQUEST_TEMPLATES;
}
interface ReceiveTemplatesAction {
  type: typeof RECEIVE_TEMPLATES;
  payload: Template[];
}
interface RequestTemplateAction {
  type: typeof REQUEST_TEMPLATE;
  name: string;
  version: string;
}
interface ReceiveTemplateAction {
  type: typeof RECEIVE_TEMPLATE;
  payload: Template;
}
interface ActivateWorksheetAction {
  type: typeof ACTIVATE_WORKSHEET;
  worksheetId: string;
}
interface RequestMappingsAction {
  type: typeof REQUEST_MAPPINGS;
}
interface ReceiveMappingsAction {
  type: typeof RECEIVE_MAPPINGS;
  payload: Mapping[];
}
interface SelectMappingAction {
  type: typeof SELECT_MAPPING;
  name: string;
}

export type DocumentCreatorActionTypes =
  | InitializeOfficeAction
  | SetBaseUrlAction
  | RequestTemplatesAction
  | ReceiveTemplatesAction
  | RequestTemplateAction
  | ReceiveTemplateAction
  | RaiseErrorAction
  | ResetErrorAction
  | RequestMappingsAction
  | ReceiveMappingsAction
  | SelectMappingAction
  | ActivateWorksheetAction;
