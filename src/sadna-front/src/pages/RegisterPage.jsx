import React, { useState } from 'react';
import { Form, Button, Card } from 'react-bootstrap';

function RegisterPage() {
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const handleFirstNameChange = (event) => {
    setFirstName(event.target.value);
  };

  const handleLastNameChange = (event) => {
    setLastName(event.target.value);
  };

  const handleEmailChange = (event) => {
    setEmail(event.target.value);
  };

  const handlePasswordChange = (event) => {
    setPassword(event.target.value);
  };

  const handleConfirmPasswordChange = (event) => {
    setConfirmPassword(event.target.value);
  };

  const handleRegisterSubmit = (event) => {
    event.preventDefault();
    console.log(`First Name: ${firstName}, Last Name: ${lastName}, Email: ${email}, Password: ${password}, Confirm Password: ${confirmPassword}`);
    // do something with registration data
  };

  return (
    <Card style={{ maxWidth: '500px', margin: 'auto' }}>
      <Card.Body>
        <Card.Title className="text-center">Register Now!</Card.Title>
        <Form onSubmit={handleRegisterSubmit}>
          <Form.Group controlId="formBasicFirstName" style={{margin: "15px"}}>
            <Form.Control type="text" placeholder="Enter first name" value={firstName} onChange={handleFirstNameChange} />
          </Form.Group>

          <Form.Group controlId="formBasicLastName" style={{margin: "15px"}}>
            <Form.Control type="text" placeholder="Enter last name" value={lastName} onChange={handleLastNameChange} />
          </Form.Group>

          <Form.Group controlId="formBasicEmail" style={{margin: "15px"}}>
            <Form.Control type="email" placeholder="Enter email" value={email} onChange={handleEmailChange} />
          </Form.Group>

          <Form.Group controlId="formBasicPassword" style={{margin: "15px"}}>
            <Form.Control type="password" placeholder="Password" value={password} onChange={handlePasswordChange} />
          </Form.Group>

          <Form.Group controlId="formBasicConfirmPassword" style={{margin: "15px"}}>
            <Form.Control type="password" placeholder="Confirm Password" value={confirmPassword} onChange={handleConfirmPasswordChange} />
          </Form.Group>

          <div className="text-center">
            <Button variant="dark" type="submit">
              Submit
            </Button>
          </div>
        </Form>
      </Card.Body>
    </Card>
  );
}

export default RegisterPage;
