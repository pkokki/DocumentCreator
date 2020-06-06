import React = require("react");
import { connect, ConnectedProps } from "react-redux";
import { Stack, Dropdown, IDropdownOption, IDropdownStyles, DefaultButton, IStackTokens } from "office-ui-fabric-react";
import { useEffect } from 'react';
import { RootState } from '../store/store';
import { fetchTemplates, fetchTemplate } from '../store/dc/actions';
import { Template } from '../store/dc/types';
import { ExcelHelper } from '../modules/excel';

const mapState = (state: RootState) => ({
    baseUrl: state.dc.baseUrl,
    availableTemplates: state.dc.availableTemplates,
    activeTemplate: state.dc.activeTemplate
});
const mapDispatch = {
    getTemplates: fetchTemplates,
    selectTemplate: fetchTemplate
};

const connector = connect(mapState, mapDispatch);
type PropsFromRedux = ConnectedProps<typeof connector>;
type Props = PropsFromRedux;

const dropdownStyles: Partial<IDropdownStyles> = { dropdown: { width: 300 } };
const stackTokens: IStackTokens = { childrenGap: 10 };
const templateOptions: IDropdownOption[] = [];

function buildTemplateOptions(templates: Template[]) {
    const items = templates
        .map(t => { return { key: t.templateName, text: t.templateName, data: t }; });
    templateOptions.splice(0, templateOptions.length, ...items);
}

const TemplateSelector = (props: Props) => {
    useEffect(() => {
        if (props.availableTemplates) {
            buildTemplateOptions(props.availableTemplates);
        }
        else {
            props.getTemplates(props.baseUrl).then(() => buildTemplateOptions(props.availableTemplates));
        }
    });
    
    return (
        <Stack tokens={stackTokens}>
            <Dropdown
                label="Select template:"
                selectedKey={props.activeTemplate ? props.activeTemplate.templateName : null}
                onChange={(_: any, option?: IDropdownOption) => props.selectTemplate(props.baseUrl, option.data.templateName, option.data.version)}
                placeholder="Select an option"
                options={templateOptions}
                styles={dropdownStyles}
            />
            <DefaultButton 
                text="Fill active worksheet" 
                onClick={async (_: any) => await ExcelHelper.fillActiveSheet(props.activeTemplate)} 
                allowDisabledFocus 
                disabled={!props.activeTemplate} />
        </Stack>
        );
};

export default connector(TemplateSelector);
