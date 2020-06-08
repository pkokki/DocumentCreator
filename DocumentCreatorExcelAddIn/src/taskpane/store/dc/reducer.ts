import {
  DocumentCreatorState,
  DocumentCreatorActionTypes,
  DocumentCreatorActions
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
    case DocumentCreatorActions.SET_BASE_URL:
      return { ...state, baseUrl: action.url };
    case DocumentCreatorActions.REQUEST_TEMPLATE:
    case DocumentCreatorActions.REQUEST_TEMPLATES:
    case DocumentCreatorActions.REQUEST_MAPPINGS:
    case DocumentCreatorActions.UPLOAD_TEMPLATE:
      return { ...state, pending: state.pending + 1 };
    case DocumentCreatorActions.RECEIVE_TEMPLATES:
      return { ...state, pending: state.pending - 1, availableTemplates: action.payload || [] };
    case DocumentCreatorActions.RECEIVE_TEMPLATE:
      return {
        ...state,
        pending: state.pending - 1,
        activeTemplate: action.payload,
        activeMappingName: defaultMappingName,
        availableMappings: undefined
      };
    case DocumentCreatorActions.RECEIVE_MAPPINGS:
      return { ...state, pending: state.pending - 1, availableMappings: action.payload || [] };
    case DocumentCreatorActions.RAISE_ERROR:
      return {
        ...state,
        pending: state.pending - (action.isHttp ? 1 : 0),
        errorMessage: action.errorMessage || "An unexpected error occurred."
      };
    case DocumentCreatorActions.REQUEST_EVALUATION:
      return { ...state, pending: state.pending + 1, lastEvaluation: { input: action.request, output: undefined } };
    case DocumentCreatorActions.RECEIVE_EVALUATION:
      return { ...state, pending: state.pending - 1, lastEvaluation: { ...state.lastEvaluation, output: action.payload }  };
      case DocumentCreatorActions.RESET_ERROR:
      return { ...state, errorMessage: undefined };
    case DocumentCreatorActions.ACTIVATE_WORKSHEET:
      return { ...state, activeWorksheetId: action.worksheetId };
    case DocumentCreatorActions.SELECT_MAPPING:
      return { ...state, activeMappingName: action.name };
    case DocumentCreatorActions.INITIALIZE_OFFICE:
      ExcelHelper.initializeExcel(action.dispatch);
      return state;
    default:
      return state;
  }
}
