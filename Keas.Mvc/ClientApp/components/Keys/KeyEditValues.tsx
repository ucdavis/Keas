﻿import * as React from "react";

import { IKey } from "../../Types";

interface IProps {
    selectedKey: IKey;
    disableEditing: boolean;
    changeProperty?: (property: string, value: string) => void;
    creating?: boolean;
}

export default class KeyEditValues extends React.Component<IProps, {}> {

    public render() {
        const { selectedKey } = this.props;
        const code = selectedKey ? selectedKey.code : "";

        return (
            <div>
                {!this.props.creating &&
                    <div className="form-group">
                        <label>Code</label>
                        <input type="text"
                            className="form-control"
                            disabled={this.props.disableEditing}
                            value={code}
                            onChange={this.onChangeCode}
                        />
                    </div>
                }
            </div>
        );
    }

    private onChangeCode = (event: React.ChangeEvent<HTMLInputElement>) => {
        this.props.changeProperty("code", event.target.value)
    }
}
