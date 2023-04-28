import React from 'react';
import { Button } from 'react-bootstrap';

export function Store({ store }) {
  return (
    <div>
      <h2>{store.name}</h2>
      <Button variant="dark" style={{margin: "5px"}}>
        View Items
      </Button>
      <Button variant="dark" style={{margin: "5px"}}>
        View Policies
      </Button>
      <Button variant="dark" style={{margin: "5px"}}>
        View Employees
      </Button>
    </div>
  );
}
