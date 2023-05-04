import React from "react";
import { Button, Card } from "react-bootstrap"

export function Condition ({id , entity , entityID , type , value  ,op  , entityRes=null , entityIDRes=null , typeRes=null , valueRes=null , opCond=null })
{
    return (
        <Card className="h-100">
        <Card.Body className="d-flex flex-column">
            <Card.Title className="d-flex justify-content-between align-items-baseline mb-4">
                <span className="fs-2">{id}</span>
            </Card.Title>
            <Card.Text>
          <span className="ms-2 text-muted">{entity}-{entityID}:{type} {value}<br /></span>
          {op !=null ? (<span>{op} opCond != null ? (<span></span>):(<span></span>)<br /></span>):(<span></span>)}
          {entityRes !=null ? (<span className="ms-2 text-muted">{entityRes}-{entityIDRes}:{typeRes} {valueRes}<br /></span>):(<span></span>)}
        </Card.Text>
        <div className="mt-auto">
        <Button variant="danger">Remove</Button>
        </div>
      </Card.Body>
    </Card>) 
}