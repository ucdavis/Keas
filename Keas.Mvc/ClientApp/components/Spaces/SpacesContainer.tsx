﻿import * as PropTypes from 'prop-types';
import * as React from "react";

import { PermissionsUtil } from "../../util/permissions";

import { AppContext, ISpace, ISpaceInfo, IKey } from "../../Types";

import Denied from "../Shared/Denied";
import SearchTags from "../Tags/SearchTags";

import AssociateSpace from "../Keys/AssociateSpace";
import SpacesDetails from "./SpacesDetails";
import SpacesList from "./SpacesList";
import SpacesTable from "./SpacesTable";

interface IProps {
    selectedKey?: IKey;
}

interface IState {
    spaces: ISpaceInfo[];
    loading: boolean;
    tableFilters: any[]; // object containing filters on table
    tagFilters: string[]; // array of filters from SearchTags
    tags: string[];
}
export default class SpacesContainer extends React.Component<IProps, IState> {
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
            loading: true,
            spaces: [],
            tableFilters: [],
            tagFilters: [],
            tags: [],
        };
    }

    public async componentDidMount() {
        const { selectedKey } = this.props;
        const { team } = this.context;

        let spacesFetchUrl =  "";
        if(!!selectedKey) {
            spacesFetchUrl = `/api/${team.slug}/spaces/getSpacesForKey?keyid=${selectedKey.id}`;
        }
        else {
            spacesFetchUrl = `/api/${team.slug}/spaces/list`;
        }

        const spaces = await this.context.fetch(spacesFetchUrl);
        const tags = await this.context.fetch(`/api/${team.slug}/tags/listTags`);

        this.setState({ loading: false, spaces, tags });
    }

    public render() {
        const permissionArray = ['SpaceMaster', 'DepartmentalAdmin', 'Admin'];
        if (!PermissionsUtil.canViewSpace(this.context.permissions)) {
            return (
                <Denied viewName="Space" />
            );
        }

        if(this.state.loading) {
            return <h2>Loading...</h2>;
        }
        const { spaceAction, spaceId, assetType, action, id } = this.context.router.route.match.params;
        const activeWorkstationAsset = assetType === "workstations";
        const selectedId = parseInt(spaceId, 10);
        const selectedSpaceInfo = this.state.spaces.find(k => k.id === selectedId);


        return (
            <div className="card spaces-color">
                <div className="card-header-spaces">
                    <div className="card-head">
                        <h2><i className="fas fa-building fa-xs"/> Spaces</h2>
                    </div>
                </div>

                <div className="card-content">
                    {!spaceAction && !activeWorkstationAsset &&
                        this._renderTableOrList()
                    }
                    { spaceAction === "details" && (!!selectedSpaceInfo && !!selectedSpaceInfo.space) &&
                        this._renderDetailsView(selectedSpaceInfo)
                    }
                    
                </div>
            </div>
        );
    }

    private _renderTableOrList() {
        const { selectedKey } = this.props;
        if (!!selectedKey) {
            return this._renderTableList();
        }

        return this._renderTableView();
    }

    // if we are at route teamName/spaces
    private _renderTableView() {
        
        let filteredSpaces = [];
        if(!!this.state.tagFilters && this.state.tagFilters.length > 0) {
            filteredSpaces = this.state.spaces.filter(x => this._checkFilters(x, this.state.tagFilters));
        }
        else {
            filteredSpaces = this.state.spaces;
        }

        return (
            <div>
                <SearchTags tags={this.state.tags} selected={this.state.tagFilters} onSelect={this._filterTags} disabled={false}/>
                <SpacesTable
                    spaces={filteredSpaces}
                    showDetails={this._openDetails}
                    filtered={this.state.tableFilters}
                    updateFilters={this._updateTableFilters}
                />
            </div>
        );
    }

    private _renderTableList = () => {
        const { action } = this.context.router.route.match.params;
        const { selectedKey } = this.props;

        // flatten the space info for simple space
        const spaces = this.state.spaces.map(s => s.space);

        return (
            <div>
                <SpacesList
                    selectedKey={selectedKey}
                    spaces={spaces}
                    showDetails={this._openDetails}
                    onDisassociate={this._disassociateSpace}
                />
                <AssociateSpace
                    selectedKey={selectedKey}
                    onAssign={this._associateSpace}
                    openModal={() => this._openAssociateModal(null)}
                    closeModal={this._closeModals}
                    isModalOpen={action === "associate"}
                />
            </div>
        )
    }

    // if we are at route teamName/spaces/details/spaceId
    private _renderDetailsView = (selectedSpaceInfo: ISpaceInfo) => {
        return(
            <SpacesDetails
                closeModal={this._closeModals}
                selectedSpaceInfo={selectedSpaceInfo}
                tags={this.state.tags}
                inUseUpdated={this._assetInUseUpdated}
                totalUpdated={this._assetTotalUpdated}
                edited={this._assetEdited}
            />
        );
    }

    private _updateTableFilters = (filters: any[]) => {
        this.setState({tableFilters: filters});
    }

    private _openDetails = (space: ISpace) => {
        const { team } = this.context;
        this.context.router.history.push(`/${team.slug}/spaces/details/${space.id}`);
    };

    private _openAssociateModal = (space: ISpace) => {
        if (!!space) {
            this.context.router.history.push(`${this._getBaseUrl()}/spaces/associate/${space.id}`);
            return;
        }

        this.context.router.history.push(`${this._getBaseUrl()}/spaces/associate/`);
    }

    private _closeModals = () => {
        this.context.router.history.push(`${this._getBaseUrl()}`);
    };

    // managing counts for assigned or revoked
    private _assetInUseUpdated = (type: string, spaceId: number, personId: number, count: number) => {
        // this is called when we are assigning or revoking an asset
        const index = this.state.spaces.findIndex(x => x.id === spaceId);
        if(index > -1)
        {
            const spaces = [...this.state.spaces];
            switch(type) {
            case "workstation":
                spaces[index].workstationsInUse += count;
            }
            this.setState({spaces});
        }
    }

    private _assetTotalUpdated = (type: string, spaceId: number, personId: number, count: number) => {
        // this is called when we are either changing the room on an asset, or creating a workstation
        const index = this.state.spaces.findIndex(x => x.id === spaceId);
        if(index > -1)
        {
            const spaces = [...this.state.spaces];
            switch(type) {
                case "equipment": 
                spaces[index].equipmentCount += count;
                break;
                case "key":
                spaces[index].keyCount += count;
                break;
                case "workstation":
                spaces[index].workstationsTotal += count;
            }
            this.setState({spaces});
        } 
    }

    private _assetEdited = async (type: string, spaceId: number, personId: number) => {
        const index = this.state.spaces.findIndex(x => x.id === spaceId);
        if(index > -1 )
        {
            const tags = await this.context.fetch(`/api/${this.context.team.slug}/spaces/getTagsInSpace?spaceId=${spaceId}`);
            const spaces = [...this.state.spaces];
            spaces[index].tags = tags;
            this.setState({spaces});
        }
    }

    private _associateSpace = async (space: ISpace, key: IKey) => {
        const { team } = this.context;
        const { spaces } = this.state;

        const request = {
            spaceId: space.id,
        };

        const associateUrl = `/api/${team.slug}/keys/associateSpace/${key.id}`;
        const result = await this.context.fetch(associateUrl, {
            body: JSON.stringify(request),
            method: "POST",
        });

        const spaceInfo: ISpaceInfo = {
            equipmentCount: 0,
            id: space.id,
            keyCount: 0,
            space,
            tags: "",
            workstationsInUse: 0,
            workstationsTotal: 0,
        };
        const updatedSpaces = [...spaces, spaceInfo];
        this.setState({
            spaces: updatedSpaces,
        });
    }

    private _disassociateSpace = async (space: ISpace, key: IKey) => {
        const { team } = this.context;
        const { spaces } = this.state;

        const request = {
            spaceId: space.id,
        }

        const disassociateUrl = `/api/${team.slug}/keys/disassociateSpace/${key.id}`;
        const result = await this.context.fetch(disassociateUrl, {
            body: JSON.stringify(request),
            method: "POST",
        });

        
        const updatedSpaces = [...spaces];
        const index = updatedSpaces.findIndex(s => s.space.id === space.id);
        updatedSpaces.splice(index, 1);

        this.setState({
            spaces: updatedSpaces,
        });
    }

    private _getBaseUrl = () => {
        const { team } = this.context;
        const { selectedKey } = this.props;

        if(!!selectedKey)
        {
            return `/${team.slug}/keys/details/${selectedKey.id}`;
        }

        return `/${team.slug}`;
    };

    private _filterTags = (filters: string[]) => {
        this.setState({tagFilters: filters});
    }

    private _checkFilters = (space: ISpaceInfo, filters: string[]) => {
        return filters.every(f => !!space && !!space.tags && space.tags.includes(f));
    }
}
