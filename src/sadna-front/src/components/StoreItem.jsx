import React from "react";
import { Button, Card } from "react-bootstrap"


export function StoreItem({ id, name, price }) {
  return (
    <Card className="h-100">
      <Card.Body className="d-flex flex-column">
        <Card.Title className="d-flex justify-content-between align-items-baseline mb-5">
          <span className="fs-2">{name}</span>
        </Card.Title>
        <Card.Text>
          <span className="ms-2 text-muted">{price} ₪</span>
        </Card.Text>
        <Card.Text>
          <span className="ms-2 text-muted">items left: 5</span>
        </Card.Text>

      </Card.Body>
    </Card>
  )
}
