import * as React from 'react';
import { RouteChildrenProps } from 'react-router';
import { toast } from 'react-toastify';
import { Button } from 'reactstrap';
import { Context } from '../../Context';
import { IKey, IKeySerial, IMatchParams, IPerson } from '../../Types';
import { PermissionsUtil } from '../../util/permissions';
import Denied from '../Shared/Denied';
import AssignKeySerial from './AssignKeySerial';
import EditKeySerial from './EditKeySerial';
import KeySerialDetails from './KeySerialDetails';
import KeySerialList from './KeySerialList';
import KeySerialTable from './KeySerialTable';
import RevokeKeySerial from './RevokeKeySerial';

interface IState {
  keySerials: IKeySerial[];
  statusList: string[];
  loading: boolean;
}

interface IProps extends RouteChildrenProps<IMatchParams> {
  selectedPerson?: IPerson;
  selectedKey?: IKey;
  assetInUseUpdated?: (
    type: string,
    keySerialId: number,
    personId: number,
    count: number
  ) => void;
  assetTotalUpdated?: (
    type: string,
    keySerialId: number,
    personId: number,
    count: number
  ) => void;
  assetEdited?: (type: string, keySerialId: number, personId: number) => void;
  goToKeyDetails?: (key: IKey) => void; // will only be supplied from person container
}

export default class KeySerialContainer extends React.Component<
  IProps,
  IState
> {
  public static contextType = Context;
  public context!: React.ContextType<typeof Context>;

  constructor(props: IProps) {
    super(props);

    this.state = {
      keySerials: [],
      loading: true,
      statusList: []
    };
  }
  public async componentDidMount() {
    if (!PermissionsUtil.canViewKeys(this.context.permissions)) {
      return;
    }
    const { selectedPerson, selectedKey } = this.props;

    // are we getting the person's key or the team's?
    let keyFetchUrl = '';
    if (!!selectedPerson) {
      keyFetchUrl = `/api/${this.context.team.slug}/keySerials/getforperson?personid=${selectedPerson.id}`;
    } else if (!!selectedKey) {
      keyFetchUrl = `/api/${this.context.team.slug}/keySerials/getforkey?keyid=${selectedKey.id}`;
    } else {
      keyFetchUrl = `/api/${this.context.team.slug}/keySerials/list/`;
    }

    const statusListFetchUrl = `/api/${this.context.team.slug}/keySerials/ListKeySerialStatus/`;

    let keySerials: IKeySerial[] = null;
    try {
      keySerials = await this.context.fetch(keyFetchUrl);
    } catch (err) {
      toast.error(
        'Failed to fetch key serials. Please refresh the page to try again.'
      );
      return;
    }
    const statusList = await this.context.fetch(statusListFetchUrl);

    this.setState({ keySerials, statusList, loading: false });
  }

  public render() {
    if (!PermissionsUtil.canViewKeys(this.context.permissions)) {
      return <Denied viewName='Keys' />;
    }

    if (this.state.loading) {
      return <h2>Loading...</h2>;
    }

    const { action, assetType, id } = this.props.match.params;
    const activeAsset = !assetType || assetType === 'keyserials';
    const selectedKeySerialId = parseInt(id, 10);
    const selectedKeySerial = this.state.keySerials.find(
      s => s.id === selectedKeySerialId
    );
    return (
      <div className='card keys-color'>
        <div className='card-header-keys'>
          <div className='card-head row justify-content-between'>
            <h2>
              <i className='fas fa-key fa-xs' /> Key Serials
            </h2>
            <Button color='link' onClick={this._openCreateModal}>
              <i className='fas fa-plus fa-sm' aria-hidden='true' /> Add Key
              Serial
            </Button>
          </div>
        </div>
        <div className='card-content'>
          {this.props.selectedPerson && !this.props.selectedKey && (
            <KeySerialList
              keySerials={this.state.keySerials}
              onRevoke={this._openRevokeModal}
              onAssign={this._openAssignModal}
              onEdit={this._openEditModal}
              onUpdate={this._openUpdateModal}
              showDetails={this._openDetailsModal}
            />
          )}
          {!this.props.selectedPerson && this.props.selectedKey && (
            <KeySerialTable
              keySerials={this.state.keySerials}
              onRevoke={this._openRevokeModal}
              onAssign={this._openAssignModal}
              onEdit={this._openEditModal}
              onUpdate={this._openUpdateModal}
              showDetails={this._openDetailsModal}
            />
          )}
          {activeAsset &&
            (action === 'create' ||
              action === 'assign' ||
              action === 'update') &&
            this._renderAssignModal(selectedKeySerialId, selectedKeySerial)}
          {activeAsset &&
            action === 'details' &&
            this._renderDetailsModal(selectedKeySerialId, selectedKeySerial)}
          {activeAsset &&
            action === 'edit' &&
            this._renderEditModal(selectedKeySerialId, selectedKeySerial)}
          {activeAsset &&
            action === 'revoke' &&
            this._renderRevokeModal(selectedKeySerialId, selectedKeySerial)}
        </div>
      </div>
    );
  }

  private _renderAssignModal = (selectedId: number, keySerial: IKeySerial) => {
    return (
      <AssignKeySerial
        key={selectedId ? `assign-keySerial-${selectedId}` : 'create-keySerial'}
        person={this.props.selectedPerson}
        selectedKey={this.props.selectedKey}
        selectedKeySerial={keySerial}
        checkIfKeySerialNumberIsValid={this._checkIfKeySerialNumberIsValid}
        onCreate={this._createAndMaybeAssignKey}
        isModalOpen={true}
        onOpenModal={this._openCreateModal}
        closeModal={this._closeModals}
        openEditModal={this._openEditModal}
        openDetailsModal={this._openDetailsModal}
        statusList={this.state.statusList}
        goToKeyDetails={this.props.goToKeyDetails}
      />
    );
  };

  private _renderDetailsModal = (selectedId: number, keySerial: IKeySerial) => {
    return (
      <KeySerialDetails
        key={`details-keySerial-${selectedId}`}
        selectedKeySerial={keySerial}
        isModalOpen={!!keySerial}
        closeModal={this._closeModals}
        openEditModal={this._openEditModal}
        openUpdateModal={this._openUpdateModal}
        updateSelectedKeySerial={this._updateKeySerialsFromDetails}
        goToKeyDetails={this.props.goToKeyDetails}
      />
    );
  };

  private _renderEditModal = (selectedId: number, keySerial: IKeySerial) => {
    return (
      <EditKeySerial
        key={`edit-keySerial-${selectedId}`}
        selectedKeySerial={keySerial}
        statusList={this.state.statusList}
        isModalOpen={!!keySerial}
        onEdit={this._editKeySerial}
        openUpdateModal={this._openUpdateModal}
        closeModal={this._closeModals}
        goToKeyDetails={this.props.goToKeyDetails}
        checkIfKeySerialNumberIsValid={
          this._checkIfKeySerialNumberIsValidOnEdit
        }
      />
    );
  };

  private _renderRevokeModal = (selectedId: number, keySerial: IKeySerial) => {
    return (
      <RevokeKeySerial
        key={`revoke-keySerial-${selectedId}`}
        selectedKeySerial={keySerial}
        isModalOpen={!!keySerial}
        closeModal={this._closeModals}
        openEditModal={this._openEditModal}
        openUpdateModal={this._openUpdateModal}
        updateSelectedKeySerial={this._updateKeySerialsFromDetails}
        onRevoke={this._revokeKeySerial}
        goToKeyDetails={this.props.goToKeyDetails}
      />
    );
  };

  private _createAndMaybeAssignKey = async (
    person: IPerson,
    keySerial: IKeySerial,
    date: any
  ) => {
    const { team } = this.context;

    let updateTotalAssetCount = false;
    let updateInUseAssetCount = false;

    // if we are creating a new key serial
    if (keySerial.id === 0) {
      const request = {
        keyId: keySerial.key.id,
        notes: keySerial.notes,
        number: keySerial.number,
        status: keySerial.status
      };

      const createUrl = `/api/${team.slug}/keyserials/create`;
      try {
        keySerial = await this.context.fetch(createUrl, {
          body: JSON.stringify(request),
          method: 'POST'
        });
        toast.success('Key serial created successfully!');
      } catch (err) {
        toast.error('Error creating key serial.');
        throw new Error(); // throw error so modal doesn't close
      }

      updateTotalAssetCount = true;
    }

    // if we know who to assign it to, do it now
    if (!!person) {
      if (!keySerial.keySerialAssignment) {
        // don't count as assigning unless this is a new one
        updateInUseAssetCount = true;
      }

      const request = {
        expiresAt: date,
        keySerialId: keySerial.id,
        personId: person.id
      };

      const assignUrl = `/api/${team.slug}/keyserials/assign`;
      try {
        keySerial = await this.context.fetch(assignUrl, {
          body: JSON.stringify(request),
          method: 'POST'
        });
        toast.success('Key serial assigned successfully!');
      } catch (err) {
        toast.error('Error assigning key serial.');
        throw new Error(); // throw error so modal doesn't close
      }

      keySerial.keySerialAssignment.person = person;
    }

    const index = this.state.keySerials.findIndex(x => x.id === keySerial.id);
    const updateKeySerials = [...this.state.keySerials];
    if (index < 0) {
      updateKeySerials.push(keySerial);
    } else {
      // update already existing entry in key
      updateKeySerials[index] = keySerial;
    }

    this.setState({
      keySerials: updateKeySerials
    });

    if (updateTotalAssetCount && this.props.assetTotalUpdated) {
      this.props.assetTotalUpdated(
        'serial',
        null,
        this.props.selectedPerson ? this.props.selectedPerson.id : null,
        1
      );
    }

    if (updateInUseAssetCount && this.props.assetInUseUpdated) {
      this.props.assetInUseUpdated(
        'serial',
        null,
        this.props.selectedPerson ? this.props.selectedPerson.id : null,
        1
      );
    }
  };

  private _revokeKeySerial = async (keySerial: IKeySerial) => {
    const { team } = this.context;

    // call API to actually revoke
    const revokeUrl = `/api/${team.slug}/keyserials/revoke/${keySerial.id}`;
    try {
      keySerial = await this.context.fetch(revokeUrl, {
        method: 'POST'
      });
      toast.success('Key serial revoked successfully!');
    } catch (err) {
      toast.error('Error revoking key serial.');
      throw new Error(); // throw error so modal doesn't close
    }
    // should we remove from state
    const index = this.state.keySerials.findIndex(k => k.id === keySerial.id);
    if (index > -1) {
      const shallowCopy = [...this.state.keySerials];

      if (!this.props.selectedPerson) {
        // if we are looking at all key, just update assignment
        shallowCopy[index] = keySerial;
      } else {
        // if we are looking at a person, remove from our list of key
        shallowCopy.splice(index, 1);
      }

      this.setState({ keySerials: shallowCopy });

      if (this.props.assetInUseUpdated) {
        this.props.assetInUseUpdated(
          'serial',
          null,
          this.props.selectedPerson ? this.props.selectedPerson.id : null,
          -1
        );
      }
    }
  };

  private _editKeySerial = async (keySerial: IKeySerial) => {
    const { team } = this.context;

    const index = this.state.keySerials.findIndex(x => x.id === keySerial.id);

    // should always already exist
    if (index < 0) {
      return;
    }

    const request = {
      notes: keySerial.notes,
      number: keySerial.number,
      status: keySerial.status
    };

    const updateUrl = `/api/${team.slug}/keyserials/update/${keySerial.id}`;
    try {
      keySerial = await this.context.fetch(updateUrl, {
        body: JSON.stringify(request),
        method: 'POST'
      });
      toast.success('Key serial updated successfully!');
    } catch (err) {
      toast.error('Error editing key serial.');
      throw new Error(); // throw error so modal doesn't close
    }

    // update already existing entry in key
    const updateKeySerials = [...this.state.keySerials];
    updateKeySerials[index] = keySerial;

    this.setState({
      ...this.state,
      keySerials: updateKeySerials
    });

    // if(this.props.assetEdited) {
    //   this.props.assetEdited("key", this.props.space ? this.props.space.id : null,
    //     this.props.person ? this.props.person.id : null);
    // }

    // TODO: handle count changes once keys are related to spaces
  };

  private _updateKeySerialsFromDetails = (
    keySerial: IKeySerial,
    id?: number
  ) => {
    const keySerialId = keySerial ? keySerial.id : id;
    const index = this.state.keySerials.findIndex(x => x.id === keySerialId);

    if (index === -1) {
      // should always already exist
      return;
    }

    // update already existing entry in key
    const updateKeySerials = [...this.state.keySerials];
    // if key serial has been deleted elsewhere
    if (keySerial === null) {
      updateKeySerials.splice(index, 1);
    } else {
      updateKeySerials[index] = keySerial;
    }

    this.setState({ ...this.state, keySerials: updateKeySerials });
  };

  private _checkIfKeySerialNumberIsValid = (keySerialNumber: string) => {
    return !this.state.keySerials.some(x => x.number === keySerialNumber);
  };

  private _checkIfKeySerialNumberIsValidOnEdit = (
    keySerialNumber: string,
    id: number
  ) => {
    return !this.state.keySerials.some(
      x => x.number === keySerialNumber && x.id !== id
    );
  };

  private _openAssignModal = (keySerial: IKeySerial) => {
    this.props.history.push(
      `${this._getBaseUrl()}/keyserials/assign/${keySerial.id}`
    );
  };

  private _openCreateModal = () => {
    this.props.history.push(`${this._getBaseUrl()}/keyserials/create`);
  };

  private _openDetailsModal = (keySerial: IKeySerial) => {
    // if we are on person page, and this serial is not in our state
    // this happens on the search, when selecting already assigned
    if (this.state.keySerials.findIndex(x => x.id === keySerial.id) === -1) {
      this.props.history.push(
        `/${this.context.team.slug}/keys/details/${keySerial.key.id}/keyserials/details/${keySerial.id}`
      );
    } else {
      this.props.history.push(
        `${this._getBaseUrl()}/keyserials/details/${keySerial.id}`
      );
    }
  };

  private _openRevokeModal = (keySerial: IKeySerial) => {
    this.props.history.push(
      `${this._getBaseUrl()}/keyserials/revoke/${keySerial.id}`
    );
  };

  private _openEditModal = (keySerial: IKeySerial) => {
    this.props.history.push(
      `${this._getBaseUrl()}/keyserials/edit/${keySerial.id}`
    );
  };

  private _openUpdateModal = (keySerial: IKeySerial) => {
    this.props.history.push(
      `${this._getBaseUrl()}/keyserials/update/${keySerial.id}`
    );
  };

  private _closeModals = () => {
    this.props.history.push(`${this._getBaseUrl()}`);
  };

  private _getBaseUrl = () => {
    const { selectedPerson, selectedKey } = this.props;
    const slug = this.context.team.slug;

    if (!!selectedPerson) {
      return `/${slug}/people/details/${selectedPerson.id}`;
    }

    if (!!selectedKey) {
      return `/${slug}/keys/details/${selectedKey.id}`;
    }

    return `/${slug}`;
  };
}
