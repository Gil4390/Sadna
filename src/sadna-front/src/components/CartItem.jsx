import React from 'react';
import { Row, Col, ListGroup, Button } from 'react-bootstrap';

export function CartItem({ item, onRemoveItem }) {
  return (
    <ListGroup.Item>
      <Row>
        <Col xs={8}>
          <h5>{item.name}</h5>
          <p>Price: ${item.price}</p>
          <Button variant="danger" onClick={() => onRemoveItem(item.id)}>Remove</Button>
        </Col>
      </Row>
    </ListGroup.Item>
  );
}
