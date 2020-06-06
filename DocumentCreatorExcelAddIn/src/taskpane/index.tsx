import "office-ui-fabric-react/dist/css/fabric.min.css";
import { AppContainer } from "react-hot-loader";
import { initializeIcons } from "office-ui-fabric-react/lib/Icons";
import * as React from "react";
import * as ReactDOM from "react-dom";
import { Provider } from "react-redux";

import App from "./App";
import configureStore from "./store/store";
import { initializeOffice } from './store/dc/actions';
/* global AppContainer, Component, document, Office, module, require */

initializeIcons();

let isOfficeInitialized = false;

const title = "DocumentCreator Task Pane Add-in";
const store = configureStore();

const render = Component => {
  console.log("index.render");
  ReactDOM.render(
    <AppContainer>
      <Provider store={store}>
        <Component title={title} isOfficeInitialized={isOfficeInitialized} />
      </Provider>
    </AppContainer>,
    document.getElementById("container")
  );
};

/*
  "Can it be done with just dispatch arguments?" -> action creator. 
  "Can it be done with previous state and action?" -> reducer 
  "Can it be done from just state?" -> probably a component. 
  So "where does the business logic lives" -> everywhere.
*/

/* Render application after Office initializes */
Office.initialize = () => {
  console.log("index.initialize");
  isOfficeInitialized = true;
  store.dispatch(initializeOffice(store.dispatch));
  render(App);
};

if ((module as any).hot) {
  (module as any).hot.accept("./App", () => {
    console.log("index.hot");
    const NextApp = require("./App").default;
    render(NextApp);
  });
}
