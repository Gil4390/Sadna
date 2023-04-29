import React from 'react';
import { Container, Row, Col, Card, Button } from 'react-bootstrap';
import { Store } from '../../components/Store';


function StoresManagementPage() {

  const stores = [
    { id: 1, name: 'Store A'},
    { id: 2, name: 'Store B'},
    { id: 3, name: 'Store C'},
  ];

  const handleEditStore = (event) => {
    
  };
  const handleDeleteStore = (event) => {
    
  };

  return (
    <Container className="my-5">
      <h1>Stores Management</h1>
      <Row className="my-3">
        <Col>
          <Button variant="primary">Add New Store</Button>
        </Col>
      </Row>
      <Row>
        {stores.map((store) => (
          <Col xs={12} md={6} lg={4} key={store.id} className="my-3">
            <Card>
              <Card.Body>
                <Store store={store} />
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
    </Container>
  );
}

export default StoresManagementPage;
