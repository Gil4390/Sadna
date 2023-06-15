/* eslint-disable @typescript-eslint/naming-convention */
import { Connection } from 'signalr-no-jquery';

export type SignalRDispatchAction = {
    type: SignalRContextActions;
    payload: any;
};

export type SignalRContextState = {
    connection?: Connection;
};

export const signalRInitialState: SignalRContextState = {
    connection: undefined,
};

export enum SignalRContextActions {
    SET_CONNECTION,
}

export const signalRReducer = (
    state: SignalRContextState,
    action: SignalRDispatchAction
): SignalRContextState => {
    switch (action.type) {
        case SignalRContextActions.SET_CONNECTION:
            return {
                ...state,
                connection: action.payload,
            };

        default:
            return state;
    }
};