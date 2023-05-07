import React, { useState, useEffect } from 'react';
import { Container, Form, Button, Row, Col } from 'react-bootstrap';
import { useNavigate, useLocation } from "react-router-dom";
import { Response } from '../models/Response.tsx';
import { handlePurchaseCart } from '../actions/GuestActions.tsx';
import Exit from "./Exit.tsx";

function PaymentPage(props) {

  const navigate = useNavigate();
  const location = useLocation();
  const id = location.state.id;

  const [cardNumber, setCardNumber] = useState('');
  const [cardHolderName, setCardHolderName] = useState('');
  const [expiryDate, setExpiryDate] = useState('');
  const [cvv, setCvv] = useState('');
  const [city, setCity] = useState('');
  const [adress, setAdress] = useState('');
  const [message, setMessage] = useState<string>('');
  const [response, setResponse] = useState<Response>();

  const handleCardNumberChange = (event) => {
    setCardNumber(event.target.value);
  };

  const handleCardHolderNameChange = (event) => {
    setCardHolderName(event.target.value);
  };

  const handleExpiryDateChange = (event) => {
    setExpiryDate(event.target.value);
  };

  const handleCvvChange = (event) => {
    setCvv(event.target.value);
  };

  const handleCityChange = (event) => {
    setCity(event.target.value);
  };

  const handleAdressChange = (event) => {
    setAdress(event.target.value);
  };

  const PurchaseSuccess = () =>{
    setCardNumber("");
    setCardHolderName("");
    setExpiryDate("");
    setCvv("");
    setCity("");
    setAdress("");
    setMessage("Purchase completed successfully!");
  }

  const isCardNumberValid = (cardNumber) =>
  {
    const cardPattern = /^(?:4[0-9]{12}(?:[0-9]{3})?|5[1-5][0-9]{14}|6(?:011|5[0-9][0-9])[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|(?:2131|1800|35\d{3})\d{11})$/;
    return cardPattern.test(cardNumber);
  }

  const isValidCardHolderName = (name) =>
  {
    const holderPattern = /^[A-Za-z]+\s[A-Za-z]+$/;
    return holderPattern.test(name);
  }

  const isValidExpiryDate = (date) =>
  {
    const expiryPattern = /^(0[1-9]|1[0-2])\/([0-9]){2}$/;
    if(!expiryPattern.test(date))
      return false;
    
    const dateParts = date.split("/");
    const expiryMonth = Number(dateParts[0]);
    const expiryYear = Number(dateParts[1]);

    const todayMonth = new Date().getMonth() + 1;
    const todayYear = new Date().getFullYear() - 2000;

    return expiryMonth >= todayMonth && expiryYear >= todayYear || expiryYear > todayYear;

  }

  const isValidCVV = (cvv) =>
  {
    const cvvPattern = /^[0-9]{3}$/;
    return cvvPattern.test(cvv);
  }

  const isValidCity = (city) =>
  {
    const cityPattern = /^[A-Za-z ]+$/;
    return cityPattern.test(city);
  }

  const isValidAddress = (address) =>
  {
    const addressPattern = /^[a-zA-Z0-9\s,'-']*$/;
    return addressPattern.test(address);
  }

  useEffect(() => {
    if(response !=undefined){
      console.log("err? "+response?.errorOccured);
      console.log("errmsg? "+response?.errorMessage);
      response?.errorOccured ? setMessage(response?.errorMessage) : PurchaseSuccess();
    }
 }, [response])

  const handleSubmit = (event) => {
    event.preventDefault();
    console.log(`Card Number: ${cardNumber}, Card Holder Name: ${cardHolderName}  Expiration Date: ${expiryDate}  
    cvv: ${cvv} City: ${city}  Adress: ${adress} `);
    const paymentDetails = `${cardNumber} ${cardHolderName} ${expiryDate} ${cvv}`;
    const usersDetails = `${city} ${adress}`;

    if(isCardNumberValid(cardNumber) && isValidCardHolderName(cardHolderName) && isValidExpiryDate(expiryDate) && isValidCVV(cvv) && isValidCity(city) && isValidAddress(adress))
    {
      handlePurchaseCart(id, paymentDetails, usersDetails).then(
        value => {
          setResponse(value as Response);
        })
        .catch(error => alert(error));
    }
    else
    {
      setMessage("Payment Failed, Make Sure All fields are Valid!");
    }

  };

  return (
    <Container className="my-5">
            <Exit id={props.id}/>
      <h1>Payment</h1>
      <Form>
        <Form.Group controlId="cardNumber">
          <Form.Label>Card Number</Form.Label>
          <Form.Control
            type="text"
            placeholder="Enter card number"
            value={cardNumber}
            onChange={handleCardNumberChange}
            style={{borderColor: isCardNumberValid(cardNumber) || cardNumber.length === 0 ? '#28a745' : '#dc3534'}} />
            {!isCardNumberValid(cardNumber) && cardNumber.length > 0 && <Form.Text className='text-danger'>Not Valid Card Number!</Form.Text>}
        </Form.Group>
        <Form.Group controlId="cardHolderName">
          <Form.Label>Card Holder Name</Form.Label>
          <Form.Control
            type="text"
            placeholder="Enter card holder name"
            value={cardHolderName}
            onChange={handleCardHolderNameChange}
            style={{borderColor: isValidCardHolderName(cardHolderName) || cardHolderName.length === 0 ? '#28a745' : '#dc3534'}} />
            {!isValidCardHolderName(cardHolderName) && cardHolderName.length > 0 && <Form.Text className='text-danger'>Not Valid Name!</Form.Text>}
        </Form.Group>
        <Row>
          <Col>
            <Form.Group controlId="expiryDate">
              <Form.Label>Expiration Date</Form.Label>
              <Form.Control
                type="text"
                placeholder="MM/YY"
                value={expiryDate}
                onChange={handleExpiryDateChange}
                style={{borderColor: isValidExpiryDate(expiryDate) || expiryDate.length === 0 ? '#28a745' : '#dc3534'}} />
                {!isValidExpiryDate(expiryDate) && expiryDate.length > 0 && <Form.Text className='text-danger'>Not Valid expiry date!</Form.Text>}
            </Form.Group>
          </Col>
          <Col>
            <Form.Group controlId="cvv">
              <Form.Label>CVV</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter CVV"
                value={cvv}
                onChange={handleCvvChange}
                style={{borderColor: isValidCVV(cvv) || cvv.length === 0 ? '#28a745' : '#dc3534'}} />
                {!isValidCVV(cvv) && cvv.length > 0 && <Form.Text className='text-danger'>Not Valid CVV!</Form.Text>}
            </Form.Group>
          </Col>
        </Row>

        <Row>
          <Col>
            <Form.Group controlId="city">
              <Form.Label>City</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter City"
                value={city}
                onChange={handleCityChange}
                style={{borderColor: isValidCity(city) || city.length === 0 ? '#28a745' : '#dc3534'}} />
                {!isValidCity(city) && city.length > 0 && <Form.Text className='text-danger'>Not Valid City!</Form.Text>}
            </Form.Group>
          </Col>
          <Col>
            <Form.Group controlId="adress">
              <Form.Label>Adress</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter Adress"
                value={adress}
                onChange={handleAdressChange}
                style={{borderColor: isValidAddress(adress) || adress.length === 0 ? '#28a745' : '#dc3534'}} />
                {!isValidAddress(adress) && adress.length > 0 && <Form.Text className='text-danger'>Not Valid Address!</Form.Text>}
            </Form.Group>
          </Col>
        </Row>
        <div className="text-center">
          <Button variant="success" type="submit" size="lg" style={{margin: "15px"}} onClick={handleSubmit} >
            Submit Payment
          </Button>
          <Button variant="secondary" size="lg" onClick={() => navigate("/CartPage")} >
            Cancel
          </Button>
        </div>
        <div className="text-center">
            {message}
          </div>
      </Form>
    </Container>
  );
}

export default PaymentPage;
