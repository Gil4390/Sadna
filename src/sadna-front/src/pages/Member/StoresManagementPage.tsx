import React, {useState, useEffect} from 'react';
import { Container, Row, Col, Card, Button, Form, Modal } from 'react-bootstrap';
import { StoreInfo } from '../../components/StoreInfo.tsx';
import { handleGetMemberPermissions, handleOpenNewStore } from '../../actions/MemberActions.tsx';
import Exit from '../Exit.tsx';
import {PermissionPerStore} from '../../models/Permission.tsx';
import { ResponseT } from '../../models/Response.tsx';


function StoresManagementPage(props) {
  const [permissions, setPermissions] = useState<PermissionPerStore>({});
  const [showAddModal, setShowAddModal] = useState(false);
  const [storesIdList, setStoresIdList] = useState<string[]>([]);
  const [storeName, setStoreName] = useState<string>('');
  const [addStoreResponse, setAddStoreResponse]=useState<ResponseT>();

  const [refreshStores, setRefreshStores]=useState(0);
  
  const handleAddModalClose = () => setShowAddModal(false);
  const handleAddModalShow = () => setShowAddModal(true);


  const getPermissions =()=>{
    handleGetMemberPermissions(props.id).then(
      value => {
        setPermissions(value as PermissionPerStore);
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    for (const key in permissions) {
      console.log(`Permissions for key ${key}:`);
      const permissionList = permissions[key];
      console.log(permissionList);
    }
    setStoresIdList(Object.keys(permissions));
   }, [permissions,refreshStores])

   useEffect(() => {
    getPermissions();
  }, [])

  useEffect(() => {
    if(addStoreResponse!=undefined)
      if(addStoreResponse?.errorOccured)
        alert(addStoreResponse?.errorMessage) 
      else{
        setShowAddModal(false);
        setStoreName('');
        getPermissions();
      }
      setAddStoreResponse(undefined);
  }, [addStoreResponse])

  useEffect(() => {
    for (const key in storesIdList) {
      console.log(` key ${key}:`);
    }
  }, [storesIdList])

  const isValidStoreName = (name) => {
    const storeNamePattern = /^[a-zA-Z0-9\s]{3,50}$/;
    return storeNamePattern.test(name);
  }

  const handleAddStore = (event) => {
    event.preventDefault();

    if(isValidStoreName(storeName))
    {
    handleOpenNewStore(props.id,storeName).then(
      value => {
        setAddStoreResponse(value as ResponseT);
      })
      .catch(error => alert(error));
    }
  };

  const handleNameChange = (event) => {
    setStoreName(event.target.value);
  }

  return (
  <div>
    <Container className="my-5">
      <Exit id={props.id}/>
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
                  <Form.Control type="text" placeholder="Enter name" value={storeName} onChange={handleNameChange}   
            style={{borderColor: isValidStoreName(storeName) || storeName.length === 0 ? '#28a745' : '#dc3534'}} />
            {!isValidStoreName(storeName) && storeName.length > 0 && <Form.Text className='text-danger'>No Valid Store Name! Store name should contain min 3 letter/numbers</Form.Text>}

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
        {storesIdList.map((storeId) => (
          <Col xs={12} md={6} lg={4} key={storeId} className="my-3">
            <Card>
              <Card.Body>
                <StoreInfo id={props.id} store={storeId} permmisionsOnStore={permissions[storeId] || []}/>
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
