import "office-ui-fabric-react/dist/css/fabric.min.css";
import { AppContainer } from "react-hot-loader";
import { initializeIcons } from "office-ui-fabric-react/lib/Icons";
import * as React from "react";
import * as ReactDOM from "react-dom";
import { Provider } from "react-redux";

import App from "./App";
import configureStore from "./store/store";
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

/* Render application after Office initializes */
Office.initialize = () => {
  console.log("index.initialize");
  isOfficeInitialized = true;
  render(App);
};

if ((module as any).hot) {
  (module as any).hot.accept("./App", () => {
    console.log("index.hot");
    const NextApp = require("./App").default;
    render(NextApp);
  });
}
