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
    handlePurchaseCart(id, paymentDetails, usersDetails).then(
      value => {
        setResponse(value as Response);
      })
      .catch(error => alert(error));
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
          />
        </Form.Group>
        <Form.Group controlId="cardHolderName">
          <Form.Label>Card Holder Name</Form.Label>
          <Form.Control
            type="text"
            placeholder="Enter card holder name"
            value={cardHolderName}
            onChange={handleCardHolderNameChange}
          />
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
              />
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
              />
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
              />
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
              />
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
