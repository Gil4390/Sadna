import React, { useState } from 'react';
import { Form, Button, Card } from 'react-bootstrap';

function LoginPage() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const handleEmailChange = (event) => {
    setEmail(event.target.value);
  };

  const handlePasswordChange = (event) => {
    setPassword(event.target.value);
  };

  const handleLoginSubmit = (event) => {
    event.preventDefault();
    console.log(`Email: ${email}, Password: ${password}`);
    // do something with login credentials
  };

  return (
    <Card style={{ maxWidth: '500px', margin: 'auto' }}>
      <Card.Body>
        <Card.Title className="text-center">Welcome Back!</Card.Title>
        <Form onSubmit={handleLoginSubmit}>
          <Form.Group controlId="formBasicEmail" style={{margin: "15px"}}>
            <Form.Control type="email" placeholder="Enter email" value={email} onChange={handleEmailChange} />
          </Form.Group>

          <Form.Group controlId="formBasicPassword" style={{margin: "15px"}}>
            <Form.Control type="password" placeholder="Password" value={password} onChange={handlePasswordChange} />
          </Form.Group>

          <div className="text-center">
            <Button variant="dark" type="submit">
              Log In
            </Button>
          </div>
          
        </Form>
      </Card.Body>
    </Card>
  );
}

export default LoginPage;
