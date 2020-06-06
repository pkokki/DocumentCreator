import React = require("react");

import Header from "./components/Header";
import AppStatus from "./components/AppStatus";
import Settings from "./components/Settings";
import TemplateSelector from "./components/TemplateSelector";
import MappingSelector from "./components/MappingSelector";
import Progress from "./components/Progress";
import { Pivot, PivotItem } from 'office-ui-fabric-react';

interface AppProps {
  title: string;
  isOfficeInitialized: boolean;
}

const App: React.FC<AppProps> = (props: AppProps) => {
  const { title } = props;
  if (!props.isOfficeInitialized) {
    return (
      <Progress title={title} logo="assets/logo-filled.png" message="Please sideload the addin to see app body." />
    );
  }
  return (
    <div>
      <Header logo="assets/logo-filled.png" title={title} />
      <AppStatus />
      <Pivot>
        <PivotItem headerText="Templates">
          <TemplateSelector />
        </PivotItem>
        <PivotItem headerText="Mappings">
          <MappingSelector />
        </PivotItem>
        <PivotItem headerText="Settings">
          <Settings />
        </PivotItem>
      </Pivot>
    </div>
  );
};

export default App;
