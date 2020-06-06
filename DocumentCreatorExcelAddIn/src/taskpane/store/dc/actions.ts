import { Dispatch } from "react";
import {
  DocumentCreatorActionTypes,
  Template,
  INITIALIZE_OFFICE,
  RAISE_ERROR,
  REQUEST_TEMPLATES,
  RECEIVE_TEMPLATES,
  SET_BASE_URL,
  REQUEST_TEMPLATE,
  RECEIVE_TEMPLATE,
  ACTIVATE_WORKSHEET,
  Mapping,
  REQUEST_MAPPINGS,
  RECEIVE_MAPPINGS,
  SELECT_MAPPING,
  RESET_ERROR,
  UPLOAD_TEMPLATE
} from "./types";

export function initializeOffice(dispatch: Dispatch<DocumentCreatorActionTypes>): DocumentCreatorActionTypes {
  return {
    type: INITIALIZE_OFFICE,
    dispatch: dispatch
  };
}

export function activateWorksheet(worksheetId: string): DocumentCreatorActionTypes {
  return {
    type: ACTIVATE_WORKSHEET,
    worksheetId: worksheetId
  };
}

export function setBaseUrl(url: string): DocumentCreatorActionTypes {
  return {
    type: SET_BASE_URL,
    url: url
  };
}
export function raiseError(errorMessage: string, isHttp: boolean = false): DocumentCreatorActionTypes {
  return {
    type: RAISE_ERROR,
    errorMessage: errorMessage,
    isHttp: isHttp
  };
}
export function resetError(): DocumentCreatorActionTypes {
  return {
    type: RESET_ERROR
  };
}

export function requestTemplates(): DocumentCreatorActionTypes {
  return {
    type: REQUEST_TEMPLATES
  };
}

export function receiveTemplates(templates: Template[]): DocumentCreatorActionTypes {
  return {
    type: RECEIVE_TEMPLATES,
    payload: templates
  };
}

export function requestTemplate(name: string, version: string): DocumentCreatorActionTypes {
  return {
    type: REQUEST_TEMPLATE,
    name: name,
    version: version
  };
}

export function receiveTemplate(template: Template): DocumentCreatorActionTypes {
  return {
    type: RECEIVE_TEMPLATE,
    payload: template
  };
}

export function requestMappings(): DocumentCreatorActionTypes {
  return {
    type: REQUEST_MAPPINGS
  };
}

export function receiveMappings(mappings: Mapping[]): DocumentCreatorActionTypes {
  return {
    type: RECEIVE_MAPPINGS,
    payload: mappings
  };
}

export function selectMapping(mappingName: string): DocumentCreatorActionTypes {
  return {
    type: SELECT_MAPPING,
    name: mappingName
  };
}

export function uploadTemplateStart(): DocumentCreatorActionTypes {
  return {
    type: UPLOAD_TEMPLATE
  };
}

/**
 * Async action creators
 */
async function httpFetch<T>(
  dispatch: Dispatch<DocumentCreatorActionTypes>,
  url: string,
  reqAction: DocumentCreatorActionTypes,
  recvAction: (json: T) => DocumentCreatorActionTypes
): Promise<void> {
  dispatch(reqAction);
  const response = await fetch(url);
  if (!response.ok) return dispatch(raiseError(response.statusText, true));
  const json = await response.json();
  if (json.error) return dispatch(raiseError(json.error, true));
  return dispatch(recvAction(json));
}

async function postFormData<T>(
  dispatch: Dispatch<DocumentCreatorActionTypes>,
  url: string,
  formData: FormData,
  reqAction: DocumentCreatorActionTypes,
  recvAction: (json: T) => DocumentCreatorActionTypes
): Promise<void> {
  dispatch(reqAction);
  const response = await fetch(url, {
    body: formData,
    method: "POST"
  });
  if (!response.ok) return dispatch(raiseError(response.statusText, true));
  const json = await response.json();
  if (json.error) return dispatch(raiseError(json.error, true));
  return dispatch(recvAction(json));
}

export function fetchTemplates(baseUrl: string) {
  return async function(dispatch: Dispatch<DocumentCreatorActionTypes>) {
    return httpFetch(dispatch, `${baseUrl}/templates`, requestTemplates(), receiveTemplates);
  };
}

export function fetchTemplate(baseUrl: string, name: string, version: string) {
  return async function(dispatch: Dispatch<DocumentCreatorActionTypes>) {
    return httpFetch(
      dispatch,
      `${baseUrl}/templates/${name}/versions/${version}`,
      requestTemplate(name, version),
      receiveTemplate
    );
  };
}

export function fetchMappings(baseUrl: string, templateName: string) {
  return async function(dispatch: Dispatch<DocumentCreatorActionTypes>) {
    return httpFetch(dispatch, `${baseUrl}/templates/${templateName}/mappings`, requestMappings(), receiveMappings);
  };
}

export function uploadTemplate(baseUrl: string, templateName: string, file: File) {
  return async function(dispatch: Dispatch<DocumentCreatorActionTypes>) {
    const formData = new FormData();
    formData.append("name", templateName);
    formData.append("FILE", file, file.name);
    return postFormData(dispatch, `${baseUrl}/templates`, formData, uploadTemplateStart(), receiveTemplate).then(_ =>
      fetchTemplates(baseUrl)
    );
  };
}
