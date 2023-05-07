import React, { useState, useEffect } from 'react';
import { Container, Row, Col, Button, Table } from 'react-bootstrap';
import { handleGetAllStorePurchases, handleGetAllUserPurchases } from '../../actions/AdminActions.tsx';
import { Orders, Order, ItemForOrder } from '../../models/Purchase.tsx';
import Exit from "../Exit.tsx";
import { ResponseT } from '../../models/Response.js';

interface OrderTableRow {
  userEmail: string;
  in: OrderTableRowIn;
}

interface OrderTableRowIn {
  storeName: string;
  itemId: string;
  itemName: string;
  itemPrice: number;
}

const AdminViewAllPurchasesPage = (props) => {
  const [storeOrUser, setStoreOrUser] = useState('user');
  const [userPurchases, setUserPurchases] = useState<Orders>({});
  const [purchases, setPurchases] = useState<Orders>({});

  const [response, setResponse] = useState<ResponseT>();


  const getAllUserPurchases = ()=>{
    handleGetAllUserPurchases(props.id).then(
      value => { 
        setPurchases(value as Orders);
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
        setPurchases(value);
      })
      .catch(error => alert(error));
    }
    const handleClickStore = () => {
      getAllStorePurchases();
      setStoreOrUser('store')
    };  
    
    useEffect(() => {
      if (storeOrUser == "store"){
        getAllStorePurchases();
      }
      else{
        getAllUserPurchases();
      }
    }, [storeOrUser]);
    
    useEffect(() => {
      for (const orderId in purchases) {
        console.log(`Order ${orderId}:`);
        const order = purchases[orderId];
        console.log(`  Number of orders: ${order.length}`);
        for (const o of order){
          for (const item of o.listItems) {
            console.log(`  Item: ${item.name} (${item.itemID})`);
            console.log(`    Store: ${item.storeName} (${item.storeID})`);
            console.log(`    Price: ${item.price}`);
          }
        }
      }
    }, [purchases]);
  
  const getOrderTableRows = (): OrderTableRow[] => {
    const orderTableRows: OrderTableRow[] = [];

    for (const userId in purchases) {
      const order = purchases[userId];
      for (const o of order){
        for (const item of o.listItems) {
            orderTableRows.push({
              userEmail: item.userEmail,
              in: {
                storeName: item.storeName,
                itemId: item.itemID,
                itemName: item.name,
                itemPrice: item.price,}
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
                <th>User Email</th>
                <th>Store Name</th>
                <th>Item ID</th>
                <th>Item Name</th>
                <th>Item Price</th>
              </tr>
            </thead>
            <tbody>
              {getOrderTableRows().map((row, index) => (
                <tr key={index}>
                  <td>{row.userEmail}</td>
                  <td>{row.in.storeName}</td>
                  <td>{row.in.itemId}</td>
                  <td>{row.in.itemName}</td>
                  <td>{row.in.itemPrice}</td>
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
