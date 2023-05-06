import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Button, Table } from 'react-bootstrap';
import { handleGetAllStorePurchases, handleGetAllUserPurchases } from '../../actions/AdminActions.tsx';
import { Orders, Order, ItemForOrder } from '../../models/Purchase.tsx';
import Exit from "../Exit.tsx";
import { ResponseT } from '../../models/Response.js';

interface OrderTableRow {
  userId: string;
  storeName: string;
  itemId: string;
  itemName: string;
  itemPrice: number;
}

const AdminViewAllPurchasesPage = (props) => {
  const [storeOrUser, setStoreOrUser] = useState('store');
  const [userPurchases, setUserPurchases] = useState<Orders>({});
  const [storePurchases, setStorePurchases] = useState<Orders>();

  const [response, setResponse] = useState<ResponseT>();


  const getAllUserPurchases = ()=>{
    handleGetAllUserPurchases(props.id).then(
      value => {
        console.log(value);
        
        setUserPurchases(value as Orders);
      })
      .catch(error => alert(error));
  }


  useEffect(() => {
    for (const orderId in userPurchases) {
      console.log(`Order ${orderId}:`);
      const order = userPurchases[orderId];
      console.log(`  Number of orders: ${order.length}`);
      for (const o of order){
        for (const item of o.listItems) {
          console.log(`  Item: ${item.name} (${item.itemID})`);
          console.log(`    Store: ${item.storeName} (${item.storeID})`);
          console.log(`    Price: ${item.price}`);
        }
      }
    }
  }, [userPurchases]);
  

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
    //getAllStorePurchases();
  }, []);

  
  const getOrderTableRows = (): OrderTableRow[] => {
    const orderTableRows: OrderTableRow[] = [];

    for (const userId in userPurchases) {
      const order = userPurchases[userId];
      for (const o of order){
        for (const item of o.listItems) {
            orderTableRows.push({
              userId,
              storeName: item.storeName,
              itemId: item.itemID,
              itemName: item.name,
              itemPrice: item.price,
            });
          }
    }



  }

  return orderTableRows;
  };

  return (
    <Container>
      <Exit id={props.id}/>
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
        {getOrderTableRows().map((row, index) => (
          <tr key={index}>
            <td>{row.userId}</td>
            <td>{row.storeName}</td>
            <td>{row.itemId}</td>
            <td>{row.itemName}</td>
            <td>{row.itemPrice}</td>
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
