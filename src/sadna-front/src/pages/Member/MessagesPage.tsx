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
              <div>{message.time}</div>
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
