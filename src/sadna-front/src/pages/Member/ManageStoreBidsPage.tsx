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
    handleReactToBid(props.id, bid.itemID, "approved").then(
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
    handleReactToBid(props.id, bid.itemID, "denied").then(
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
    console.log(`Counter offer submitted for bid: ${props.id}`);
    const price:string = counterOfferValue.toString();
    handleReactToBid(props.id, props.id, price).then(
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
        <Button variant="success" onClick={() => handleApprove(props.bid)} style={{margin: "0.3rem"}}>
          Approve
        </Button>
        <Button variant="danger" onClick={() => handleDeny(props.bid)} style={{margin: "0.3rem"}}>
          Deny
        </Button>
        <Button onClick={() => handleCounterOffer(props.bid)} style={{margin: "0.3rem"}}>
          Counter Offer
        </Button>
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
  // Sample data for active bids
  const activeBids: Bid[] = [
    {
      itemName: 'Item 1',
      itemID: "123",
      bidderEmail: 'bidder1@example.com',
      offerPrice: 100,
      approvers: ['approver1@example.com', 'approver2@example.com'],
    },
    {
      itemName: 'Item 2',
      itemID: "111",
      bidderEmail: 'bidder2@example.com',
      offerPrice: 150,
      approvers: ['approver3@example.com'],
    },
    // Add more active bids as needed
  ];

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
        {bids?.map((bid, index) => (
          <li key={index} className="list-group-item">
            <BidItem id={props.id} bid={bid} setResponse={setResponse}/>
          </li>
        ))}
      </ul>
    </div>
  );
};

export default ManageStoreBidsPage;