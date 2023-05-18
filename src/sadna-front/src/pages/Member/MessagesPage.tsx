import React, { useEffect, useState } from 'react';
import { ListGroup, Button } from 'react-bootstrap';
import { handleGetUserNotifications, handleMarkNotificationAsRead } from '../../actions/MemberActions.tsx';
import { Notification } from '../../models/Notification.tsx';
import Exit from '../Exit.tsx';

function MessagesPage(props) {
  const [messages, setMessages] = useState<Notification[]>([]);

  
  const getUserNotifications = ()=>{
    handleGetUserNotifications(props.id).then(
      value => {
        console.log(value);
        
        setMessages(value as Notification[]);
      })
      .catch(error => alert(error));
    }

    const MarkNotificationAsRead = (message)=>{
      handleMarkNotificationAsRead(props.id, message.notificationID).then(
        value => {
          getUserNotifications();
        })
        .catch(error => alert(error));
      }
    
    function formatDate(dateString: string): string {
      const date = new Date(dateString);
      date.setHours(date.getHours()+3)
      const options: Intl.DateTimeFormatOptions = {
        year: "numeric",
        month: "long",
        day: "numeric",
        hour: "numeric",
        minute: "numeric",
        second: "numeric",
        timeZone: "UTC",
      };
      return date.toLocaleString(undefined, options);
    }
      
      


  useEffect(() => {
    getUserNotifications();
  }, []);

  return (
    <div className="container">
      <Exit id={props.id}/>
      <h1>My Notifications</h1>
      <ListGroup>
        {messages.map((message, index) => (
          <ListGroup.Item key={index} variant={message.read ? "light" : "warning"}>
            <div className="d-flex justify-content-between">
              <div>{message.message}</div>
              <div>{formatDate(message.time)}</div>
            </div>
            {!message.read && (
              <Button variant="primary" size="sm" onClick={() => MarkNotificationAsRead(message)}>
                Mark as Read
              </Button>
            )}
          </ListGroup.Item>
        ))}
      </ListGroup>
    </div>
  );
}

export default MessagesPage;
