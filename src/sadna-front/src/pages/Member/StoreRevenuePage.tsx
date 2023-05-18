import React, { useState } from 'react';
import { Button, Form, FormControl, InputGroup } from 'react-bootstrap';
import { handleGetStoreRevenue } from '../../actions/MemberActions.tsx';
import Exit from '../Exit.tsx';

function StoreRevenuePage(props) {
  const [selectedDate, setSelectedDate] = useState('');
  const [revenueAmount, setRevenueAmount] = useState(0);

  const handleDateChange = (event) => {
    setSelectedDate(event.target.value);
  };

  const handleRequest = () => {
    handleGetStoreRevenue(props.id, props.storeId, selectedDate).then(
      value => {
        setRevenueAmount(value as number);
      }
    ).catch(error => alert(error));
  };

  return (
    <div className="container">
      <Exit id={props.id}/>
      <h1>Store Revenue</h1>

      <Form>
        <Form.Group controlId="dateSelect">
          <Form.Label>Select a Date</Form.Label>
          <InputGroup style={{width: "350px"}}>
            <FormControl
              type="date"
              value={selectedDate}
              onChange={handleDateChange}
            />
            <Button variant="primary" onClick={handleRequest}>
              Get Revenue
            </Button>
          </InputGroup>
        </Form.Group>
      </Form>

      <div>
        <h2>Revenue Amount:</h2>
        <p>{revenueAmount} $</p>
      </div>
    </div>
  );
}

export default StoreRevenuePage;
