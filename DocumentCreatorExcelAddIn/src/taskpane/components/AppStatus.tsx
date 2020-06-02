import * as React from "react";
import { Spinner, SpinnerSize, IStackStyles, Stack, IStackTokens, IStackItemStyles } from "office-ui-fabric-react";
import { RootState } from '../store/store';
import { connect, ConnectedProps } from 'react-redux';

const stackStyles: IStackStyles = {
  root: {
    width: "100%",
    //background: DefaultPalette.themeTertiary
  }
};
const stackItemStyles: IStackItemStyles = {
  root: {
    alignItems: "center",
    //background: DefaultPalette.themePrimary,
    //color: DefaultPalette.white,
    display: "flex",
    //height: 20,
    justifyContent: "flex-start"
  }
};
const stackTokens: IStackTokens = {
  childrenGap: 5,
  padding: 5
};

const mapState = (state: RootState) => ({
  error: state.dc.error,
  pendingRequests: state.dc.pending
});
const connector = connect(mapState, {});
type Props = ConnectedProps<typeof connector>;

const AppStatus = (props: Props) => {
  return (
    <Stack horizontal styles={stackStyles} tokens={stackTokens}>
      <Stack.Item grow={11} styles={stackItemStyles}>
        <span>{props.error?.message }</span>
      </Stack.Item>
      <Stack.Item grow styles={stackItemStyles}>
        {props.pendingRequests > 0 ? <Spinner size={SpinnerSize.xSmall} /> : null}
      </Stack.Item>
    </Stack>
  );
};

export default connector(AppStatus);
