import React from "react";
import { Button, Card } from "react-bootstrap"

const cartItemStyles = {
  width: "100%",
  marginBottom: "1rem"
};

export function Condition (props)
{
    return (
      <Card className="h-100 col-md-100">

        <Card.Body className="d-flex flex-column">
            <Card.Title className="d-flex justify-content-between align-items-baseline mb-4">
                <span className="fs-2">{props.condID}</span>
            </Card.Title>
            <Card.Text>
          <span className="ms-2 text-muted">{props.entity}-{props.entityID}:{props.type} {props.value}<br /></span>
          {props.op === 'Conditioning' && (
                    <span>{props.op} <br/> {props.entityRes !=null ? (<span className="ms-2 text-muted">{props.entityRes}-{props.entityIDRes}:{props.typeRes} {props.valueRes}<br /></span>):(<span></span>)}</span>
                )}
          {props.opCond > -1 && (
                    <span>{props.op} <br/> {props.entityRes !=null ? (<span className="ms-2 text-muted">{props.opCond}<br /></span>):(<span></span>)}</span>)}
        </Card.Text>
        <div className="mt-auto">
        <Button variant="danger">Remove</Button>
        </div>
      </Card.Body>
    </Card>) 
}

