import React = require("react");
import { Stack, TextField, ITextFieldStyles, IStackTokens } from "office-ui-fabric-react";
import { RootState } from "../store/store";
import { connect, ConnectedProps } from "react-redux";
import { setBaseUrl } from '../store/dc/actions';

const mapState = (state: RootState) => ({
  baseUrl: state.dc.baseUrl
});

const mapDispatch = {
  setBaseUrl: setBaseUrl
};

const connector = connect(mapState, mapDispatch);

type Props = ConnectedProps<typeof connector>;

const textFieldStyles: Partial<ITextFieldStyles> = { fieldGroup: { width: 300 } };
const stackTokens: IStackTokens = { childrenGap: 10, padding: 10 };

const Settings = (props: Props) => (
  <Stack tokens={stackTokens}>
    <TextField
      label="API base url"
      value={props.baseUrl}
      onChange={(_: any, newValue?: string) => props.setBaseUrl(newValue || "")}
      styles={textFieldStyles}
    />
  </Stack>
);

export default connector(Settings);
