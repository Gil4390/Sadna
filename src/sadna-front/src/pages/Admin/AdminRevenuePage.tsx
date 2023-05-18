import React, { useState } from 'react';
import { Button, Form, FormControl, InputGroup } from 'react-bootstrap';

function AdminRevenuePage(props) {
  const [selectedDate, setSelectedDate] = useState('');
  const [revenueAmount, setRevenueAmount] = useState('');

  const handleDateChange = (event) => {
    setSelectedDate(event.target.value);
  };

  const handleRequest = () => {

    setRevenueAmount("$1,234.56");
  };

  return (
    <div className="container">
      <h1>System Revenue</h1>

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

      {revenueAmount && (
        <div>
          <h2>Revenue Amount:</h2>
          <p>{revenueAmount}</p>
        </div>
      )}
    </div>
  );
}

export default AdminRevenuePage;
