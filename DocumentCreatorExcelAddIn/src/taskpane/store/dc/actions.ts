import { Dispatch } from "react";
import {
  DocumentCreatorActionTypes,
  Template,
  INITIALIZE_OFFICE,
  REQUEST_FAILED,
  REQUEST_TEMPLATES,
  RECEIVE_TEMPLATES,
  SET_BASE_URL,
  REQUEST_TEMPLATE,
  RECEIVE_TEMPLATE,
  ACTIVATE_WORKSHEET,
  Mapping,
  REQUEST_MAPPINGS,
  RECEIVE_MAPPINGS,
  SELECT_MAPPING
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
export function requestFailed(error: Error): DocumentCreatorActionTypes {
  return {
    type: REQUEST_FAILED,
    error: error
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
export function fetchTemplates(baseUrl: string) {
  return async function(dispatch) {
    dispatch(requestTemplates());
    const response = await fetch(`${baseUrl}/templates`);
    const json = await response.json();
    if (json.error) return dispatch(requestFailed(json.error));
    return dispatch(receiveTemplates(json));
  };
}

export function fetchTemplate(baseUrl: string, name: string, version: string) {
  return async function(dispatch) {
    dispatch(requestTemplate(name, version));
    const response = await fetch(`${baseUrl}/templates/${name}/versions/${version}`);
    const json = await response.json();
    if (json.error) return dispatch(requestFailed(json.error));
    return dispatch(receiveTemplate(json));
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
export function fetchMappings(baseUrl: string, templateName: string) {
  console.log("fetchMappings");
  return async function(dispatch) {
    dispatch(requestMappings());
    const response = await fetch(`${baseUrl}/templates/${templateName}/mappings`);
    const json = await response.json();
    if (json.error) return dispatch(requestFailed(json.error));
    return dispatch(receiveMappings(json));
  };
}
export function selectMapping(mappingName: string): DocumentCreatorActionTypes {
  return {
    type: SELECT_MAPPING,
    name: mappingName
  };
}