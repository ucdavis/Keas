import * as React from 'react';
import { Button } from 'reactstrap';
import { ISpace } from '../../Types';
import ListActionsDropdown, { IAction } from '../ListActionsDropdown';

interface IProps {
  space: ISpace;
  onDisassociate?: (space: ISpace) => void;
  showDetails?: (space: ISpace) => void;
}

export default class SpacesListItem extends React.Component<IProps, {}> {
  public render() {
    const { space } = this.props;

    const actions: IAction[] = [];

    if (!!this.props.onDisassociate) {
      actions.push({
        onClick: () => this.props.onDisassociate(space),
        title: 'Disassociate'
      });
    }

    return (
      <tr>
        <td>
          <Button
            color='link'
            onClick={() => this.props.showDetails(this.props.space)}
          >
            Details
          </Button>
        </td>
        <td>
          {space.roomNumber} {space.bldgName}
        </td>
        <td>{space.roomName}</td>
        <td>
          <ListActionsDropdown actions={actions} />
        </td>
      </tr>
    );
  }
}
