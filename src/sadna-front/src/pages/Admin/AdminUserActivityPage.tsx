import React, { useEffect, useState } from 'react';
import { Bar } from 'react-chartjs-2';
import { Button, Container, Form, Row, Col } from 'react-bootstrap';
import {Chart as ChartJS} from 'chart.js/auto'
import {CategoryScale, LinearScale} from 'chart.js'
import { handleGetSystemUserData } from '../../actions/AdminActions.tsx'
import useSignalRNotifications from '../../hooks/signalR/useSignalRNotifications.ts';import Exit from '../Exit.tsx';
;

ChartJS.register(
  CategoryScale,
  LinearScale
);



const AdminUserActivityPage = (props) => {
  const [chart, setChart] = useState({
    labels: ['Guest', 'Member', 'Store Managers', 'Store Owners', 'System Managers'],
    datasets: [
      {
        label: 'User Types',
        data: [0,0,0,0,0],
        backgroundColor: 'rgba(75,192,192,0.6)',
        borderColor: 'rgba(75,192,192,1)',
        borderWidth: 1,
      },
    ],
  });
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');
  const todayForDefault = new Date().toISOString().split('T')[0]; // Format: YYYY-MM-DD

  const handleFromDateChange = (event) => {
    setFromDate(event.target.value);
  };
  const handleToDateChange = (event) => {
    setToDate(event.target.value);
  };

  const handleRequest = () => {
    handleGetSystemUserData(props.id, fromDate, toDate).then(
      value => {
        setChart((prevChartData) => ({
          ...prevChartData,
          datasets: [
            {
              ...prevChartData.datasets[0],
              data: value as number[],
            },
          ],
        }));
      }
    ).catch(error => alert(error));
  };

  useSignalRNotifications('SystemActivityHub', {
    ['SystemActivityUpdated']:()=>{
      handleRequest();
   },
 });

  return (
    <Container style={{ paddingBottom: "4rem" }}>
      <Exit id={props.id} />
      <h1>User Activity</h1>
      <Form.Group controlId="formDate">
        <Row>
          <Form.Label column sm="2">
            From date:
          </Form.Label>
          <Col sm="6">
            <Form.Control type="date" value={fromDate} onChange={handleFromDateChange} />
          </Col>
        </Row>
        <Row>
          <Form.Label column sm="2">
            To date:
          </Form.Label>
          <Col sm="6">
            <Form.Control type="date" value={toDate} onChange={handleToDateChange} />
          </Col>
        </Row>
      </Form.Group>

      <Button variant="primary" onClick={handleRequest}>
        Send Request
      </Button>

      <div className="mt-4">
        <Bar data={chart} />
      </div>
    </Container>

  );
};

export default AdminUserActivityPage;
