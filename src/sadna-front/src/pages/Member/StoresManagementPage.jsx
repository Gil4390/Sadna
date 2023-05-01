import React, {useState} from 'react';
import { Container, Row, Col, Card, Button, Form, Modal } from 'react-bootstrap';
import { Store } from '../../components/Store';

const stores = [
  { id: 1, name: 'Store A'},
  { id: 2, name: 'Store B'},
  { id: 3, name: 'Store C'},
];

function StoresManagementPage() {


  const [showAddModal, setShowAddModal] = useState(false);
  const [storesList, setstoresList] = useState(stores);

  const handleAddModalClose = () => setShowAddModal(false);
  const handleAddModalShow = () => setShowAddModal(true);
  

  const handleAddStore = (event) => {
    event.preventDefault();
    const formData = new FormData(event.target);
    const newStore = {
      id: Math.max(...stores.map((store) => store.id)) + 1,
      name: formData.get('name'),
    };
    setstoresList([...storesList, newStore]);
    setShowAddModal(false);
  };


  const handleDeleteStore = (event) => {
    
  };

  return (
    <Container className="my-5">
      <h1>Stores Management</h1>
      <Row className="my-3">
        <Col>
          <Button variant="primary" onClick={handleAddModalShow}>
            Create New Store
          </Button>
          <Modal show={showAddModal} onHide={handleAddModalClose}>
            <Modal.Header closeButton>
              <h5 class="modal-title">Create New Store</h5>
            </Modal.Header>
            <Modal.Body>
              <Form onSubmit={handleAddStore}>
                <Form.Group controlId="Store name">
                  <Form.Label>Name</Form.Label>
                  <Form.Control type="text" placeholder="Enter name" />
                </Form.Group>
                <Button variant="primary" type="submit" style={{margin: "0.5rem"}}>
                  Add
                </Button>
              </Form>
            </Modal.Body>
          </Modal>
        </Col>
      </Row>
      <Row>
        {storesList.map((store) => (
          <Col xs={12} md={6} lg={4} key={store.id} className="my-3">
            <Card>
              <Card.Body>
                <Store name={store.name} />
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
    </Container>
  );
}

export default StoresManagementPage;
