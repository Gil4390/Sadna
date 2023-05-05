import React, { useState, useEffect } from 'react';
import { Form, Button, Card } from 'react-bootstrap';
import SystemNotInit from '../SystemNotInit.tsx';
import { handleRegister } from '../../actions/GuestActions.tsx';
import {Response} from '../../models/Response.tsx';

function RegisterPage(props) {
 
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [response, setResponse] = useState<Response>();
  const [message, setMessage] = useState<string>('');
  
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

  const RegisterSuccess = () =>{
    setFirstName("");
    setLastName("");
    setEmail("");
    setPassword("");
    setConfirmPassword("");
    setMessage("Registration completed successfully!");
  }

//   useEffect(() => {
//     if(message!='')
//       alert(message);
//  }, [message])

  useEffect(() => {
    if(response !=undefined){
      console.log("err? "+response?.errorOccured);
      console.log("errmsg? "+response?.errorMessage);
      response?.errorOccured ? setMessage(response?.errorMessage) : RegisterSuccess();
    }
 }, [response])

  const handleRegisterSubmit = (event) => {
    event.preventDefault();
    console.log(`First Name: ${firstName}, Last Name: ${lastName}, Email: ${email}, Password: ${password}, Confirm Password: ${confirmPassword}`);
     // check if two string are in the same accent example 'a' != 'á' and check if they lower&upper case match example 'AaA' != 'aAa'
     if(password.localeCompare(confirmPassword, undefined, {sensitivity: 'accent'}) === 0 && password === confirmPassword)
     {
     handleRegister(props.id,email,firstName,lastName,password).then(
       value => {
         setResponse(value as Response);
       })
       .catch(error => alert(error));
     }
     else
     {
       alert("Password and Confirm Password do not match");
     }
    
    // do something with registration data
  };

  return (
    props.isInit?(
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

          <div className="text-center">
            {message}
          </div>
        </Form>
      </Card.Body>
    </Card>)
    : (<SystemNotInit/>)
  );
}

export default RegisterPage;
