import React = require("react");
import { connect, ConnectedProps } from "react-redux";
import { Stack, Dropdown, IDropdownOption, IDropdownStyles, IStackTokens, TextField, PrimaryButton } from "office-ui-fabric-react";
import { useEffect, useCallback } from "react";
import { useDropzone } from "react-dropzone";
import { RootState } from "../store/store";
import { fetchTemplates, fetchTemplate, uploadTemplate } from "../store/dc/actions";
import { Template } from "../store/dc/types";
import { ExcelHelper } from "../modules/excel";

const mapState = (state: RootState) => ({
  baseUrl: state.dc.baseUrl,
  availableTemplates: state.dc.availableTemplates,
  activeTemplate: state.dc.activeTemplate
});
const mapDispatch = {
  getTemplates: fetchTemplates,
  selectTemplate: fetchTemplate,
  uploadTemplate: uploadTemplate
};

const connector = connect(mapState, mapDispatch);
type PropsFromRedux = ConnectedProps<typeof connector>;
type Props = PropsFromRedux;

const dropdownStyles: Partial<IDropdownStyles> = { dropdown: { width: 300 } };
const stackTokens: IStackTokens = { childrenGap: 10, padding: 10 };
const templateOptions: IDropdownOption[] = [];

function buildTemplateOptions(templates: Template[]) {
  const items = templates.map(t => {
    return { key: t.templateName, text: t.templateName, data: t };
  });
  templateOptions.splice(0, templateOptions.length, ...items);
}

const TemplateSelector = (props: Props) => {
  useEffect(() => {
    if (props.availableTemplates) {
      buildTemplateOptions(props.availableTemplates);
    } else {
      props.getTemplates(props.baseUrl).then(() => buildTemplateOptions(props.availableTemplates));
    }
  });

  var newTemplateName = "T101";
  // https://react-dropzone.js.org/
  const onDrop = useCallback(acceptedFiles => {
    console.log(acceptedFiles);
    if (acceptedFiles && acceptedFiles.length === 1) {
        const theFile = acceptedFiles[0];
        props.uploadTemplate(props.baseUrl, newTemplateName, theFile);
    }
  }, []);

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    maxSize: 1024 * 1024,
    multiple: false,
    //accept: "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
  });

  return (
    <Stack tokens={stackTokens}>
      <TextField
        label="Create a new template..."
        value={newTemplateName}
        onChange={(_: any, newValue?: string) => newTemplateName = (newValue || "")}
        //styles={textFieldStyles}
        />
      <div {...getRootProps()}>
        <input {...getInputProps()} />
        {isDragActive ? (
          <p>Drop the template here ...</p>
        ) : (
          <p>Drag &amp; drop a DOCX template here, or click to select files</p>
        )}
      </div>

      <Dropdown
        label="... or select an existing template:"
        selectedKey={props.activeTemplate ? props.activeTemplate.templateName : null}
        onChange={(_: any, option?: IDropdownOption) =>
          props.selectTemplate(props.baseUrl, option.data.templateName, option.data.version)
        }
        placeholder="Select an option"
        options={templateOptions}
        styles={dropdownStyles}
      />

      <PrimaryButton
        text="Fill active worksheet"
        onClick={async (_: any) => await ExcelHelper.fillActiveSheet(props.activeTemplate)}
        allowDisabledFocus
        disabled={!props.activeTemplate}
      />
    </Stack>
  );
};

export default connector(TemplateSelector);
