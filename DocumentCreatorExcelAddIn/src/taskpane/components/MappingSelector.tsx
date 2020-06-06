import React = require("react");
import {
  ComboBox,
  IComboBoxOption,
  IComboBox,
  IComboBoxStyles,
  Stack,
  IStackTokens,
  PrimaryButton
} from "office-ui-fabric-react";
import { useEffect } from "react";
import { Mapping } from "../store/dc/types";
import { RootState } from "../store/store";
import { ConnectedProps, connect } from "react-redux";
import { fetchMappings, selectMapping } from "../store/dc/actions";
import { ExcelHelper } from "../modules/excel";

const comboboxStyles: Partial<IComboBoxStyles> = { container: { width: 300 } };
const stackTokens: IStackTokens = { childrenGap: 10, padding: 10 };

const mapState = (state: RootState) => ({
  baseUrl: state.dc.baseUrl,
  activeTemplate: state.dc.activeTemplate,
  activeMappingName: state.dc.activeMappingName,
  availableMappings: state.dc.availableMappings
});
const mapDispatch = {
  getMappings: fetchMappings,
  selectMapping: selectMapping
};

const connector = connect(mapState, mapDispatch);
type PropsFromRedux = ConnectedProps<typeof connector>;
type Props = PropsFromRedux;

const mappingOptions: IComboBoxOption[] = [];

function buildMappingOptions(mappings: Mapping[]) {
  const items = mappings.map(t => {
    return { key: t.mappingName, text: t.mappingName, data: t };
  });
  mappingOptions.splice(0, mappingOptions.length, ...items);
}

const MappingSelector = (props: Props) => {
  useEffect(() => {
    if (props.activeTemplate) {
      if (props.availableMappings) {
        buildMappingOptions(props.availableMappings);
      } else {
        props
          .getMappings(props.baseUrl, props.activeTemplate.templateName)
          .then(() => buildMappingOptions(props.availableMappings));
      }
    } else {
      mappingOptions.splice(0, mappingOptions.length);
    }
  });

  return (
    <Stack tokens={stackTokens}>
      <ComboBox
        label="Select mapping"
        text={props.activeMappingName}
        //key={'' + autoComplete + allowFreeform}
        allowFreeform
        autoComplete="off"
        onChange={(_event: React.FormEvent<IComboBox>, option?: IComboBoxOption, _index?: number, value?: string) =>
          props.selectMapping(option ? option.text : value)
        }
        options={mappingOptions}
        styles={comboboxStyles}
      />
      <PrimaryButton
        text="Test current mapping"
        onClick={async (_: any) =>
          await ExcelHelper.testMappings(props.activeTemplate.templateName, props.activeMappingName)
        }
        allowDisabledFocus
        disabled={!props.activeTemplate || !props.activeMappingName}
      />
    </Stack>
  );
};
export default connector(MappingSelector);
