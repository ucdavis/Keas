import * as React from "react";

import { IKey } from "../../Types";
import ListActionsDropdown, { IAction } from "../ListActionsDropdown";

interface IProps {
  keyEntity: IKey;
  onDisassociate?: (key: IKey) => void;
  onAdd?: (key: IKey) => void;
  showDetails?: (key: IKey) => void;
  onEdit?: (key: IKey) => void;
  onDelete?: (key: IKey) => void;
}

export default class KeyListItem extends React.Component<IProps, {}> {
  public render() {
    const { keyEntity } = this.props;
    const keySerials = keyEntity.serials || [];

    const total = keySerials.length;
    const available = keySerials.filter(s => !s.keySerialAssignment).length;

    const actions: IAction[] = [];
    if (!!this.props.onDisassociate) {
      actions.push({ title: 'Disassociate', onClick: () => this.props.onDisassociate(keyEntity) });
    }

    if (!!this.props.showDetails) {
        actions.push({ title: 'Details', onClick: () => this.props.showDetails(keyEntity) });
    }

    if (!!this.props.onEdit) {
        actions.push({ title: 'Edit', onClick: () => this.props.onEdit(keyEntity) });
    }

    if (!!this.props.onDelete) {
      actions.push({ title: 'Delete', onClick: () => this.props.onDelete(keyEntity) });
  }

    return (
      <tr>
        <td>{keyEntity.name}</td>
        <td>{keyEntity.code}</td>
        <td className=""><i className="fas fa-key"/> {available} / {total}</td>
        <td>
          <ListActionsDropdown actions={actions} />
        </td>
      </tr>
    );
  }
}
