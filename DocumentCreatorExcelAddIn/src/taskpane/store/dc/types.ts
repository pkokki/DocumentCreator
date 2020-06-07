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

export interface EvaluationRequest {
  templateName: string;
  expressions: EvaluationExpression[];
  sources: EvaluationSource[];
}

export interface EvaluationExpression {
  name: string;
  cell: string;
  expression: string;
  parent: string;
  isCollection: boolean;
  content: string;
}

export interface EvaluationSource {
  name: string;
  cell: string;
  payload: {};
}

export interface EvaluationOutput {
  results: EvaluationResult[],
  total: number;
  errors: number;
}

export interface EvaluationResult {
  name: string;
  value: any;
  text: string;
  error: string;
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
export const UPLOAD_TEMPLATE = "UPLOAD_TEMPLATE";
export const REQUEST_EVALUATION = "REQUEST_EVALUATION";
export const RECEIVE_EVALUATION = "RECEIVE_EVALUATION";
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
interface UploadTemplateAction {
  type: typeof UPLOAD_TEMPLATE;
}
interface RequestEvaluationAction {
  type: typeof REQUEST_EVALUATION;
}
interface ReceiveEvaluationAction {
  type: typeof RECEIVE_EVALUATION;
  payload: EvaluationOutput;
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
  | UploadTemplateAction
  | RequestEvaluationAction
  | ReceiveEvaluationAction
  | ActivateWorksheetAction;
