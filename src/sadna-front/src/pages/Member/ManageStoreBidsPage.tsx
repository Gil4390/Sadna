import React, { useState, useEffect } from 'react';
import { Button } from 'react-bootstrap';
import { Bid } from '../../models/Bid';
import { handleGetBidsInStore, handleReactToBid } from '../../actions/MemberActions.tsx';
import { Response } from '../models/Response.tsx';
import { useLocation } from 'react-router-dom';

export function BidItem(props) {
  const [showCounterOffer, setShowCounterOffer] = useState(false);
  const [counterOfferValue, setCounterOfferValue] = useState(0);

  const handleApprove = (bid: Bid) => {
    handleReactToBid(props.id, props.bid.itemID, props.bid.bidID, "approved").then(
      value => {
        props.setResponse(value as Response);
        if (value.errorOccured){
          alert(value.errorMessage)
        }
        console.log(value);
      }
    )
    console.log(`Approve bid: ${bid}`);
  };

  const handleDeny = (bid: Bid) => {
    handleReactToBid(props.id,  props.bid.itemID, props.bid.bidID, "denied").then(
      value => {
        props.setResponse(value as Response);
        if (value.errorOccured){
          alert(value.errorMessage)
        }
      }
    )
    console.log(`Deny bid: ${bid.itemName}`);
  };

  const handleCounterOffer = (bid: Bid) => {
    setShowCounterOffer(true);
  };

  const handleCounterOfferSubmit = (bid: Bid) => {
    const price:string = counterOfferValue.toString();
    handleReactToBid(props.id, props.bid.itemID, props.bid.bidID, price).then(
      value => {
        props.setResponse(value as Response);
        if (value.errorOccured){
          alert(value.errorMessage)
        }
        console.log(value);
        setShowCounterOffer(false);
        setCounterOfferValue(0);
      }
    )
  };

  return(
    <div>
      <div>
        <strong>Item Name: </strong>
        {props.bid.itemName}
      </div>
      <div>
        <strong>Bidder Email: </strong>
        {props.bid.bidderEmail}
      </div>
      <div>
        <strong>Offer Price: </strong>
        {props.bid.offerPrice}
      </div>
      <div>
        <strong>Approvers: </strong>
        <div>
          {props.bid.approvers.map((approver, index) => (<div key={index}> {approver} </div>))}
        </div>
      </div>
      <div style={{display: "flex", margin: ""}}>
        {!props.bid.isActive ? (
          <Button variant="success" onClick={() => handleApprove(props.bid)} style={{margin: "0.3rem"}}>
            Approve
          </Button>
        ) : (<div></div>)}
        <Button variant="danger" onClick={() => handleDeny(props.bid)} style={{margin: "0.3rem"}}>
          Deny
        </Button>
        {!props.bid.isActive ? (
          <Button onClick={() => handleCounterOffer(props.bid)} style={{margin: "0.3rem"}}>
            Counter Offer
          </Button>
        ) : (<div></div>)}
        {showCounterOffer && (
          <div>
            <input
              type="number"
              value={counterOfferValue}
              onChange={(e) => setCounterOfferValue(e.target.value)}
              style={{margin: "0.3rem"}}
            />
            <Button onClick={() => handleCounterOfferSubmit(props.bid)} style={{margin: "0.3rem"}}>
              Submit
            </Button>
          </div>
        )}
      </div>
    </div>
  )
}

const ManageStoreBidsPage = (props) => {
  const location = useLocation();
  const { userId, storeId } = location.state;
  const [response, setResponse] = useState<Response>();
  const [bids, setBids] = useState<Bid[]>([]);

  const GetBidsInStore = () => {
    handleGetBidsInStore(userId, storeId).then(
      value => {
        setBids(value as Bid[]);
        console.log(bids);
      }
    )
    .catch(error => alert(error));
  }

  useEffect(() => {
    if(response !=undefined){
      if (!response.errorOccured){
        GetBidsInStore();
      }
    }
  }, [response])

  useEffect(() => {
    GetBidsInStore();
  }, [])

  return (
    <div>
      <h2>Active Bids</h2>
      <ul className="list-group">
        {bids?.map((bid) => (
          <li key={bid.bidID} className="list-group-item">
            <BidItem id={props.id} bid={bid} setResponse={setResponse}/>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ManageStoreBidsPage;