import React, { useState, useEffect } from 'react';
import { Container, ListGroup, Button } from 'react-bootstrap';
import { CartItem } from '../components/CartItem.tsx';
import { useNavigate } from "react-router-dom";
import { Item } from '../models/Shop.tsx';
import SystemNotInit from './SystemNotInit.tsx';
import { handleGetShoppingCart } from '../actions/GuestActions.tsx';

function CartPage(props) {

  const navigate = useNavigate();
  const [cartItems, setCartItems] = useState<Item[]>([]);
  const [totalPrice, setTotalPrice] = useState(0);
  const calculatePrice=(items:Item[])=>{
    var price=0;
    cartItems.map((item)=>price+=(item.price)*item.count);
    setTotalPrice(price);
  }
  const getShoppingCartItems=()=>{
    handleGetShoppingCart(props.id).then(
      value => {
        setCartItems(value as Item[]);
      })
      .catch(error => alert(error));
  }
  useEffect(() => {
    getShoppingCartItems();
  }, []);

  const shoppingCartChanged = () => {
    //post requset to remove item from cart
    getShoppingCartItems();
  };

  return (
    props.isInit?
    (<Container className="my-5">
      <h1>Checkout</h1>
      {cartItems.length === 0 ? (
        <p>Your cart is empty.</p>
      ) : (
        <div>
          <ListGroup>
            {cartItems.map((item) => (
              <CartItem key={item.itemId} item={item} id={props.id} onShoppingCartChanged={shoppingCartChanged} />
            ))}
          </ListGroup>
          <hr />
          <h4>Total Price: ${totalPrice}</h4>
          <Button variant="success" size="lg" onClick={() => navigate("/PaymentPage")}>Checkout Now</Button>
        </div>
      )}
    </Container>):(<SystemNotInit/>)
  );
}

export default CartPage;
