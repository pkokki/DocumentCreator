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
  name: string;
}

/**
 * State
 */
export interface DocumentCreatorState {
  readonly baseUrl: string;
  readonly pending: number;
  readonly error?: Error;
  readonly availableTemplates: Template[];
  readonly activeTemplate?: Template;
}

/**
 * Action type constants
 */
export const SET_BASE_URL = "SET_BASE_URL";
export const REQUEST_FAILED = "REQUEST_FAILED";
export const REQUEST_TEMPLATES = "REQUEST_TEMPLATES";
export const RECEIVE_TEMPLATES = "RECEIVE_TEMPLATES";
export const REQUEST_TEMPLATE = "REQUEST_TEMPLATE";
export const RECEIVE_TEMPLATE = "RECEIVE_TEMPLATE";
/**
 * Action types
 */
interface SetBaseUrlAction {
  type: typeof SET_BASE_URL;
  url: string;
}
interface RequestFailedAction {
  type: typeof REQUEST_FAILED;
  error: Error;
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

export type DocumentCreatorActionTypes =
  | SetBaseUrlAction
  | RequestTemplatesAction
  | ReceiveTemplatesAction
  | RequestTemplateAction
  | ReceiveTemplateAction
  | RequestFailedAction;
