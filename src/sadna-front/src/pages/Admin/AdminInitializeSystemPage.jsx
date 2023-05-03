import React from "react";
import { Button } from "react-bootstrap";

function AdminInitializeSystemPage() {
  const handleIntialize = () => {
    //request server
  };

  return (
    <div className="AdminInitializeSystemPage">
      <div class="container">
        <div class="row align-items-center my-5">
          <div class="col-lg-5">
            <h2 class="font-weight-light">Admin Initialize System Page </h2>
            <p>
              <Button onClick={handleIntialize}>
                Initialize System
              </Button>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default AdminInitializeSystemPage;