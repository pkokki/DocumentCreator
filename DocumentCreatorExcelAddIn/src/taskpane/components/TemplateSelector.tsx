import React = require("react");
import { connect, ConnectedProps } from "react-redux";
import { Stack, Dropdown, IDropdownOption, IDropdownStyles } from "office-ui-fabric-react";
import { useEffect } from 'react';
import { RootState } from '../store/store';
import { fetchTemplates, fetchTemplate } from '../store/dc/actions';
import { Template } from '../store/dc/types';

const mapState = (state: RootState) => ({
    baseUrl: state.dc.baseUrl,
    currentTemplate: state.dc.activeTemplate,
    availableTemplates: state.dc.availableTemplates
});
const mapDispatch = {
    getTemplates: fetchTemplates,
    selectTemplate: fetchTemplate
};

const connector = connect(mapState, mapDispatch);
type PropsFromRedux = ConnectedProps<typeof connector>;
type Props = PropsFromRedux;

const dropdownStyles: Partial<IDropdownStyles> = { dropdown: { width: 300 } };
const templateOptions: IDropdownOption[] = [];

function buildTemplateOptions(templates: Template[]) {
    const items = templates
        .map(t => { return { key: t.templateName, text: t.templateName, data: t }; });
    templateOptions.splice(0, templateOptions.length, ...items);
}

const TemplateSelector = (props: Props) => {
    useEffect(() => {
        if (templateOptions.length == 0) {
            if (props.availableTemplates && props.availableTemplates.length) {
                buildTemplateOptions(props.availableTemplates);
            }
            else {
                props.getTemplates(props.baseUrl).then(() => buildTemplateOptions(props.availableTemplates));
            }
        }
    });
    
    return (
        <Stack>
            <Dropdown
            label="Select template:"
            selectedKey={props.currentTemplate ? props.currentTemplate.templateName : null}
            onChange={(_: any, option?: IDropdownOption) => props.selectTemplate(props.baseUrl, option.data.templateName, option.data.version)}
            placeholder="Select an option"
            options={templateOptions}
            styles={dropdownStyles}
            />
        </Stack>
        );
};

export default connector(TemplateSelector);
