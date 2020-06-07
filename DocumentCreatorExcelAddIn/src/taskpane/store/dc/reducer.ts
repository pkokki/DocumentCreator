import {
  DocumentCreatorState,
  DocumentCreatorActionTypes,
  REQUEST_TEMPLATES,
  RECEIVE_TEMPLATES,
  RAISE_ERROR,
  RESET_ERROR,
  SET_BASE_URL,
  REQUEST_TEMPLATE,
  RECEIVE_TEMPLATE,
  INITIALIZE_OFFICE,
  ACTIVATE_WORKSHEET,
  REQUEST_MAPPINGS,
  RECEIVE_MAPPINGS,
  SELECT_MAPPING,
  UPLOAD_TEMPLATE
} from "./types";
import { ExcelHelper } from "../../modules/excel";

const defaultMappingName = "M01";
export const initialState: DocumentCreatorState = {
  baseUrl: "http://localhost:6001/api",
  pending: 0,
  activeMappingName: defaultMappingName
};

export function documentCreatorReducer(state = initialState, action: DocumentCreatorActionTypes): DocumentCreatorState {
  switch (action.type) {
    case SET_BASE_URL:
      return { ...state, baseUrl: action.url };
    case REQUEST_TEMPLATE:
    case REQUEST_TEMPLATES:
    case REQUEST_MAPPINGS:
    case UPLOAD_TEMPLATE:
      return { ...state, pending: state.pending + 1 };
    case RECEIVE_TEMPLATES:
      return { ...state, pending: state.pending - 1, availableTemplates: action.payload || [] };
    case RECEIVE_TEMPLATE:
      return {
        ...state,
        pending: state.pending - 1,
        activeTemplate: action.payload,
        activeMappingName: defaultMappingName,
        availableMappings: undefined
      };
    case RECEIVE_MAPPINGS:
      return { ...state, pending: state.pending - 1, availableMappings: action.payload || [] };
    case RAISE_ERROR:
      return {
        ...state,
        pending: state.pending - (action.isHttp ? 1 : 0),
        errorMessage: action.errorMessage || "An unexpected error occurred."
      };
    case RESET_ERROR:
      return { ...state, errorMessage: undefined };
    case ACTIVATE_WORKSHEET:
      return { ...state, activeWorksheetId: action.worksheetId };
    case SELECT_MAPPING:
      return { ...state, activeMappingName: action.name };
    case INITIALIZE_OFFICE:
      ExcelHelper.initializeExcel(action.dispatch);
      return state;
    default:
      return state;
  }
}
