import * as PropTypes from 'prop-types';
import * as React from 'react';
import { toast } from 'react-toastify';
import { Button, Modal, ModalBody } from 'reactstrap';
import { AppContext, IKeySerial } from '../../Types';
import HistoryContainer from '../History/HistoryContainer';
import KeySerialAssignmentValues from './KeySerialAssignmentValues';
import KeySerialEditValues from './KeySerialEditValues';

interface IProps {
  isModalOpen: boolean;
  closeModal: () => void;
  openEditModal: (keySerial: IKeySerial) => void;
  openUpdateModal: (keySerial: IKeySerial) => void;
  selectedKeySerial: IKeySerial;
  updateSelectedKeySerial: (keySerial: IKeySerial, id?: number) => void;
}

export default class KeyDetails extends React.Component<IProps, {}> {
  public static contextTypes = {
    fetch: PropTypes.func,
    team: PropTypes.object
  };
  public context: AppContext;

  public componentDidMount() {
    if (!this.props.selectedKeySerial) {
      return;
    }
    this._fetchDetails(this.props.selectedKeySerial.id);
  }

  // make sure we change the key we are updating if the parent changes selected key
  public componentWillReceiveProps(nextProps: IProps) {
    if (
      nextProps.selectedKeySerial &&
      (!this.props.selectedKeySerial ||
        nextProps.selectedKeySerial.id !== this.props.selectedKeySerial.id)
    ) {
      this._fetchDetails(nextProps.selectedKeySerial.id);
    }
  }
  public render() {
    const { selectedKeySerial } = this.props;

    if (!selectedKeySerial) {
      return null;
    }

    return (
      <div>
        <Modal
          isOpen={this.props.isModalOpen}
          toggle={this.props.closeModal}
          size='lg'
          className='keys-color'
        >
          <div className='modal-header row justify-content-between'>
            <h2>
              Details for {selectedKeySerial.key.code}{' '}
              {selectedKeySerial.number}
            </h2>
            <Button color='link' onClick={this.props.closeModal}>
              <i className='fas fa-times fa-lg' />
            </Button>
          </div>
          <ModalBody>
            <KeySerialEditValues
              keySerial={selectedKeySerial}
              disableEditing={true}
              openEditModal={this.props.openEditModal}
            />
            <KeySerialAssignmentValues
              selectedKeySerial={selectedKeySerial}
              openUpdateModal={this.props.openUpdateModal}
            />
            <HistoryContainer
              controller='keyserials'
              id={selectedKeySerial.id}
            />
          </ModalBody>
        </Modal>
      </div>
    );
  }

  private _fetchDetails = async (id: number) => {
    const url = `/api/${this.context.team.slug}/keySerials/details/${id}`;
    let keySerial: IKeySerial = null;
    try {
      keySerial = await this.context.fetch(url);
    } catch (err) {
      if (err.message === 'Not Found') {
        toast.error(
          'The key serial you were trying to view could not be found. It may have been deleted.'
        );
        this.props.updateSelectedKeySerial(null, id);
        this.props.closeModal();
      } else {
        toast.error(
          'Error fetching key serial details. Please refresh the page to try again.'
        );
      }
      return;
    }
    this.props.updateSelectedKeySerial(keySerial);
  };
}
