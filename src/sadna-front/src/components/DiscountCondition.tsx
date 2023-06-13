
import { Button, Card } from "react-bootstrap"
import { Policy } from '../../models/Shop.tsx';
import { handleRemovePolicy , handleAddPolicy, handleRemovePurchaseCondition } from "../actions/MemberActions.tsx";
import React, { useState,useEffect } from 'react';
import { ResponseT,Response } from '../../models/Response.tsx';



const cartItemStyles = {
    width: "100%",
    marginBottom: "1rem"
  };
  
  export function DiscountCondition (props)
  {
    const [isActive, setIsActive] = useState(props.obj.active);
    const [condResponse, setCondResponse]=useState<ResponseT>();
    const [condResponseDis, setCondResponseDis]=useState<Response>();
    const [activatePolicyResponse, setActivatePolicyResponse]=useState<ResponseT>();

    const handleRemovePress = () => {
      if (props.purchase)
      {
        handleRemovePurchaseCondition(props.userID,props.storeID ,props.obj.policyID).then(
          value => {
            setCondResponse(value as ResponseT)
          })
          .catch(error => alert(error));
      }
      else
      {
        handleRemovePolicy(props.userID,props.storeID ,props.obj.policyID , props.obj.type).then(
          value => {
            setCondResponseDis(value as Response)
          })
          .catch(error => alert(error));
      }
    };

    const handleActiveAddPolicy = () => {
      handleAddPolicy(props.userID,props.storeID ,props.obj.policyID).then(
          value => {
            setActivatePolicyResponse(value as ResponseT)
          })
          .catch(error => alert(error));
    };

    useEffect(() => {
      if(activatePolicyResponse!=undefined)
        if(activatePolicyResponse?.errorOccured)
          alert(activatePolicyResponse?.errorMessage) 
        else{
          props.onPolicyChanged();
          setIsActive(true);
          setTimeout(() => {
            alert("Policy activated successfully")
          }, 0);
        }
        setActivatePolicyResponse(undefined);
    }, [activatePolicyResponse])

    useEffect(() => {
      if(condResponse!=undefined)
        if(condResponse?.errorOccured)
          alert(condResponse?.errorMessage) 
        else{
          props.onPolicyChanged();
          setTimeout(() => {
            alert("Condition removed successfully")
          }, 0);

        }
        setCondResponse(undefined);
    }, [condResponse])

    useEffect(() => {
      if(condResponseDis!=undefined)
        if(condResponseDis?.errorOccured)
          alert(condResponseDis?.errorMessage) 
        else{
          props.onPolicyChanged();
          setTimeout(() => {
            alert("Condition removed successfully")
          }, 0);
        }
        setCondResponseDis(undefined);
    }, [condResponseDis])

      return (
        <Card className="h-100 col-md-100">
          <Card.Body className="d-flex flex-column">
              <Card.Title className="d-flex justify-content-between align-items-baseline mb-4">
                  <span className="fs-2">{props.obj.type} {props.obj.policyID}</span>
              </Card.Title>
           <Card.Text>
          <div> <span className="ms-2 "><pre>{props.obj.policyRule}</pre></span></div> 
          </Card.Text>        
          <div className="mt-auto" style={{ display: 'flex', justifyContent: 'space-between', maxWidth: '400px' }}>
          <Button variant="danger"onClick={handleRemovePress} >Remove</Button>
          {!isActive && props.obj.type =="Policy" && (<Button variant="success" onClick={handleActiveAddPolicy}>Activate</Button>)}
          </div>
        </Card.Body>
      </Card>) 
  }