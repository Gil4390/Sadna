import React, { useState, useEffect } from 'react';
import { Form, Button, Card } from 'react-bootstrap';
import SystemNotInit from '../SystemNotInit.tsx';
import { handleRegister } from '../../actions/GuestActions.tsx';
import {Response} from '../../models/Response.tsx';
import Exit from "../Exit.tsx";


function RegisterPage(props) {
 
  const [firstName, setFirstName] = useState('');
  const [firstNameError, setFirstNameError] = useState('');
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

  useEffect(() => {
    if(response !=undefined){
      response?.errorOccured ? setMessage(response?.errorMessage) : RegisterSuccess();
    }
 }, [response])

  const isValidFirstOrLastName = (name) => {
    if(name.length < 3)
      return false;
    const nameRegex = /^[a-zA-Z]+$/;
    return nameRegex.test(name);
  }

  const getPasswordStrength = (password) => {
    const passwordRegexes = [
      /[A-Z]/, // Uppercase
      /[a-z]/, //lowercase
      /\d/, //digits
      /[^A-Za-z\d]/, //special characters
      /.{8,}/, // min length of 8 chars
    ];
    const strength = passwordRegexes.reduce((count, regex) => {
      return count + Number(regex.test(password));
    }, 0);

    return strength;
  }

  const isValidPassword = (password) => {
    if(getPasswordStrength(password) >= 5)
      return true;
    return false;
  }

  const getPasswordStrengthLevel = (password) => {
    const strength = getPasswordStrength(password);
    if(strength >= 5)
      return 'Strong Password';
    else if(strength >= 3)
      return 'Moderate Password'
    return 'Weak Password'
  }

  const isValidEmail = (email) => {
    // regex for email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    // test email againt regex pattern
    return emailRegex.test(email);
  }

  const isValidPasswordConfirmPassword = (password, confirmPassword) => {
    return password.localeCompare(confirmPassword, undefined, {sensitivity: 'accent'}) === 0 && password === confirmPassword;
  }

  const handleRegisterSubmit = (event) => {
    event.preventDefault();
    console.log(`First Name: ${firstName}, Last Name: ${lastName}, Email: ${email}, Password: ${password}, Confirm Password: ${confirmPassword}`);
    

    if(!isValidFirstOrLastName(firstName) || !isValidFirstOrLastName(lastName) || !isValidEmail(email) || !isValidPassword(password) 
     || !isValidPasswordConfirmPassword(password, confirmPassword) )
    {
      setMessage("Make Sure All Field Are Valid And Then Try Again!");
    }
    else
    {
        handleRegister(props.id,email,firstName,lastName,password).then(
        value => {
          setResponse(value as Response);
        })
        .catch(error => alert(error));
    }
    
    

    // do something with registration data
  };

  return (
    props.isInit?(
    <Card style={{ maxWidth: '500px', margin: 'auto' }}>
      <Exit id={props.id}/>
      <Card.Body>
        <Card.Title className="text-center">Register Now!</Card.Title>
        <Form onSubmit={handleRegisterSubmit}>
          <Form.Group controlId="formBasicFirstName" style={{margin: "15px"}}>
            <Form.Control type="text" placeholder="Enter first name" value={firstName} onChange={handleFirstNameChange} 
            style={{borderColor: isValidFirstOrLastName(firstName) || firstName.length === 0 ? '#28a745' : '#dc3534'}} />
            
            { !isValidFirstOrLastName(firstName) && firstName.length > 0 && <Form.Text className='text-danger'>First Name can only contain and min 3 letters! Try Again!</Form.Text>}
          </Form.Group>

          <Form.Group controlId="formBasicLastName" style={{margin: "15px"}}>
            <Form.Control type="text" placeholder="Enter last name" value={lastName} onChange={handleLastNameChange}  
            style={{borderColor: isValidFirstOrLastName(lastName) || lastName.length === 0 ? '#28a745' : '#dc3534'}} />
            
            {!isValidFirstOrLastName(lastName) && lastName.length > 0 && <Form.Text className='text-danger'>Last Name can only contain and min 3 letters! Try Again!</Form.Text>}
          </Form.Group>

          <Form.Group controlId="formBasicEmail" style={{margin: "15px"}}>
            <Form.Control type="email" placeholder="Enter email" value={email} onChange={handleEmailChange}  
            style={{borderColor: isValidEmail(email) || email.length === 0 ? '#28a745' : '#dc3534'}} />
            {!isValidEmail(email) && email.length > 0 && <Form.Text className='text-danger'>No Valid Email! Try Again!</Form.Text>}
          </Form.Group>


          <Form.Group controlId="formBasicPassword" style={{margin: "15px"}}>
            <Form.Control type="password" placeholder="Password" value={password} onChange={handlePasswordChange}  
            style={{borderColor: isValidPassword(password) || password.length === 0  ? '#28a745' : '#dc3534'}} />
            {<Form.Text style={{fontWeight: 'bold'}}>{getPasswordStrengthLevel(password)}</Form.Text>}
            {!isValidPassword(password) && password.length > 0 && <Form.Text className='text-danger'>Password should be at least 8 chars and min 1 (Uppercase,lower,digit,special char)! Try Again!</Form.Text>}
          </Form.Group>
   
             
          
          <Form.Group controlId="formBasicConfirmPassword" style={{margin: "15px"}}>
            <Form.Control type="password" placeholder="Confirm Password" value={confirmPassword} onChange={handleConfirmPasswordChange}   
            style={{borderColor: isValidPasswordConfirmPassword(password,confirmPassword) || confirmPassword.length === 0  ? '#28a745' : '#dc3534'}} />
            {!isValidPasswordConfirmPassword(password,confirmPassword) && confirmPassword.length > 0 && <Form.Text className='text-danger'>Password and Confirm Password do not match! Try Again!</Form.Text>}
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