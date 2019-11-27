import * as React from 'react';
import { IHistory } from '../../models/Shared';
import HistoryListItem from './HistoryListItem';

interface IProps {
  histories: IHistory[];
}

export default class HistoryList extends React.Component<IProps, {}> {
  public render() {
    const histories = this.props.histories.map(x => (
      <HistoryListItem key={x.id} history={x} />
    ));
    return (
      <div className='table'>
        <table className='table'>
          <thead>
            <tr />
          </thead>
          <tbody>{histories}</tbody>
        </table>
      </div>
    );
  }
}
