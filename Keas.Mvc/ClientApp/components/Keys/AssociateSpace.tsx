import PropTypes from "prop-types";
import * as React from "react";
import {
  Button,
  Modal,
  ModalBody,
  ModalFooter,
} from "reactstrap";

import { AppContext, IKey, ISpace } from "../../Types";

import SearchKeys from "./SearchKeys";
import SearchSpaces from "../Spaces/SearchSpaces";


interface IProps {
  onAssign: (space: ISpace, key: IKey) => void;
  isModalOpen: boolean;
  openModal: () => void;
  closeModal: () => void;

  selectedKey?: IKey;
  selectedSpace?: ISpace;
}

interface IState {
  error: string;
  selectedKey: IKey;
  selectedSpace: ISpace;
  submitting: boolean;
  validState: boolean;
}

export default class AssociateSpace extends React.Component<IProps, IState> {
  public static contextTypes = {
    fetch: PropTypes.func,
    team: PropTypes.object
  };

  public context: AppContext;

  constructor(props: IProps) {
    super(props);

    this.state = {
      error: "",
      selectedKey: this.props.selectedKey,
      selectedSpace: this.props.selectedSpace,
      submitting: false,
      validState: false
    };
  }

  public componentWillReceiveProps(nextProps: IProps) {
    if (nextProps.selectedKey !== this.props.selectedKey) {
      this.setState({ selectedKey: nextProps.selectedKey });
    }

    if (nextProps.selectedSpace !== this.props.selectedSpace) {
      this.setState({ selectedSpace: nextProps.selectedSpace });
    }
  }

  public render() {
    return (
      <div>
        <Button color="link" onClick={this.props.openModal}>
          <i className="fas fa-plus fa-sm" aria-hidden="true" /> Associate
        </Button>
        { this.renderModal() }
      </div>
    );
  }
  
  private renderModal() {

    const { isModalOpen, selectedKey, selectedSpace } = this.props;

    return (
      <Modal isOpen={isModalOpen} toggle={this._closeModal} size="lg" className="keys-color">
        <div className="modal-header row justify-content-between">
          <h2>Assign Key</h2>
          <Button color="link" onClick={this._closeModal}>
          <i className="fas fa-times fa-lg"/>
          </Button>
        </div>
        <ModalBody>
          <div className="form-group">
            <label htmlFor="assignto">Associate With</label>
            <SearchSpaces
              defaultSpace={selectedSpace}
              onSelect={this._onSelectSpace}
            />
          </div>

          <div className="form-group">
            <label>Pick an key to associate</label>
            <SearchKeys
              defaultKey={selectedKey}
              onSelect={this._onSelected}
              onDeselect={this._onDeselected}
            />
          </div>
          {this.state.error}
        </ModalBody>
        <ModalFooter>
          <Button
            color="primary"
            onClick={this._assignSelected}
            disabled={!this.state.validState || this.state.submitting}
          >
            Go! {this.state.submitting && <i className="fas fa-circle-notch fa-spin"/>}
          </Button>{" "}
        </ModalFooter>
      </Modal>
    );
  }

  // default everything out on close
  private _closeModal = () => {
    const { selectedKey, selectedSpace } = this.props;
    
    this.setState({
      error: "",
      selectedKey,
      selectedSpace,
      submitting: false,
      validState: false
    });
    
    this.props.closeModal();
  }

  // assign the selected key even if we have to create it
  private _assignSelected = async () => {
    const { selectedSpace, selectedKey } = this.state;

    if (!this.state.validState || this.state.submitting) {
      return;
    }

    this.setState({ submitting: true });

    await this.props.onAssign(selectedSpace, selectedKey);

    this._closeModal();
  }

  private _onSelected = (key: IKey) => {
    this.setState({ selectedKey: key, error: "" }, this._validateState);
  }

  private _onDeselected = () => {
    this.setState({ selectedKey: null, error: "" }, this._validateState);
  }

  private _onSelectSpace = (space: ISpace) => {
    this.setState({ selectedSpace: space }, this._validateState);
  }

  private _validateState = () => {
    let valid = true;
    if (!this.state.selectedKey) {
      valid = false;
    }
    else if (this.state.error !== "") {
      valid = false;
    }
    
    this.setState({ validState: valid });
  }
}