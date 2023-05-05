import React, { useState } from 'react';
import { Container, Row, Col, Button, Table } from 'react-bootstrap';

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

const AdminViewAllPurchasesPage = () => {
  const [storeOrUser, setStoreOrUser] = useState('store');

  const [purchaseType, setPurchaseType] = useState('store');
  const purchases = purchasesData.filter(purchase => purchase.type === purchaseType);

  return (
    <Container>
      <Row>
        <Col>
          <Button onClick={() => setPurchaseType('store')} active={purchaseType === 'store'}>Store Purchases</Button>
          <Button onClick={() => setPurchaseType('user')} active={purchaseType === 'user'}>User Purchases</Button>
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
              {purchases.map(purchase => (
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
