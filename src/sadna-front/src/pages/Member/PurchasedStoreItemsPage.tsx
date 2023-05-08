import React, { useEffect, useState } from 'react';
import { useLocation } from 'react-router-dom';
import { Container, Table } from 'react-bootstrap';
import Exit from "../Exit.tsx";
import { handleGetAllPurchasesFromStore } from '../../actions/MemberActions.tsx';
import { ItemForOrder } from '../../models/Purchase.tsx';


const PurchasedStoreItemsPage = (props) => {
  const [purchases, setPurchases] = useState<ItemForOrder[]>([]);
  const location = useLocation();
  const { userId, storeId } = location.state;

  const getStorePurchases = ()=>{
    handleGetAllPurchasesFromStore(userId, storeId).then(
      value => {
        setPurchases(value as ItemForOrder[]);
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    getStorePurchases();
  }, []);

  return (
    <Container>
      <Exit id={props.id}/>
      <h1>Purchases</h1>
      <Table striped bordered hover>
        <thead className="thead-dark">
          <tr>
            <th>User Email</th>
            <th>Item Name</th>
            <th>Item Category</th>
            <th>Item ID</th>
            <th>Price</th>
          </tr>
        </thead>
        <tbody>
          {purchases.map((item) => (
              <tr key={`${item.userEmail}-${item.name}`}>
                <td>{item.userEmail}</td>
                <td>{item.name}</td>
                <td>{item.category}</td>
                <td>{item.itemID}</td>
                <td>{item.price}</td>
              </tr>
            )
          )}
        </tbody>
      </Table>
    </Container>
  );
};

export default PurchasedStoreItemsPage;
