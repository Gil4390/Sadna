import React from "react";
import { Button } from "react-bootstrap";
import { handleInitializeSystem } from "../../actions/AdminActions.tsx";
import Exit from "../Exit.tsx";

function AdminInitializeSystemPage(props) {
  const handleClickIntialize = () => {
    handleInitializeSystem(props.id);
  };

  return (
    <div className="AdminInitializeSystemPage">
      <Exit id={props.id}/>
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