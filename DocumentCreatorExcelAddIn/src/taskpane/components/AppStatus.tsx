import * as React from "react";
import { Spinner, SpinnerSize, MessageBar, MessageBarType } from "office-ui-fabric-react";
import { RootState } from "../store/store";
import { connect, ConnectedProps } from "react-redux";
import { resetError } from '../store/dc/actions';

const mapState = (state: RootState) => ({
  errorMessage: state.dc.errorMessage,
  pendingRequests: state.dc.pending
});
const mapDispatch = {
  resetError: resetError
};
const connector = connect(mapState, mapDispatch);
type Props = ConnectedProps<typeof connector>;

const AppStatus = (props: Props) => {
  if (props.errorMessage)
    return (
      <MessageBar
        messageBarType={MessageBarType.error}
        isMultiline={false}
        onDismiss={props.resetError}
        dismissButtonAriaLabel="Close"
      >
        {props.errorMessage}
      </MessageBar>
    );
  else if (props.pendingRequests > 0) {
    return (<Spinner size={SpinnerSize.xSmall} />);
  }
  return null;
};

export default connector(AppStatus);
