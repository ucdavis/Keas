import * as React from "react";
import ReactTable from "react-table";
import { Button } from "reactstrap";
import { IKeySerial } from "../../Types";
import { DateUtil } from "../../util/dates";
import ListActionsDropdown, { IAction } from "../ListActionsDropdown";

interface IProps {
    keySerials: IKeySerial[];
    onAssign?: (keySerial: IKeySerial) => void;
    onRevoke?: (keySerial: IKeySerial) => void;
    onUpdate?: (keySerial: IKeySerial) => void;
    showDetails?: (keySerial: IKeySerial) => void;
    onEdit?: (keySerial: IKeySerial) => void;
}

interface IFilter {
    id: string;
    value: any;
}

interface IRow {
    original: IKeySerial;
}

export default class KeySerialTable extends React.Component<IProps, {}> {
    public render() {
        const { keySerials } = this.props;

        return (
            <ReactTable
                data={keySerials}
                filterable={true}
                minRows={1}
                columns={[
                    {
                        Cell: (row: IRow) => (
                            <Button
                                color="link"
                                onClick={() => this.props.showDetails(row.original)}
                            >
                                Details
                            </Button>
                        ),
                        Header: "",
                        className: "key-details",
                        filterable: false,
                        headerClassName: "key-details",
                        maxWidth: 150,
                        resizable: false,
                        sortable: false
                    },
                    {
                        Header: "Code",
                        accessor: "key.code",
                        className: "word-wrap",
                        filterMethod: (filter: IFilter, row: IRow) =>
                            !!row[filter.id] &&
                            row[filter.id].toLowerCase().includes(filter.value.toLowerCase())
                    },
                    {
                        Header: "SN",
                        accessor: "number",
                        className: "word-wrap",
                        filterMethod: (filter: IFilter, row: IRow) =>
                            !!row[filter.id] &&
                            row[filter.id].toLowerCase().includes(filter.value.toLowerCase())
                    },
                    {
                        Header: "Status",
                        accessor: "status",
                        className: "text-mono word-wrap",
                        filterMethod: (filter: IFilter, row: IRow) =>
                            !!row[filter.id] &&
                            row[filter.id].toLowerCase().includes(filter.value.toLowerCase())
                    },
                    {
                        Cell: (row: IRow) => {
                            return row.original.keySerialAssignment
                                ? row.original.keySerialAssignment.person.name
                                : "";
                        },
                        Header: "Assignment",
                        accessor: "keySerial.keySerialAssignment.person.name",
                        className: "word-wrap",
                        filterMethod: (filter: IFilter, row: IRow) =>
                            !!row[filter.id] &&
                            row[filter.id].toLowerCase().includes(filter.value.toLowerCase())
                    },
                    {
                        Cell: (row: IRow) => {
                            return row.original.keySerialAssignment
                                ? DateUtil.formatExpiration(
                                      row.original.keySerialAssignment.expiresAt
                                  )
                                : "";
                        },
                        Header: "Expiration",
                        accessor: "keySerial.keySerialAssignment.expiresAt",
                        className: "word-wrap",
                        filterMethod: (filter: IFilter, row: IRow) =>
                            !!row[filter.id] &&
                            row[filter.id].toLowerCase().includes(filter.value.toLowerCase())
                    },
                    {
                        Cell: this.renderDropdownColumn,
                        Header: "Actions",
                        className: "table-actions",
                        filterable: false,
                        headerClassName: "table-actions",
                        resizable: false,
                        sortable: false
                    }
                ]}
            />
        );
    }

    private renderDropdownColumn = (row: IRow) => {
        const keySerial = row.original;

        const actions: IAction[] = [];
        if (!!this.props.onAssign && !keySerial.keySerialAssignment) {
            actions.push({
                onClick: () => this.props.onAssign(keySerial),
                title: "Assign"
            });
        }

        if (!!this.props.onRevoke && !!keySerial.keySerialAssignment) {
            actions.push({
                onClick: () => this.props.onRevoke(keySerial),
                title: "Revoke"
            });
        }

        return <ListActionsDropdown actions={actions} />;
    };
}
