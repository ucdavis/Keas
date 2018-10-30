﻿import * as React from "react";

import { IAccess, IAccessAssignment } from "../../Types";

import ReactTable from 'react-table';
import { DateUtil } from "../../util/dates";
import SearchTags from "../Tags/SearchTags";



interface IProps {
    selectedAccess: IAccess;
    disableEditing: boolean;
    changeProperty?: (property: string, value: string) => void;
    creating?: boolean;
    onRevoke: (accessAssignment: IAccessAssignment) => void;
    tags?: string[];
}

interface IState {
    access: IAccess;
}


export default class AccessEditValues extends React.Component<IProps, IState> {
    constructor(props) {
        super(props);
        this.state = {
            access: this.props.selectedAccess
        };
    }

    public render() {
        const columns = [{
            id: "personFirstName",
            Header: "First Name",
            accessor: x=> x.person.firstName
        }, {
            id: "personLastName",
            Header: "Last Name",
            accessor: x=> x.person.lastName
        }, {
            id: "expiresAt",
            Header: "Expires at",
            accessor: x=> DateUtil.formatExpiration(x.expiresAt)
        },{
            Header: "Revoke",
            Cell: row => (<button type="button" className="btn btn-outline-danger" disabled={this.props.disableEditing  || !this.props.onRevoke}
                onClick={() => this._revokeSelected(row.original.person.id)}><i className="fas fa-trash" /></button>),        
            sortable: false,
        }]     

        return (
            <div>
                {!this.props.creating &&
                <div className="form-group">
                    <label>Item</label>
                    <input type="text"
                        className="form-control"
                        disabled={this.props.disableEditing}
                        value={this.props.selectedAccess.name ? this.props.selectedAccess.name : ""}
                        onChange={(e) => this.props.changeProperty("name", e.target.value)}
                    />
                </div>}
                <div className="form-group">
                    <label>Tags</label>
                    <SearchTags 
                        tags={this.props.tags} 
                        disabled={this.props.disableEditing}
                        selected={!!this.props.selectedAccess.tags ? this.props.selectedAccess.tags.split(",") : []}
                        onSelect={(e) => this.props.changeProperty("tags", e.join(","))} />
                </div>
                <h3>Assigned to:</h3>             
                <ReactTable data={this.props.selectedAccess.assignments} columns={columns} minRows={1} />
            </div>
        );
    }

    private _revokeSelected = async (personId: number) => { 
        const accessAssignment = this.state.access.assignments.filter(x => x.personId === personId);
        await this.props.onRevoke(accessAssignment[0]);
    };
}
