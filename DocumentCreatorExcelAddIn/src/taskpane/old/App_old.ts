// import * as React from "react";
// import { Dropdown, IDropdownStyles, IDropdownOption, PrimaryButton, DefaultButton } from "office-ui-fabric-react";
// import Header from "./Header";
// import Progress from "./Progress";

// /* global console, Excel */

// export interface AppProps {
//   title: string;
//   isOfficeInitialized: boolean;
// }

// export interface Template {
//   templateName: string;
//   version: string;
//   timestamp: Date;
//   size: number;
//   fileName: string;
//   fields: TemplateField[]
// }

// export interface TemplateField {
//   name: string;
//   isCollection: boolean;
//   parent: string;
//   content: string;
// }

// export interface Mapping {
//   name: string;
// }

// export interface AppState {
//   apiBaseUrl: string;
//   error: any;
//   isLoaded: boolean;
//   availableTemplates: Template[];
//   currentTemplateName: string;
// }

// const dropdownStyles: Partial<IDropdownStyles> = { dropdown: { width: 300 } };

// export default class App extends React.Component<AppProps, AppState> {
//   constructor(props: AppProps, context: any) {
//     super(props, context);
//     this.state = {
//       apiBaseUrl: "http://localhost:6001/api",
//       error: null,
//       isLoaded: false,
//       availableTemplates: [],
//       currentTemplateName: null
//     };
//     console.log("constructor", this.state);
//   }

//   async apiGet<T>(url: string): Promise<T> {
//     this.setState({ isLoaded: false });
//     try {
//       const res = await fetch(`${this.state.apiBaseUrl}/${url}`);
//       const json = await res.json();
//       this.setState({ isLoaded: true });
//       console.log("apiGet", url, this.state);
//       return json;
//     }
//     catch (error) {
//       console.error("apiGet", url, error);
//       this.setState({ isLoaded: true });
//       return null;
//     }
//   }

//   componentDidMount() {
//     this.apiGet<Template[]>("templates").then(json => {
//       this.setState({ availableTemplates: json });
//     });
//   }

//   onExcelError = (err: any) => {
//     console.error(err);
//   };
  
//   onSelectedTemplateChanged = async (_event: React.FormEvent<HTMLDivElement>, option?: IDropdownOption, _index?: number) => {
//     const currentTemplateName = option.key.toString();
//     this.setState({ currentTemplateName: currentTemplateName });
//     console.log("onSelectedTemplateChanged", option, this.state);

//     await Excel.run(async context => {
//       console.log("onSelectedTemplateChanged 1");
//       var sheets = context.workbook.worksheets;
//       sheets.load("items/name");
//       await context.sync();

//       console.log("onSelectedTemplateChanged 2", sheets);
//       let sheet = sheets.items.find(v => v.name === currentTemplateName);
//       if (sheet === undefined) {
//         sheet = sheets.add(currentTemplateName);
//         await context.sync();
//       }
//       sheet.activate();
//       return context.sync();
//     }).catch(this.onExcelError);
//   };
  
//   fillTemplateFields = async () => {
//     console.log("fillTemplateFields", this.state);
//     const template = this.state.availableTemplates.find(t => t.templateName === this.state.currentTemplateName);
//     if (template) {
//       const response = await this.apiGet<Template>(`templates/${template.templateName}/versions/${template.version}`);
//       if (response && response.fields && response.fields.length) {
//         const cellData = [];
//         response.fields.map(field => cellData.push([field.name, field.parent, field.isCollection, field.content]));

//         await Excel.run(async context => {
//           const sheet = context.workbook.worksheets.getActiveWorksheet();

//           const range1 = sheet.getRange("A1:N2");
//           range1.values = [
//             ["FIELDS", "", "", "", "", "MAPPING", "", "", "", "", "", "", "SOURCES", ""],
//             ["Name", "ParentId", "IsColl", "Content", "", "Value", "Expected", "Comment", "Expression", "API result", "Check", "", "Name", "Value"]
//           ];
//           range1.format.autofitColumns();

//           const range2 = sheet.getRange("A3:D" + (cellData.length + 2));
//           range2.values = cellData;
//           range2.format.autofitColumns();
//           return context.sync();
//         }).catch(this.onExcelError);
//       }
//     }
//   };

//   testTemplate = async () => {
//     console.log("testTemplate", this.state);
//   };

//   render() {
//     console.log("render", this.state);

//     const { title, isOfficeInitialized } = this.props;
//     const { currentTemplateName, availableTemplates } = this.state;
//     const templateOptions = [];
//     availableTemplates.map(v => templateOptions.push({ key: v.templateName, text: `${v.templateName}` }));

//     if (!isOfficeInitialized) {
//       return (
//         <Progress title={title} logo="assets/logo-filled.png" message="Please sideload your addin to see app body." />
//       );
//     }

//     return (
//       <div className="ms-welcome">
//         <Header logo="assets/logo-filled.png" title={this.props.title} isLoaded={this.state.isLoaded} />
//         <div className="ms-welcome__main">
//           <Dropdown
//             label="Select template:"
//             selectedKey={currentTemplateName}
//             onChange={this.onSelectedTemplateChanged}
//             placeholder="Select an option"
//             options={templateOptions}
//             styles={dropdownStyles}
//           />
//           <Dropdown
//             label="Select existing mapping:"
//             selectedKey={currentMappingName}
//             onChange={this.onSelectedMappingChanged}
//             placeholder="Select an option"
//             options={mappingOptions}
//             styles={dropdownStyles}
//           />
//           <div className="ms-welcome__main">
//             <DefaultButton 
//               text={"Fill template fields"} 
//               onClick={this.fillTemplateFields} 
//               allowDisabledFocus 
//               disabled={currentTemplateName == null} />
//           </div>
//           <div className="ms-welcome__main">
//             <PrimaryButton 
//               text={"Test " + currentTemplateName} 
//               onClick={this.testTemplate} 
//               allowDisabledFocus 
//               disabled={currentTemplateName == null} />
//           </div>
//         </div>
//       </div>
//     );
//   }
// }
