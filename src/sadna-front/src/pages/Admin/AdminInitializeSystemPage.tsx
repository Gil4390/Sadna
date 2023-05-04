import React from "react";
import { Button } from "react-bootstrap";

function AdminInitializeSystemPage() {
  const handleIntialize = () => {
    //request server
  };

  return (
    <div className="AdminInitializeSystemPage">
      <div className="container">
        <div className="row align-items-center my-5">
          <div className="col-lg-5">
            <h2 className="font-weight-light">Admin Initialize System Page </h2>
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