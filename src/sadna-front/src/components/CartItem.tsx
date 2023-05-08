
import React, { useState, useEffect } from 'react';
import { Row, Col, ListGroup, Button } from 'react-bootstrap';
import { handleAddItemCart , handleEditItemCart, handleRemoveItemCart} from '../actions/GuestActions.tsx';
import {Response} from '../../models/Response.tsx';


export function CartItem(props) {

  const [amountInCart, setAmountInCart] = useState(props.item.count);
  const [responseAddItemCart, setResponseAddItemCart] = useState<Response>();
  const [responseDecreaseCartQuantity, setResponseDecreaseCartQuantity] = useState<Response>();
  const [responseRemoveItemFromCart, setResponseRemoveItemFromCart] = useState<Response>();

 console.log(`id: ${props.item.itemId}, name: ${props.item.name}`,`category: ${props.item.category}, Price: ${props.item.price}, Rating: ${props.item.rating}, storeid: ${props.item.storeId} instock : ${props.item.inStock} count : ${props.item.count}`)
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
      props.onShoppingCartChanged(props.item.itemId,amountInCart);
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
    props.onShoppingCartChanged(props.item.itemId,amountInCart);
  }, [responseDecreaseCartQuantity])

  const removeFromCart =(id) => {
    handleRemoveItemCart(props.id,props.item.storeId,id).then(
      value => {
        setResponseRemoveItemFromCart(value);
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    if(responseRemoveItemFromCart!=undefined)
    responseRemoveItemFromCart?.errorOccured ? alert(responseRemoveItemFromCart.errorMessage) : (setAmountInCart(0));
    props.onShoppingCartChanged(props.item.itemId,amountInCart);
  }, [responseRemoveItemFromCart])


  return (
    <ListGroup.Item>
      <Row>
        <Col xs={8}>
          <h5>{props.item.name}</h5>
          <p>Price: ${props.item.price}</p>
          {props.item.priceDiscount > -1 && (<div><p> Note! you have discount </p>
          <p>DiscountPrice: ${props.item.priceDiscount}</p></div>)}
          <div className="d-flex align-items-center flex-column" style={{gap:".5rem"}}>
                <div className="d-flex align-items-center justify-content-center" style={{ gap: ".5rem" }}> 
                  <Button variant="warning" onClick={()=> decreaseCartQuantity(props.item.itemId)}>-</Button>
                  <div>
                    <span>{amountInCart} in cart</span> 
                  </div>
                  <Button variant="warning" onClick={()=> increaseCartQuantity(props.item.itemId)}>+</Button>
                </div>
                <Button variant="danger" size="sm" onClick={()=> removeFromCart(props.item.itemId)}>Remove From Cart</Button>
              </div>
        </Col>
      </Row>
    </ListGroup.Item>
  );
}
