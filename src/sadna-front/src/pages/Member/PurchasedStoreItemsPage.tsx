import React, { useState } from 'react';
import { Container, Table } from 'react-bootstrap';
import Exit from "../Exit.tsx";

const purchases = [
  {
    userId: 1,
    items: [
      { id: 1, name: 'Item 1', price: 10, quantity: 2 },
      { id: 2, name: 'Item 2', price: 5, quantity: 1 },
    ],
    total: 25,
  },
  {
    userId: 2,
    items: [
      { id: 3, name: 'Item 3', price: 15, quantity: 1 },
      { id: 4, name: 'Item 4', price: 8, quantity: 3 },
    ],
    total: 39,
  },
];

const PurchasedStoreItemsPage = (props) => {
  return (
    <Container>
            <Exit id={props.id}/>
      <h1>Purchases</h1>
      <Table striped bordered hover>
        <thead className="thead-dark">
          <tr>
            <th>User ID</th>
            <th>Item ID</th>
            <th>Item Name</th>
            <th>Price</th>
            <th>Quantity</th>
            <th>Total</th>
          </tr>
        </thead>
        <tbody>
          {purchases.map((purchase, purchaseIndex) => (
            purchase.items.map((item, itemIndex) => (
              <tr key={`${purchaseIndex}-${itemIndex}`}>
                {itemIndex === 0 && (
                  <td rowSpan={purchase.items.length}>{purchase.userId}</td>
                )}
                <td>{item.id}</td>
                <td>{item.name}</td>
                <td>{item.price}</td>
                <td>{item.quantity}</td>
                {itemIndex === 0 && (
                  <td rowSpan={purchase.items.length}>{purchase.total}</td>
                )}
              </tr>
            ))
          ))}
        </tbody>
      </Table>
    </Container>
  );
};

export default PurchasedStoreItemsPage;
