
import { Button, Card } from "react-bootstrap"
import { PurcahseCondition , Item , Store} from '../../models/Shop.tsx';
import { handleRemovePurchaseCondition } from "../actions/MemberActions.tsx";
import React, { useState,useEffect } from 'react';



const cartItemStyles = {
  width: "100%",
  marginBottom: "1rem"
};

export function Condition (props)
{
  const [policysList, setPolicyList] = useState<PurcahseCondition[]>([]);
  const handleRemovePress = () => {
    console.log("phase1")
    handleRemovePurchaseCondition(props.storID ,props.condID).then(
      value => {
        setPolicyList(value as PurcahseCondition[]);
      })
      .catch(error => alert(error));
  };

  useEffect(() => {
    console.log("phase2")
    props.handleRemove()
  }, [policysList]);
    return (
      <Card className="h-100 col-md-100">

        <Card.Body className="d-flex flex-column">
            <Card.Title className="d-flex justify-content-between align-items-baseline mb-4">
                <span className="fs-2">{props.condID}</span>
            </Card.Title>
            <Card.Text>
          <span className="ms-2 "> {props.type} of {props.entity} {props.entityName} is {props.value}<br /></span>
          {props.op === 'Conditioning' && (
                    <span>{props.op} <br/> {props.entityRes !=null ? (<span className="ms-2">{props.typeRes} of {props.entityRes} {props.entityNameRes} is {props.valueRes}<br /></span>):(<span></span>)}</span>
                )}
          {props.opCond > -1 && (
                    <span>{props.op} <br/> {props.entityRes !=null ? (<span className="ms-2 text-muted">{props.opCond}<br /></span>):(<span></span>)}</span>)}
        </Card.Text>
        <div className="mt-auto">
        <Button variant="danger" onClick={handleRemovePress}>Remove</Button>
        </div>
      </Card.Body>
    </Card>) 
}

