import * as React from "react";
import ReactTable from "react-table";
import "react-table/react-table.css";
import { Button, UncontrolledTooltip } from "reactstrap";
import { ISpace, ISpaceInfo } from "../../Types";

interface IProps {
    filtered: any[];
    spaces: ISpaceInfo[];
    showDetails: (space: ISpace) => void;
    updateFilters: (filters: any[]) => void;
}

export default class SpacesTable extends React.Component<IProps, {}> {
    constructor(props) {
        super(props);

        this.state = {
            filtered: []
        };
    }
    public render() {
        return (
            <ReactTable
                data={this.props.spaces}
                filterable={true}
                minRows={1}
                filtered={this.props.filtered}
                onFilteredChange={filtered => this.props.updateFilters(filtered)}
                defaultPageSize={localStorage.getItem('PeaksDefaultPageSize') || 20}
                onPageSizeChange={(pageSize) => { localStorage.setItem('PeaksDefaultPageSize', pageSize); }}
                columns={[
                    {
                        Cell: row => (
                            <Button
                                color="link"
                                onClick={() => this.props.showDetails(row.original.space)}
                            >
                                Details
                            </Button>
                        ),
                        Header: "Actions",
                        className: "spaces-details",
                        filterable: false,
                        headerClassName: "spaces-details",
                        maxWidth: 150,
                        resizable: false,
                        sortable: false
                    },
                    {
                        Cell: row => (
                            <span>
                                {row.original.space.roomNumber} {row.original.space.bldgName}
                            </span>
                        ),
                        Header: "Room",
                        accessor: row => row.space.roomNumber + " " + row.space.bldgName,
                        filterMethod: (filter, row) =>
                            !!row[filter.id] &&
                            row[filter.id].toLowerCase().includes(filter.value.toLowerCase()),
                        id: "room"
                    },
                    {
                        Cell: row => <span>{row.original.space.roomName}</span>,
                        Header: "Room Name",
                        accessor: "space.roomName",
                        className: "word-wrap",
                        filterMethod: (filter, row) =>
                            !!row[filter.id] &&
                            row[filter.id].toLowerCase().includes(filter.value.toLowerCase())
                    },
                    {
                        Cell: row => <span>{row.original.keyCount}</span>,
                        Header: "Keys",
                        accessor: "keyCount",
                        className: "table-10p",
                        filterable: false,
                        headerClassName: "table-10p"
                    },
                    {
                        Cell: row => <span>{row.original.equipmentCount}</span>,
                        Header: "Equipment",
                        accessor: "equipmentCount",
                        className: "table-10p",
                        filterable: false,
                        headerClassName: "table-10p"
                    },
                    {
                        Cell: row => (
                            <span>
                                {row.value.workstationsInUse} / {row.value.workstationsTotal}
                            </span>
                        ),
                        Filter: ({ filter, onChange }) => (
                            <select
                                onChange={e => onChange(e.target.value)}
                                style={{ width: "100%" }}
                                value={filter ? filter.value : "all"}
                            >
                                <option value="all">Show All</option>
                                <option value="unassigned">Unassigned</option>
                                <option value="assigned">Assigned</option>
                                <option value="any">Any</option>
                            </select>
                        ),
                        Header: header => (
                            <div>
                                Workstations{" "}
                                <i id="workstationsTooltip" className="fas fa-info-circle" />
                                <UncontrolledTooltip placement="right" target="workstationsTooltip">
                                    In Use / Total
                                </UncontrolledTooltip>
                            </div>
                        ),
                        accessor: spaceInfo => {
                            return {
                                workstationsInUse: spaceInfo.workstationsInUse,
                                workstationsTotal: spaceInfo.workstationsTotal
                            };
                        },
                        className: "table-10p",
                        filterMethod: (filter, row) => {
                            if (filter.value === "all") {
                                return true;
                            }
                            if (filter.value === "unassigned") {
                                return (
                                    row.workstationsCount.workstationsTotal -
                                        row.workstationsCount.workstationsInUse >
                                    0
                                );
                            }
                            if (filter.value === "assigned") {
                                return row.workstationsCount.workstationsInUse > 0;
                            }
                            if (filter.value === "any") {
                                return row.workstationsCount.workstationsTotal > 0;
                            }
                        },
                        headerClassName: "table-10p",
                        id: "workstationsCount",
                        sortMethod: (a, b) => {
                            if (a.workstationsTotal === b.workstationsTotal) {
                                if (a.workstationsInUse === b.workstationsInUse) {
                                    return 0;
                                } else {
                                    return a.workstationsInUse < b.workstationsInUse ? 1 : -1;
                                }
                            } else {
                                return a.workstationsTotal < b.workstationsTotal ? 1 : -1;
                            }
                        }
                    }
                ]}
            />
        );
    }
}
