import React, { useState,useEffect } from 'react';
import { Form, Button, Card } from 'react-bootstrap';
import { useNavigate, useLocation } from "react-router-dom";
import {ResponseT} from '../../models/Response.tsx';
import { handleIsAdmin, handleLogin } from '../../actions/GuestActions.tsx';
import Exit from "../Exit.tsx";


function LoginPage(props) {
  const navigate = useNavigate();
  const [localId, setLocalId] = useState(props.id);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const [response, setResponse] = useState<ResponseT>();
  const [message, setMessage] = useState<string>('');

  const handleEmailChange = (event) => {
    setEmail(event.target.value);
  };

  const handlePasswordChange = (event) => {
    setPassword(event.target.value);
  };

  const LoginSuccess = () =>{
    setEmail("");
    setPassword("");
    setMessage("Login completed successfully!");
    setLocalId(response?.value);
    props.onIdChange(response?.value);
  }

  useEffect(() => {
    if(response !=undefined){
      response?.errorOccured ? setMessage(response?.errorMessage) : LoginSuccess();
    }
 }, [response])


 const isValidEmail = (email) => {
  // regex for email validation
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  // test email againt regex pattern
  return emailRegex.test(email);
}

  const handleLoginSubmit = (event) => {
    event.preventDefault();
    console.log(`Email: ${email}, Password: ${password}`);
    
    if(!isValidEmail(email))
    {
      setMessage("Not Valid Email! Try Again!");
    }
    else
    {
      handleLogin(props.id,email,password).then(
        value => {
          setResponse(value as ResponseT);
          //navigate("/");
        })
        .catch(error => alert(error));
    }
    // do something with login credentials
  };

  return (
    <Card style={{ maxWidth: '500px', margin: 'auto' }}>
      <Exit id={props.id}/>
      <Card.Body>
        <Card.Title className="text-center">Welcome Back!</Card.Title>
        <Form onSubmit={handleLoginSubmit}>
          <Form.Group controlId="formBasicEmail" style={{margin: "15px"}}>
            <Form.Control type="email" placeholder="Enter email" value={email} onChange={handleEmailChange}   
            style={{borderColor: isValidEmail(email) || email.length === 0 ? '#28a745' : '#dc3534'}} />
            {!isValidEmail(email) && email.length > 0 && <Form.Text className='text-danger'>No Valid Email! Try Again!</Form.Text>}
          </Form.Group>

          <Form.Group controlId="formBasicPassword" style={{margin: "15px"}}>
            <Form.Control type="password" placeholder="Password" value={password} onChange={handlePasswordChange} />
          </Form.Group>

          <div className="text-center">
            <Button variant="dark" type="submit">
              Log In
            </Button>
          </div>
          <div className="text-center">
            {message}
          </div>
        </Form>
      </Card.Body>
    </Card>
  );
}

export default LoginPage;
