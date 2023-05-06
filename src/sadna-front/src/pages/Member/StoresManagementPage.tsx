import React, {useState, useEffect} from 'react';
import { Container, Row, Col, Card, Button, Form, Modal } from 'react-bootstrap';
import { Store } from '../../components/Store.tsx';
import { handleGetMemberPermissions } from '../../actions/MemberActions.tsx';
import Exit from '../Exit.tsx';



const stores = [
  { id: 1, name: 'Store A', isOpne: true},
  { id: 2, name: 'Store B', isOpne: true},
  { id: 3, name: 'Store C', isOpne: true},
];

function StoresManagementPage(props) {
  const [permissions, setPermissions] = useState<Permissions>();

  const getPermissions =()=>{
    handleGetMemberPermissions(props.id).then(
      value => {
        setPermissions(value as Permissions);
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    for (const key in permissions) {
      console.log(`Permissions for key ${key}:`);
      const permissionList = permissions[key];
      console.log(permissionList);
    }
   }, [permissions])

  useEffect(() => {
    console.log(props.id);
    getPermissions();
  }, [])


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
      isOpne: true,
    };
    //setstoresList([...storesList, newStore]);
    setShowAddModal(false);
  };


  const handleDeleteStore = (event) => {
    
  };

  return (
  <div>
    <Container className="my-5">
      <h1>Stores Management</h1>
      <Row className="my-3">
        <Col>
          <Button variant="primary" onClick={handleAddModalShow}>
            Create New Store
          </Button>
          <Modal show={showAddModal} onHide={handleAddModalClose}>
            <Modal.Header closeButton>
              <h5 className="modal-title">Create New Store</h5>
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
                <Store store={store} />
              </Card.Body>
            </Card>
          </Col>
        ))}
      </Row>
    </Container>
    </div>
  );
}

export default StoresManagementPage;
