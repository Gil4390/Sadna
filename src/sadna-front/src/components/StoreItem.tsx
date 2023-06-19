import React, { useState, useEffect } from 'react';
import { Button, Card, Form, Modal } from "react-bootstrap"
import { handleAddItemCart , handleEditItemCart, handlePlaceBid, handleRemoveItemCart} from '../actions/GuestActions.tsx';
import {Response} from '../../models/Response.tsx';
import { handleGetItemReviews } from '../actions/MemberActions.tsx';
import { Review } from '../models/Review.js';


const ReviewsModal = ({ show, handleClose, item, reviews}) => {
  return (
    <Modal show={show} onHide={handleClose}>
      <Modal.Header closeButton>
        <Modal.Title>Reviews of: {item.name}</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {reviews.length == 0 ? 
        (<div>this item has no reviews</div>) : 
        
        reviews?.map((review) =>
          <div>{review.reviewText}</div>
          )
        }
      </Modal.Body>
    </Modal>
  );
};


export function StoreItem(props) {
  const [showReviewModal, setShowReviewModal] = useState(false);
  const [showBidModal, setShowBidModal] = useState(false);
  const [reviews, setReviews] = useState<Review[]>([]);
  const [showBidButton, setShowBidButton] = useState(props.item.bidOpen);
    
  const [amountInCart, setAmountInCart] = useState(props.item.count);
  const [responseAddItemCart, setResponseAddItemCart] = useState<Response>();
  const [responseDecreaseCartQuantity, setResponseDecreaseCartQuantity] = useState<Response>();
  const [responseRemoveItemFromCart, setResponseRemoveItemFromCart] = useState<Response>();
  const [responsePlaceBid, setResponsePlaceBid] = useState<Response>();
  const [modified, setModified] = useState(props.modified);

  const [price, setPrice] = useState(0);
  const handlePriceChange = (event) => {
    setPrice(event.target.value);
  }


 //console.log(`id: ${props.item.itemId}, name: ${props.item.name}`,`category: ${props.item.category}, Price: ${props.item.price}, Rating: ${props.item.rating}, storeid: ${props.item.storeId} instock : ${props.item.inStock}`)
  const increaseCartQuantity =(id) => {
    handleAddItemCart(props.id,props.item.storeId,id).then(
      value => {
        setResponseAddItemCart(value);
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    if(responseAddItemCart!=undefined)
      responseAddItemCart?.errorOccured ? alert(responseAddItemCart.errorMessage) : setAmountInCart(amountInCart+1);
  }, [responseAddItemCart])

  const decreaseCartQuantity =(id) => {
    handleEditItemCart(props.id,props.item.storeId,id,amountInCart-1 ).then(
      value => {
        setResponseDecreaseCartQuantity(value);
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    if(responseDecreaseCartQuantity!=undefined)
    responseDecreaseCartQuantity?.errorOccured ? alert(responseDecreaseCartQuantity.errorMessage) : setAmountInCart(amountInCart-1);
  }, [responseDecreaseCartQuantity])

  const removeFromCart =(id) => {
    handleRemoveItemCart(props.id,props.item.storeId,id).then(
      value => {
        setResponseRemoveItemFromCart(value);
      })
      .catch(error => alert(error));
  }

  const handleClickBid = () => {
    setShowBidModal(true);
  }
  
  const sendBid = (event) => {
    event.preventDefault();
    setShowBidModal(false);
    handlePlaceBid(props.id, props.item.itemId, price).then(
      value => {
        setResponsePlaceBid(value as Response);
        if (value.errorOccured) {
          alert(value.errorMessage)
        }
        else {
          setShowBidButton(false);
          props.setModified(Date.now());
          setTimeout(() => {
            alert("Bid sent")
          }, 0);
        }
      }
    ).catch(error => alert(error));

  }

  const handleClickReviews = (item) => {
    setShowReviewModal(true);
    handleGetItemReviews(item.itemId).then(
      value => {
        console.log(value);
        setReviews(value);

      }
    )
  }

  useEffect(() => {
    if(responseRemoveItemFromCart!=undefined)
    responseRemoveItemFromCart?.errorOccured ? alert(responseRemoveItemFromCart.errorMessage) : setAmountInCart(0);
  }, [responseRemoveItemFromCart])

  useEffect(() => {
    setShowBidButton(!props.item.openBid)
  },[])



  return (
    <div>

      <Card className="h-100">
        <Card.Body className="d-flex flex-column">
          <Card.Title className="d-flex justify-content-between align-items-baseline mb-4">
            <div><div style={{display:"flex", justifyContent:"space-between"}}><div className="fs-2">{props.item.name} </div>
            <div className="ms-2 text-muted" style={{marginLeft:"0.5rem"}}>{props.item.price} $ </div></div>
            {(props.item.priceDiscount > -1) && <div><span style={{fontSize:"12px", color:"blue"}}> Note! you have discount</span> <span style={{fontSize:"12px", color:"blue"}}>{ props.item.priceDiscount} $</span></div>}
            {(props.item.offerPrice > -1) && <div><span style={{fontSize:"12px", color:"green"}}> Note! you have offer</span> <span style={{fontSize:"12px", color:"green"}}>{ props.item.offerPrice} $</span></div>}</div>

            {/* <span className="ms-2 text-muted">{props.item.rating} ₪</span> */}
          </Card.Title>
          {props.item.inStock==0? (<Card.Text>
            <span className="ms-2 text-muted">Out Of Stock</span>
          </Card.Text>) : (<Card.Text></Card.Text>) }
          
          <div className="mt-auto">
            {props.item.inStock>0 ?
            (amountInCart === 0 ? (
              <Button className="w-20" onClick={()=> increaseCartQuantity(props.item.itemId)}> Add To Cart</Button>
            ) : (<div className="d-flex align-items-center flex-column" style={{gap:".5rem"}}>
                  <div className="d-flex align-items-center justify-content-center" style={{ gap: ".5rem" }}> 
                    <Button variant="warning" onClick={()=> decreaseCartQuantity(props.item.itemId)}>-</Button>
                    <div>
                      <span>{amountInCart} in cart</span> 
                    </div>
                    <Button variant="warning" onClick={()=> increaseCartQuantity(props.item.itemId)}>+</Button>
                  </div>
                  <Button variant="danger" size="sm" onClick={()=> removeFromCart(props.item.itemId)}>Remove From Cart</Button>
                </div>)):(<div></div>)}

          </div>
          {showBidButton ? (
            <div>
            <Button onClick={() => handleClickBid()} style={{margin: "0.5rem"}}>
              Place Bid
            </Button>
            </div>
          ): (<div></div>)}
          
          
          <Button onClick={() => handleClickReviews(props.item)} style={{margin: "0.5rem"}}>
            Reviews
          </Button>

        </Card.Body>

        <Modal show={showBidModal} onHide={() => setShowBidModal(false)}>
          <Modal.Header closeButton>
            <Modal.Title>Place bid on: {props.item.name}</Modal.Title>
          </Modal.Header>
          <Modal.Body>

            <Form onSubmit={sendBid}>
              <Form.Group controlId="price">
                  <Form.Control type="text" placeholder="Enter bid price" value={price} onChange={handlePriceChange}></Form.Control>
              </Form.Group>
              <Button variant="primary" type="submit" style={{margin: "0.5rem"}}>
                Place Bid
              </Button>
            </Form>
          </Modal.Body>
        </Modal>

        <ReviewsModal 
          show={showReviewModal}
          handleClose={() => setShowReviewModal(false)}
          item={props.item}
          reviews={reviews}
        />
      </Card>
    </div>
  )
}