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
  numFormatId?: number;
  numFormatCode?: string;
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
  readonly lastEvaluation?: { input: EvaluationRequest, output: EvaluationOutput }
}

/**
 * Action type constants
 */
export enum DocumentCreatorActions {
  INITIALIZE_OFFICE = "INITIALIZE_OFFICE",
  SET_BASE_URL = "SET_BASE_URL",
  RAISE_ERROR = "RAISE_ERROR",
  RESET_ERROR = "RESET_ERROR",
  REQUEST_TEMPLATES = "REQUEST_TEMPLATES",
  RECEIVE_TEMPLATES = "RECEIVE_TEMPLATES",
  REQUEST_TEMPLATE = "REQUEST_TEMPLATE",
  RECEIVE_TEMPLATE = "RECEIVE_TEMPLATE",
  ACTIVATE_WORKSHEET = "ACTIVATE_WORKSHEET",
  REQUEST_MAPPINGS = "REQUEST_MAPPINGS",
  RECEIVE_MAPPINGS = "RECEIVE_MAPPINGS",
  SELECT_MAPPING = "SELECT_MAPPING",
  UPLOAD_TEMPLATE = "UPLOAD_TEMPLATE",
  REQUEST_EVALUATION = "REQUEST_EVALUATION",
  RECEIVE_EVALUATION = "RECEIVE_EVALUATION"
}

/**
 * Action types
 */
interface InitializeOfficeAction {
  type: typeof DocumentCreatorActions.INITIALIZE_OFFICE;
  dispatch: Dispatch<DocumentCreatorActionTypes>;
}
interface SetBaseUrlAction {
  type: typeof DocumentCreatorActions.SET_BASE_URL;
  url: string;
}
interface RaiseErrorAction {
  type: typeof DocumentCreatorActions.RAISE_ERROR;
  errorMessage: string;
  isHttp: boolean;
}
interface ResetErrorAction {
  type: typeof DocumentCreatorActions.RESET_ERROR;
}
interface RequestTemplatesAction {
  type: typeof DocumentCreatorActions.REQUEST_TEMPLATES;
}
interface ReceiveTemplatesAction {
  type: typeof DocumentCreatorActions.RECEIVE_TEMPLATES;
  payload: Template[];
}
interface RequestTemplateAction {
  type: typeof DocumentCreatorActions.REQUEST_TEMPLATE;
  name: string;
  version: string;
}
interface ReceiveTemplateAction {
  type: typeof DocumentCreatorActions.RECEIVE_TEMPLATE;
  payload: Template;
}
interface ActivateWorksheetAction {
  type: typeof DocumentCreatorActions.ACTIVATE_WORKSHEET;
  worksheetId: string;
}
interface RequestMappingsAction {
  type: typeof DocumentCreatorActions.REQUEST_MAPPINGS;
}
interface ReceiveMappingsAction {
  type: typeof DocumentCreatorActions.RECEIVE_MAPPINGS;
  payload: Mapping[];
}
interface SelectMappingAction {
  type: typeof DocumentCreatorActions.SELECT_MAPPING;
  name: string;
}
interface UploadTemplateAction {
  type: typeof DocumentCreatorActions.UPLOAD_TEMPLATE;
}
interface RequestEvaluationAction {
  type: typeof DocumentCreatorActions.REQUEST_EVALUATION;
  request: EvaluationRequest;
}
interface ReceiveEvaluationAction {
  type: typeof DocumentCreatorActions.RECEIVE_EVALUATION;
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
