import { createStore, applyMiddleware, combineReducers } from "redux";
import thunkMiddleware from 'redux-thunk';
import { createLogger } from 'redux-logger';
import { composeWithDevTools } from "redux-devtools-extension";
import { documentCreatorReducer } from "./dc/reducer";

const rootReducer = combineReducers({
  dc: documentCreatorReducer
});

export type RootState = ReturnType<typeof rootReducer>;

const loggerMiddleware = createLogger();

export default function configureStore() {
  const middlewares = [
    thunkMiddleware, // lets us dispatch() functions
    loggerMiddleware // neat middleware that logs actions
  ];
  const middleWareEnhancer = applyMiddleware(...middlewares);

  const store = createStore(
    rootReducer,
    composeWithDevTools(middleWareEnhancer)
  );
  return store;
}

