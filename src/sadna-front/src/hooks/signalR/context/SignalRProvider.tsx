/* eslint-disable @typescript-eslint/naming-convention */
import React, { useEffect, useMemo, useRef, useState } from 'react';
import { hubConnection } from 'signalr-no-jquery';
import {
    signalRReducer,
    signalRInitialState,
    SignalRDispatchAction,
    SignalRContextState,
    SignalRContextActions,
} from './signalR-reducer.ts';

const maxRetryReconnect = 10;

export type SignalRContextType = {
    state: SignalRContextState;
    dispatch: React.Dispatch<SignalRDispatchAction>;
};

export const SignalRContext = React.createContext<SignalRContextType>(
    {} as SignalRContextType
);

export type SignalRProviderProps = {
    registeredHubs: string[];
    children: JSX.Element;
};

export const SignalRProvider: React.FC<SignalRProviderProps> = ({
    registeredHubs,
    children,
}: SignalRProviderProps) => {
    const [signalRUrl, setSignalRUrl] = useState("http://localhost:8081/signalR");
    const [state, dispatch] = React.useReducer(
        signalRReducer,
        signalRInitialState
    );
    const retryConnection = useRef(0);
    const oneTimeRegisteredHubs = useRef(registeredHubs);

    // useEffect(() => {
    //     setSignalRUrl("http://localhost:8081/signalR");
    // }, ["http://localhost:8081/signalR"]);

    useEffect(() => {
        if (!state.connection && signalRUrl) {
            const newConnection = hubConnection(signalRUrl, {
                logging: true,
            });

            // hubs must be registered before the connection start in order for server to connect
            for (const hub of oneTimeRegisteredHubs.current) {
                const hubProxy = newConnection.createHubProxy(hub);
                hubProxy.on('firstConnection', () => {});
            }

            newConnection
                .start()
                .done(() => {
                    retryConnection.current = 0;
                })
                .fail((e: any) => {
                    console.log(e);
                    console.log('SignalR could not connect');
                });

            newConnection.disconnected(() => {
                if (retryConnection.current <= maxRetryReconnect) {
                    retryConnection.current++;
                    setTimeout(() => {
                        newConnection.start().done(() => {
                            retryConnection.current = 0;
                        });
                    }, 10000);
                }
            });

            dispatch({
                type: SignalRContextActions.SET_CONNECTION,
                payload: newConnection,
            });
        }
        return () => {
            retryConnection.current = 999;
            state.connection?.stop();
        };
    }, [state, dispatch, signalRUrl]);

    const providerValue = useMemo(() => {
        return { state, dispatch };
    }, [state]);

    return (
        <SignalRContext.Provider value={providerValue}>
            {children}
        </SignalRContext.Provider>
    );
};