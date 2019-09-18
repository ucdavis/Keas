import * as React from "react";
import { UncontrolledTooltip } from "reactstrap";
import { IKey, IKeyInfo } from "../../Types";
import KeyListItem from "./KeyListItem";

interface IProps {
    keysInfo: IKeyInfo[];
    onDisassociate?: (key: IKeyInfo) => void;
    onAdd?: (key: IKey) => void;
    showDetails?: (key: IKey) => void;
    onEdit?: (key: IKey) => void;
    onDelete?: (key: IKey) => void;
}

export default class KeyList extends React.Component<IProps, {}> {
    public render() {
        const { keysInfo } = this.props;
        const keys =
            !keysInfo || keysInfo.length < 1 ? (
                <tr>
                    <td colSpan={4}>No Keys Found</td>
                </tr>
            ) : (
                keysInfo.map(this.renderItem)
            );

        return (
            <table className="table">
                <thead>
                    <tr>
                        <th />
                        <th>Key Name</th>
                        <th>Key Code</th>
                        <th>
                            Serials{" "}
                            <i id="serialTooltip" className="fas fa-info-circle" />
                            <UncontrolledTooltip placement="right" target="serialTooltip">
                                In Use / Total
                            </UncontrolledTooltip>
                        </th>
                        <th className="list-actions">Actions</th>
                    </tr>
                </thead>
                <tbody>{keys}</tbody>
            </table>
        );
    }

    private renderItem = keyInfo => {
        return (
            <KeyListItem
                key={keyInfo.id}
                keyInfo={keyInfo}
                onDisassociate={this.props.onDisassociate}
                onAdd={this.props.onAdd}
                onDelete={this.props.onDelete}
                showDetails={this.props.showDetails}
                onEdit={this.props.onEdit}
            />
        );
    };
}
