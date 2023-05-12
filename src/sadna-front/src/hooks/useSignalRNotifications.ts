import { useEffect,useState, useRef } from 'react';
import { Connection, hubConnection } from 'signalr-no-jquery';
export type SignalRNotificationCallback = (...params: any[]) => void;

export type SignalRNotificationsDictionary = {
    [notificationName: string]: SignalRNotificationCallback;
};

const useSignalRNotifications = (
    hubName: string,
    notifications: SignalRNotificationsDictionary
): void => {

    const retryConnection=useRef(0);
    
    const newConnection = hubConnection('http://localhost:8081/signalR', {logging:true,});
    const hubProxy = newConnection.createHubProxy('NotificationHub');
    hubProxy.on('firstConnection',()=>{});
    newConnection.start()
    .done(()=>{
        retryConnection.current=0;
    })
    .fail((e:any)=>{
        console.log(e);
        console.log("signalR could not connect")
    })

    newConnection.disconnected(()=>{
        if(retryConnection.current<=10){
            retryConnection.current++;
            setTimeout(()=>{
                newConnection.start().done(()=>{
                    retryConnection.current=0;
                });
            },10000)
        }
    })

    const [connection,setConnection] =useState<Connection>(newConnection);

    const hubEventsName = useRef(Object.keys(notifications));

    const proxyCallback =
        useRef<(notificationName: string) => SignalRNotificationCallback>();

    useEffect(() => {
        proxyCallback.current = (notificationName: string) => {
                return notifications[notificationName];
        };
    }, [notifications]);

    useEffect(() => {
        if (connection) {
            const hubProxy = connection.createHubProxy(hubName);
            for (const eventName of hubEventsName.current) {
                hubProxy.on(eventName, (...params) => {
                    if (proxyCallback.current) {
                        proxyCallback.current(eventName)(...params);
                    }
                });
            }
        }
        return () => {
            proxyCallback.current = undefined;
        };
    }, [connection, hubName]);
};

export default useSignalRNotifications;