import React from "react";
import { Button } from "react-bootstrap";

function AdminInitializeSystemPage(props) {
  const handleClickIntialize = () => {
    handleInitialize(props.id);
  };

  return (
    <div className="AdminInitializeSystemPage">
      <div className="container">
        <div className="row align-items-center my-5">
          <div className="col-lg-5">
            <h2 className="font-weight-light">Admin Initialize System Page </h2>
            <p>
              <Button onClick={handleClickIntialize}>
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