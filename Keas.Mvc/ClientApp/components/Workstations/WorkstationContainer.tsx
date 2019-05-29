import * as PropTypes from "prop-types";
import * as React from "react";
import { Button } from "reactstrap";
import { AppContext, IPerson, ISpace, IWorkstation } from "../../Types";
import { PermissionsUtil } from "../../util/permissions";
import Denied from "../Shared/Denied";
import AssignWorkstation from "../Workstations/AssignWorkstation";
import EditWorkstation from "../Workstations/EditWorkstation";
import RevokeWorkstation from "../Workstations/RevokeWorkstation";
import WorkstationDetails from "../Workstations/WorkstationDetails";
import WorkstationList from "./../Workstations/WorkstationList";
import DeleteWorkstation from "./DeleteWorkstation";

interface IProps {
    assetInUseUpdated?: (type: string, spaceId: number, personId: number, count: number) => void;
    assetTotalUpdated?: (type: string, spaceId: number, personId: number, count: number) => void;
    assetEdited?: (type: string, spaceId: number, personId: number) => void;
    space?: ISpace;
    person?: IPerson;
    tags: string[];
}

interface IState {
    loading: boolean;
    workstations: IWorkstation[];
}

export default class WorkstationContainer extends React.Component<IProps, IState> {
    public static contextTypes = {
        fetch: PropTypes.func,
        permissions: PropTypes.array,
        router: PropTypes.object,
        team: PropTypes.object
    };

    public context: AppContext;

    constructor(props) {
        super(props);

        this.state = {
            loading: false,
            workstations: []
        };
    }

    public async componentDidMount() {
        if (!PermissionsUtil.canViewWorkstations(this.context.permissions)) {
            return;
        }
        this.setState({ loading: true });
        let workstations = [];
        if (!this.props.space && !!this.props.person) {
            workstations = await this.context.fetch(
                `/api/${this.context.team.slug}/workstations/listAssigned?personId=${
                    this.props.person.id
                }`
            );
        }
        if (!!this.props.space && !this.props.person) {
            workstations = await this.context.fetch(
                `/api/${this.context.team.slug}/workstations/getWorkstationsInSpace?spaceId=${
                    this.props.space.id
                }`
            );
        }
        this.setState({ workstations, loading: false });
    }

    public render() {
        if (!PermissionsUtil.canViewWorkstations(this.context.permissions)) {
            return <Denied viewName="Workstations" />;
        }
        if (!this.props.space && !this.props.person) {
            return null;
        }
        if (this.state.loading) {
            return <div>Loading Workstations...</div>;
        }
        const { action, assetType, id } = this.context.router.route.match.params;
        const activeAsset = assetType === "workstations";
        const selectedId = parseInt(id, 10);
        const selectedWorkstation = this.state.workstations.find(k => k.id === selectedId);

        return (
            <div className="card spaces-color">
                <div className="card-header-spaces">
                    <div className="card-head">
                        <h2>
                            <i className="fas fa-briefcase fa-xs" /> Workstations
                        </h2>
                    </div>
                </div>
                <div className="card-content">
                    <WorkstationList
                        workstations={this.state.workstations}
                        showDetails={this._openDetailsModal}
                        onEdit={this._openEditModal}
                        onAdd={this._openAssignModal}
                        onCreate={this._openCreateModal}
                        onDelete={this._deleteWorkstation}
                        onRevoke={this._openRevokeModal}
                    />
                    <Button color="link" onClick={() => this._openCreateModal()}>
                        <i className="fas fa-plus fa-sm" aria-hidden="true" /> Add Workstation
                    </Button>
                    <WorkstationDetails
                        closeModal={this._closeModals}
                        modal={activeAsset && action === "details"}
                        selectedWorkstation={selectedWorkstation}
                        openEditModal={this._openEditModal}
                        openUpdateModal={this._openAssignModal}
                    />
                    <EditWorkstation
                        closeModal={this._closeModals}
                        tags={this.props.tags}
                        modal={activeAsset && action === "edit"}
                        selectedWorkstation={selectedWorkstation}
                        onEdit={this._editWorkstation}
                        openUpdateModal={this._openAssignModal}
                    />
                    <AssignWorkstation
                        closeModal={this._closeModals}
                        modal={activeAsset && (action === "assign" || action === "create")}
                        person={
                            selectedWorkstation && selectedWorkstation.assignment
                                ? selectedWorkstation.assignment.person
                                : this.props.person
                        }
                        selectedWorkstation={selectedWorkstation}
                        tags={this.props.tags}
                        space={this.props.space}
                        onCreate={this._createAndMaybeAssignWorkstation}
                        openEditModal={this._openEditModal}
                        openDetailsModal={this._openDetailsModal}
                        onAddNew={this._openCreateModal}
                    />
                    <RevokeWorkstation
                        closeModal={this._closeModals}
                        revokeWorkstation={this._revokeWorkstation}
                        modal={activeAsset && action === "revoke"}
                        selectedWorkstation={selectedWorkstation}
                    />
                    <DeleteWorkstation
                        selectedWorkstation={selectedWorkstation}
                        deleteWorkstation={this._deleteWorkstation}
                        closeModal={this._closeModals}
                        modal={activeAsset && action === "delete"}
                    />
                </div>
            </div>
        );
    }

    private _createAndMaybeAssignWorkstation = async (
        person: IPerson,
        workstation: IWorkstation,
        date: any
    ) => {
        let updateTotalAssetCount = false;
        let updateInUseAssetCount = false;

        // call API to create a workstation, then assign it if there is a person to assign to
        // if we are creating a new workstation
        if (workstation.id === 0) {
            workstation.teamId = this.context.team.id;
            workstation = await this.context.fetch(
                `/api/${this.context.team.slug}/workstations/create`,
                {
                    body: JSON.stringify(workstation),
                    method: "POST"
                }
            );
            updateTotalAssetCount = true;
        }

        // if we know who to assign it to, do it now
        if (person) {
            const assignUrl = `/api/${this.context.team.slug}/workstations/assign?workstationId=${
                workstation.id
            }&personId=${person.id}&date=${date}`;

            if (!workstation.assignment) {
                // only count as assigned if this is a new one
                updateInUseAssetCount = true;
            }
            workstation = await this.context.fetch(assignUrl, {
                method: "POST"
            });
            workstation.assignment.person = person;
        }

        const index = this.state.workstations.findIndex(x => x.id === workstation.id);
        if (index !== -1) {
            // update already existing entry in workstation
            const updateWorkstation = [...this.state.workstations];
            updateWorkstation[index] = workstation;

            this.setState({
                ...this.state,
                workstations: updateWorkstation
            });
        } else if (!!this.props.space && this.props.space.id !== workstation.space.id) {
            // if we are on the space tab and we have created a workstation that is not in this space, do nothing to our state here
        } else {
            this.setState({
                workstations: [...this.state.workstations, workstation]
            });
        }
        if (updateTotalAssetCount && this.props.assetTotalUpdated) {
            this.props.assetTotalUpdated(
                "workstation",
                workstation.space ? workstation.space.id : null,
                this.props.person ? this.props.person.id : null,
                1
            );
        }
        if (updateInUseAssetCount && this.props.assetInUseUpdated) {
            this.props.assetInUseUpdated(
                "workstation",
                workstation.space ? workstation.space.id : null,
                this.props.person ? this.props.person.id : null,
                1
            );
        }
    };

    private _revokeWorkstation = async (workstation: IWorkstation) => {
        if (!confirm("Are you sure you want to revoke workstation?")) {
            return false;
        }
        // call API to actually revoke
        const removed: IWorkstation = await this.context.fetch(
            `/api/${this.context.team.slug}/workstations/revoke`,
            {
                body: JSON.stringify(workstation),
                method: "POST"
            }
        );

        // remove from state
        const index = this.state.workstations.indexOf(workstation);
        if (index > -1) {
            const shallowCopy = [...this.state.workstations];
            if (!this.props.person && !!this.props.space) {
                // if we are looking at all workstations, just update assignment
                shallowCopy[index].assignment = null;
            } else {
                // if we are looking at a person, remove from our list of workstations
                shallowCopy.splice(index, 1);
            }
            this.setState({ workstations: shallowCopy });

            if (this.props.assetInUseUpdated) {
                this.props.assetInUseUpdated(
                    "workstation",
                    this.props.space ? this.props.space.id : null,
                    this.props.person ? this.props.person.id : null,
                    -1
                );
            }
        }
    };

    private _deleteWorkstation = async (workstation: IWorkstation) => {
        if (!confirm("Are you sure you want to delete item?")) {
            return false;
        }
        const deleted: IWorkstation = await this.context.fetch(
            `/api/${this.context.team.slug}/workstations/delete/${workstation.id}`,
            {
                method: "POST"
            }
        );

        // remove from state
        const index = this.state.workstations.indexOf(workstation);
        if (index > -1) {
            const shallowCopy = [...this.state.workstations];
            shallowCopy.splice(index, 1);
            this.setState({ workstations: shallowCopy });

            if (workstation.assignment !== null && this.props.assetInUseUpdated) {
                this.props.assetInUseUpdated(
                    "workstation",
                    this.props.space ? this.props.space.id : null,
                    this.props.person ? this.props.person.id : null,
                    -1
                );
            }
            if (this.props.assetTotalUpdated) {
                this.props.assetTotalUpdated(
                    "workstation",
                    this.props.space ? this.props.space.id : null,
                    this.props.person ? this.props.person.id : null,
                    -1
                );
            }
        }
    };

    private _editWorkstation = async (workstation: IWorkstation) => {
        const index = this.state.workstations.findIndex(x => x.id === workstation.id);
        if (index === -1) {
            // should always already exist
            return;
        }
        const updated: IWorkstation = await this.context.fetch(
            `/api/${this.context.team.slug}/workstations/update`,
            {
                body: JSON.stringify(workstation),
                method: "POST"
            }
        );

        updated.assignment = workstation.assignment;

        // update already existing entry in key
        const updateWorkstation = [...this.state.workstations];
        updateWorkstation[index] = updated;

        this.setState({
            ...this.state,
            workstations: updateWorkstation
        });

        if (this.props.assetEdited) {
            this.props.assetEdited(
                "workstation",
                this.props.space ? this.props.space.id : null,
                this.props.person ? this.props.person.id : null
            );
        }
    };

    private _openDetailsModal = (workstation: IWorkstation) => {
        // if we are on spaces or person page, and this workstation is not in our state
        // this happens on the search, when selecting already assigned
        if (this.state.workstations.findIndex(x => x.id === workstation.id) === -1) {
            this.context.router.history.push(
                `/${this.context.team.slug}/spaces/details/${
                    workstation.space.id
                }/workstations/details/${workstation.id}`
            );
        } else {
            this.context.router.history.push(
                `${this._getBaseUrl()}/workstations/details/${workstation.id}`
            );
        }
    };

    private _openEditModal = (workstation: IWorkstation) => {
        this.context.router.history.push(
            `${this._getBaseUrl()}/workstations/edit/${workstation.id}`
        );
    };

    private _openAssignModal = (workstation: IWorkstation) => {
        this.context.router.history.push(
            `${this._getBaseUrl()}/workstations/assign/${workstation.id}`
        );
    };

    private _openCreateModal = () => {
        this.context.router.history.push(`${this._getBaseUrl()}/workstations/create/`);
    };

    private _openRevokeModal = (workstation: IWorkstation) => {
        this.context.router.history.push(
            `${this._getBaseUrl()}/workstations/revoke/${workstation.id}`
        );
    };

    private _closeModals = () => {
        if (!!this.props.person && !this.props.space) {
            this.context.router.history.push(`${this._getBaseUrl()}`);
        } else if (!this.props.person && !!this.props.space) {
            this.context.router.history.push(`${this._getBaseUrl()}`);
        }
    };

    private _getBaseUrl = () => {
        return this.props.person
            ? `/${this.context.team.slug}/people/details/${this.props.person.id}`
            : `/${this.context.team.slug}/spaces/details/${this.props.space.id}`;
    };
}
