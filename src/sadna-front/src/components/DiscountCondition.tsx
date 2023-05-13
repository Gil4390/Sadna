
import { Button, Card } from "react-bootstrap"
import { Policy } from '../../models/Shop.tsx';
import { handleRemovePolicy , handleAddPolicy } from "../actions/MemberActions.tsx";
import React, { useState,useEffect } from 'react';
import { ResponseT,Response } from '../../models/Response.tsx';



const cartItemStyles = {
    width: "100%",
    marginBottom: "1rem"
  };
  
  export function DiscountCondition (props)
  {
    const [isActive, setIsActive] = useState(true);

    const [policysList, setPolicyList] = useState<Policy[]>([]);
    const handleRemovePress = () => {
      handleRemovePolicy(props.storID ,props.policyID).then(
        value => {
          setPolicyList(value as Policy[]);
        })
        .catch(error => alert(error));
    };

    const handleActiveAddPolicy = () => {
      handleAddPolicy(props.storID ,props.policyID).then(
          value => {
            setPolicyResponse(value as ResponseT)
          })
          .catch(error => alert(error));
    };

    const [policyResponse, setPolicyResponse]=useState<ResponseT>();
    useEffect(() => {
      if(policyResponse!=undefined)
        if(policyResponse?.errorOccured)
          alert(policyResponse?.errorMessage) 
        else{
          console.log("Na")
          setIsActive(!isActive);
        }
        setPolicyResponse(undefined);
    }, [policyResponse])


    useEffect(() => {
      setIsActive(props.active);
      props.handleRemove()
    }, [policysList]);
      return (
        <Card className="h-100 col-md-100">
          <Card.Body className="d-flex flex-column">
              <Card.Title className="d-flex justify-content-between align-items-baseline mb-4">
                  <span className="fs-2">{props.type} {props.policyID}</span>
                </Card.Title>
                <Card.Text>
                                      <div> <span className="ms-2 "><pre>{props.policyRule}</pre></span></div>
                    {props.active  }              </Card.Text>
          <div className="mt-auto" style={{ display: 'flex', justifyContent: 'space-between', maxWidth: '400px' }}>
          <Button variant="danger"onClick={handleRemovePress} >Remove</Button>
          <div>{!isActive && props.type =="Policy" ? (<Button variant="success" onClick={handleActiveAddPolicy}>Activate</Button>):(<span></span>)}</div>
          </div>
        </Card.Body>
      </Card>) 
  }