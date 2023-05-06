import React, { useState,useEffect } from 'react';
import { Form, Button, Card } from 'react-bootstrap';
import {ResponseT} from '../../models/Response.tsx';
<<<<<<< Updated upstream
import { handleIsAdmin, handleLogin } from '../../actions/GuestActions.tsx';
=======
import { handleLogin } from '../../actions/GuestActions.tsx';
>>>>>>> Stashed changes
import Exit from "../Exit.tsx";


function LoginPage(props) {

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
      console.log("err? "+response?.errorOccured);
      console.log("errmsg? "+response?.errorMessage);
      response?.errorOccured ? setMessage(response?.errorMessage) : LoginSuccess();
    }
 }, [response])

  const handleLoginSubmit = (event) => {
    event.preventDefault();
    console.log(`Email: ${email}, Password: ${password}`);

    handleLogin(props.id,email,password).then(
      value => {
        setResponse(value as ResponseT);
      })
      .catch(error => alert(error));
    
    // do something with login credentials
  };

  return (
    <Card style={{ maxWidth: '500px', margin: 'auto' }}>
      <Exit id={props.id}/>
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
          <div className="text-center">
            {message}
          </div>
        </Form>
      </Card.Body>
    </Card>
  );
}

export default LoginPage;
