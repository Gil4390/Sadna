import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Button, Table } from 'react-bootstrap';
import { handleGetAllStorePurchases, handleGetAllUserPurchases } from '../../actions/AdminActions.tsx';
import { Orders, Order, ItemForOrder } from '../../models/Purchase.js';

const AdminViewAllPurchasesPage = (props) => {
  const [storeOrUser, setStoreOrUser] = useState('store');
  const [userPurchases, setUserPurchases] = useState<Orders>({orders: new Map()});
  const [storePurchases, setStorePurchases] = useState<Orders>();


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
                <th>User ID</th>
                <th>Store Name</th>
                <th>Item ID</th>
                <th>Item Name</th>
                <th>Item Price</th>
              </tr>
            </thead>
            <tbody>
              {Array.from(userPurchases?.orders).map(([userId, orderList]) => (
                orderList.map(({ order }) =>
                  order.map(({ ItemId, storeName, name, price }) => (
                    <tr key={`${userId}-${ItemId}`}>
                      <td>{userId}</td>
                      <td>{storeName}</td>
                      <td>{ItemId}</td>
                      <td>{name}</td>
                      <td>{price}</td>
                    </tr>
                  ))
                )
              ))}
            </tbody>
          </Table>
        </Col>
      </Row>
    </Container>
  );
};

export default AdminViewAllPurchasesPage;
