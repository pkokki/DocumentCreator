import { Dispatch } from "react";
import {
  DocumentCreatorActionTypes,
  Template,
  Mapping,
  EvaluationRequest,
  EvaluationOutput,
  DocumentCreatorActions
} from "./types";

export function initializeOffice(dispatch: Dispatch<DocumentCreatorActionTypes>): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.INITIALIZE_OFFICE,
    dispatch: dispatch
  };
}

export function activateWorksheet(worksheetId: string): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.ACTIVATE_WORKSHEET,
    worksheetId: worksheetId
  };
}

export function setBaseUrl(url: string): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.SET_BASE_URL,
    url: url
  };
}
export function raiseError(errorMessage: string, isHttp: boolean = false): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.RAISE_ERROR,
    errorMessage: errorMessage,
    isHttp: isHttp
  };
}
export function resetError(): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.RESET_ERROR
  };
}

export function requestTemplates(): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.REQUEST_TEMPLATES
  };
}

export function receiveTemplates(templates: Template[]): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.RECEIVE_TEMPLATES,
    payload: templates
  };
}

export function requestTemplate(name: string, version: string): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.REQUEST_TEMPLATE,
    name: name,
    version: version
  };
}

export function receiveTemplate(template: Template): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.RECEIVE_TEMPLATE,
    payload: template
  };
}

export function requestMappings(): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.REQUEST_MAPPINGS
  };
}

export function receiveMappings(mappings: Mapping[]): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.RECEIVE_MAPPINGS,
    payload: mappings
  };
}

export function selectMapping(mappingName: string): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.SELECT_MAPPING,
    name: mappingName
  };
}

export function uploadTemplateStart(): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.UPLOAD_TEMPLATE
  };
}

export function requestEvaluation(request: EvaluationRequest): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.REQUEST_EVALUATION,
    request: request
  };
}

export function receiveEvaluation(result: EvaluationOutput): DocumentCreatorActionTypes {
  return {
    type: DocumentCreatorActions.RECEIVE_EVALUATION,
    payload: result
  };
}

/**
 * Async action creators
 */
async function httpFetch<T>(
  dispatch: Dispatch<DocumentCreatorActionTypes>,
  url: string,
  init: any,
  reqAction: DocumentCreatorActionTypes,
  recvAction: (json: T) => DocumentCreatorActionTypes
) {
  dispatch(reqAction);
  const response = await fetch(url, init);
  if (response.ok) {
    const json = await response.json();
    if (json.error) {
      dispatch(raiseError(json.error, true));
      return undefined;
    } else {
      dispatch(recvAction(json));
      return <T>json;
    }
  } else {
    var errors = "";
    const json = await response.json();
    if (json.errors) {
      for (const key in json.errors) {
        const error = json.errors[key];
        errors += `: ${error}`;
      }
    }
    dispatch(raiseError(`${response.status} (${response.statusText}) ${json.title} ${errors}`, true));
    return undefined;
  }
}

async function httpFetchGet<T>(
  dispatch: Dispatch<DocumentCreatorActionTypes>,
  url: string,
  reqAction: DocumentCreatorActionTypes,
  recvAction: (json: T) => DocumentCreatorActionTypes
) {
  return httpFetch(dispatch, url, undefined, reqAction, recvAction);
}

async function httpFetchForm<T>(
  dispatch: Dispatch<DocumentCreatorActionTypes>,
  url: string,
  body: any,
  reqAction: DocumentCreatorActionTypes,
  recvAction: (json: T) => DocumentCreatorActionTypes
) {
  return httpFetch(
    dispatch,
    url,
    {
      body: body,
      method: "POST"
    },
    reqAction,
    recvAction
  );
}

async function httpFetchPost<T>(
  dispatch: Dispatch<DocumentCreatorActionTypes>,
  url: string,
  body: {},
  reqAction: DocumentCreatorActionTypes,
  recvAction: (json: T) => DocumentCreatorActionTypes
) {
  return httpFetch(
    dispatch,
    url,
    {
      body: JSON.stringify(body),
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json;charset=UTF-8"
      }
    },
    reqAction,
    recvAction
  );
}

export function fetchTemplates(baseUrl: string) {
  return async function(dispatch: Dispatch<DocumentCreatorActionTypes>) {
    return httpFetchGet(dispatch, `${baseUrl}/templates`, requestTemplates(), receiveTemplates);
  };
}

export function fetchTemplate(baseUrl: string, name: string, version: string) {
  return async function(dispatch: Dispatch<DocumentCreatorActionTypes>) {
    return httpFetchGet(
      dispatch,
      `${baseUrl}/templates/${name}/versions/${version}`,
      requestTemplate(name, version),
      receiveTemplate
    );
  };
}

export function fetchMappings(baseUrl: string, templateName: string) {
  return async function(dispatch: Dispatch<DocumentCreatorActionTypes>) {
    return httpFetchGet(dispatch, `${baseUrl}/templates/${templateName}/mappings`, requestMappings(), receiveMappings);
  };
}

export function uploadTemplate(baseUrl: string, templateName: string, file: File) {
  return async function(dispatch: Dispatch<DocumentCreatorActionTypes>) {
    const formData = new FormData();
    formData.append("name", templateName);
    formData.append("FILE", file, file.name);
    const template = await httpFetchForm(
      dispatch,
      `${baseUrl}/templates`,
      formData,
      uploadTemplateStart(),
      receiveTemplate
    );
    fetchTemplates(baseUrl);
    return template;
  };
}

export function fetchEvaluation(baseUrl: string, evalRequest: EvaluationRequest) {
  return async function(dispatch: Dispatch<DocumentCreatorActionTypes>) {
    return await httpFetchPost(dispatch, `${baseUrl}/evaluations`, evalRequest, requestEvaluation(evalRequest), receiveEvaluation);
  };
}
