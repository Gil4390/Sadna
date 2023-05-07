import React, { useState } from 'react';
import { ListGroup, Button } from 'react-bootstrap';

const exampleMessages = [
  { time: '2023-05-06 10:00:00', message: 'Hello, how are you?', read: false },
  { time: '2023-05-05 15:30:00', message: 'Can you please confirm your order?', read: false },
  { time: '2023-05-03 11:45:00', message: 'Your package has been shipped!', read: false },
];

function MessagesPage() {
  const [messages, setMessages] = useState(exampleMessages);

  function handleMarkAsRead(index) {
    const newMessages = [...messages];
    newMessages[index].read = true;
    setMessages(newMessages);
  }

  return (
    <div className="container">
      <h1>My Messages</h1>
      <ListGroup>
        {messages.map((message, index) => (
          <ListGroup.Item key={index} variant={message.read ? "light" : "warning"}>
            <div className="d-flex justify-content-between">
              <div>{message.message}</div>
              <div>{message.time}</div>
            </div>
            {!message.read && (
              <Button variant="primary" size="sm" onClick={() => handleMarkAsRead(index)}>
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
