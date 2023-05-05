import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Button, Table } from 'react-bootstrap';
import { handleGetAllStorePurchases, handleGetAllUserPurchases } from '../../actions/AdminActions.tsx';

const purchasesData = [
  {
    type: 'store',
    storeId: 1,
    userId: 2,
    items: [
      { id: 1, name: 'Item 1', price: 10, quantity: 2 },
      { id: 2, name: 'Item 2', price: 20, quantity: 1 }
    ],
    total: 40
  },
  {
    type: 'user',
    storeId: 3,
    userId: 2,
    items: [
      { id: 3, name: 'Item 3', price: 15, quantity: 3 }
    ],
    total: 45
  },
  // Add more purchase data here
];

const AdminViewAllPurchasesPage = (props) => {
  const [storeOrUser, setStoreOrUser] = useState('store');
  const [userPurchases, setUserPurchases] = useState([]);
  const [storePurchases, setStorePurchases] = useState([]);


  const getAllUserPurchases = ()=>{
    handleGetAllUserPurchases(props.id).then(
      value => {
        setUserPurchases(value);
      })
      .catch(error => alert(error));
  }
  const handleClickUser = () => {
    getAllUserPurchases();
    setStoreOrUser('user')
  };

  const getAllStorePurchases = ()=>{
    handleGetAllStorePurchases(props.id).then(
      value => {
        setStorePurchases(value);
      })
      .catch(error => alert(error));
  }
  const handleClickStore = () => {
    getAllStorePurchases();
    setStoreOrUser('store')
  };  

  useEffect(() => {
    getAllUserPurchases();
    getAllStorePurchases();
  }, []);

  return (
    <Container>
      <Row>
        <Col>
          <Button onClick={handleClickStore}>Store Purchases</Button>
          <Button onClick={handleClickUser}>User Purchases</Button>
        </Col>
      </Row>
      <Row>
        <Col>
          <Table striped bordered hover>
            <thead>
              <tr>
                <th>Purchase ID</th>
                <th>User ID</th>
                <th>Store ID</th>
                <th>Items</th>
                <th>Total</th>
              </tr>
            </thead>
            <tbody>
              {userPurchases.map(purchase => (
                <tr key={`${purchase.type}-${purchase.userId}-${purchase.storeId}`}>
                  <td>{`${purchase.type}-${purchase.userId}-${purchase.storeId}`}</td>
                  <td>{purchase.userId}</td>
                  <td>{purchase.storeId}</td>
                  <td>
                    <ul>
                      {purchase.items.map(item => (
                        <li key={item.id}>{`${item.name} (${item.quantity} x $${item.price})`}</li>
                      ))}
                    </ul>
                  </td>
                  <td>${purchase.total}</td>
                </tr>
              ))}
            </tbody>
          </Table>
        </Col>
      </Row>
    </Container>
  );
};

export default AdminViewAllPurchasesPage;
