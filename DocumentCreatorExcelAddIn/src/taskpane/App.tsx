import React = require("react");

import Header from "./components/Header";
import AppStatus from "./components/AppStatus";
import Settings from "./components/Settings";
import TemplateSelector from "./components/TemplateSelector";
import MappingSelector from "./components/MappingSelector";
import Commander from "./components/Commander";
import Progress from './components/Progress';

interface AppProps {
  title: string;
  isOfficeInitialized: boolean;
}

const App: React.FC<AppProps> = (props: AppProps) => {
  const { title } = props;
  if (!props.isOfficeInitialized) {
    return (
      <Progress title={title} logo="assets/logo-filled.png" message="Please sideload your addin to see app body." />
    );
  }
  return (
    <div>
      <Header logo="assets/logo-filled.png" title={title} />
      <section className="ms-welcome__main">
        <Settings />
        <TemplateSelector />
        <MappingSelector />
        <Commander />
        <AppStatus />
      </section>
    </div>
  );
};

export default App;
