import React, { useState } from 'react';
import { Button, Form, FormControl, InputGroup } from 'react-bootstrap';
import { handleGetSystemRevenue } from '../../actions/AdminActions.tsx';
import Exit from '../Exit.tsx';

function AdminRevenuePage(props) {
  const [selectedDate, setSelectedDate] = useState('');
  const [revenueAmount, setRevenueAmount] = useState(0);

  const handleDateChange = (event) => {
    setSelectedDate(event.target.value);
  };

  const handleRequest = () => {
    handleGetSystemRevenue(props.id, selectedDate).then(
      value => {
        setRevenueAmount(value as number);
      }
    ).catch(error => alert(error));
  };

  return (
    <div className="container">
      <Exit id={props.id}/>
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

      <div>
        <h2>Revenue Amount:</h2>
        <p>{revenueAmount} $</p>
      </div>
    </div>
  );
}

export default AdminRevenuePage;
