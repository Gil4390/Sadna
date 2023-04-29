import React from 'react';
import { Button } from 'react-bootstrap';
import { useNavigate } from "react-router-dom";

export function Store({ store }) {
  const navigate = useNavigate();
  return (
    <div>
      <h2>{store.name}</h2>
      <Button variant="dark" onClick={() => navigate("/ManageItemsPage")} style={{margin: "5px"}}>
        View Items
      </Button>
      <Button variant="dark" onClick={() => navigate("/PoliciesPage")}style={{margin: "5px"}}>
        View Policies
      </Button>
      <Button variant="dark" onClick={() => navigate("/ManageStoreEmployeesPage")} style={{margin: "5px"}} >
        View Employees
      </Button>
    </div>
  );
}
