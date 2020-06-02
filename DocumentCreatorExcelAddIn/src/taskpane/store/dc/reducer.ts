import {
  DocumentCreatorState,
  DocumentCreatorActionTypes,
  REQUEST_TEMPLATES,
  RECEIVE_TEMPLATES,
  REQUEST_FAILED,
  SET_BASE_URL,
  REQUEST_TEMPLATE,
  RECEIVE_TEMPLATE
} from "./types";

export const initialState: DocumentCreatorState = {
  baseUrl: "http://localhost:6001/api",
  pending: 0,
  availableTemplates: []
};

export function documentCreatorReducer(state = initialState, action: DocumentCreatorActionTypes): DocumentCreatorState {
  switch (action.type) {
    case SET_BASE_URL:
        return { ...state, baseUrl: action.url };
    case REQUEST_TEMPLATE:
    case REQUEST_TEMPLATES:
      return { ...state, pending: state.pending + 1 };
    case RECEIVE_TEMPLATES:
      return { ...state, pending: state.pending - 1, availableTemplates: action.payload };
    case RECEIVE_TEMPLATE:
        return { ...state, pending: state.pending - 1, activeTemplate: action.payload };
    case REQUEST_FAILED:
      return { ...state, pending: state.pending - 1, error: action.error };
    default: 
      return state;
  }
}
