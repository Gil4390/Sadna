import React, {useState} from 'react';
import { Button } from 'react-bootstrap';
import { useNavigate } from "react-router-dom";

export function Store({ name }) {
  const navigate = useNavigate();
  const [storeName, setStoreName] = useState(name);
  return (
    <div>
      <h2>{storeName}</h2>
      <Button variant="dark" onClick={() => navigate("/ManageItemsPage")} style={{margin: "5px"}}>
        View Items
      </Button>
      <Button variant="dark" onClick={() => navigate("/ManageStoreEmployeesPage")} style={{margin: "5px"}} >
        View Employees
      </Button>
      <Button variant="dark" onClick={() => navigate("/PurchasedStoreItemsPage")} style={{margin: "5px"}} >
        View Purchase History
      </Button>
      <Button variant="dark" onClick={() => navigate("/DiscountPoliciesPage")} style={{margin: "5px"}}>
        Discount Policies
      </Button>
      <Button variant="dark" onClick={() => navigate("/PurchasePoliciesPage")} style={{margin: "5px"}}>
        Purchase Policies
      </Button>
    </div>
  );
}
