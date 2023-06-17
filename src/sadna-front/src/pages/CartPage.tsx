import React, { useState, useEffect } from 'react';
import { Container, ListGroup, Button } from 'react-bootstrap';
import { CartItem } from '../components/CartItem.tsx';
import { useNavigate } from "react-router-dom";
import { ItemCart } from '../models/Shop.tsx';
import SystemNotInit from './SystemNotInit.tsx';
import { handleCheckPurchaseConditions, handleGetDetailsOnCart } from '../actions/GuestActions.tsx';
import Exit from './Exit.tsx';


function CartPage(props) {

  const navigate = useNavigate();
  const [cartItems, setCartItems] = useState<ItemCart[]>([]);
  const [totalPrice, setTotalPrice] = useState(0);

  const calculatePrice=()=>{
    const sum = cartItems.reduce((acc, item) => acc + (item.offerPrice > -1 ? item.offerPrice*item.count : (item.priceDiscount > -1 ? item.priceDiscount*item.count : item.price*item.count)), 0);
    const ReducedSum= Math.floor(sum*100)/100;
    setTotalPrice(ReducedSum);
  }
  
  const getShoppingCartItems=()=>{
    handleGetDetailsOnCart(props.id).then(
      value => {
        setCartItems(value as ItemCart[]);
        calculatePrice();
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    if (props.isInit){
      getShoppingCartItems();
      calculatePrice();
    }
  }, []);

  useEffect(() => {
    calculatePrice();
  }, [cartItems]);

  const shoppingCartChanged = () => {
    getShoppingCartItems();
    calculatePrice(); 
  };

  const handleCheckout=(id)=>{
    handleCheckPurchaseConditions(id).then(
      value => {
        value.errorOccured ?
        alert(value.errorMessage) :  navigate('/PaymentPage', {state: {id}})
      })
      .catch(error => alert(error));
  }


  
  return (
    props.isInit?
    (<Container className="my-5">
      <Exit id={props.id}/>
      {cartItems.length === 0 ? (
        <p>Your cart is empty.</p>
      ) : (
        <div>
          <ListGroup>
            {cartItems.map((item) => (
              <CartItem key={item.itemId} item={item} id={props.id} onShoppingCartChanged={shoppingCartChanged}/>
            ))}
          </ListGroup>
          <hr />
          <h4>Total Price: ${totalPrice}</h4>
          <Button variant="success" size="lg" onClick={() => handleCheckout(props.id)}>Checkout Now</Button>
        </div>
      )}
    </Container>):(<SystemNotInit/>)
  );
}

export default CartPage;