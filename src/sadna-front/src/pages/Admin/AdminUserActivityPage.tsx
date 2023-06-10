import React, { useState } from 'react';
import { Bar } from 'react-chartjs-2';
import { Button, Container, Form } from 'react-bootstrap';
import {Chart as ChartJS} from 'chart.js/auto'
import {CategoryScale, LinearScale} from 'chart.js'
import { handleGetSystemUserData } from '../../actions/AdminActions.tsx';

ChartJS.register(
  CategoryScale,
  LinearScale
);



const AdminUserActivityPage = (props) => {
  const [data, setData] = useState<number[]>([1,2,3,4,5]);
  const [chart, setChart] = useState({
    labels: ['Guest', 'Member', 'Store Managers', 'Store Owners', 'System Managers'],
    datasets: [
      {
        label: 'User Types',
        data: [10, 15, 5, 8, 3],
        backgroundColor: 'rgba(75,192,192,0.6)',
        borderColor: 'rgba(75,192,192,1)',
        borderWidth: 1,
      },
    ],
  });
  const [selectedDate, setSelectedDate] = useState('');

  const handleDateChange = (event) => {
    setSelectedDate(event.target.value);
  };

  const handleRequest = () => {
    handleGetSystemUserData(props.id, selectedDate).then(
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

  return (
    <Container>
      <h1>User Activity</h1>
      <Form.Group controlId="formDate">
        <Form.Label>Select Date:</Form.Label>
        <Form.Control type="date" value={selectedDate} onChange={handleDateChange} />
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
