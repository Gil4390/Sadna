import React, { useState, useEffect } from 'react';
import { Container, ListGroup, Button } from 'react-bootstrap';
import { CartItem } from '../components/CartItem.tsx';
import { useNavigate } from "react-router-dom";

function CartPage() {
  const items = [
    { id: 1, name: 'Product 1', price: 10.99 },
    { id: 2, name: 'Product 2', price: 19.99 },
    { id: 3, name: 'Product 3', price: 5.99 },
  ];

  const navigate = useNavigate();
  const [cartItems, setCartItems] = useState(items);
  const [totalPrice, setTotalPrice] = useState(0);

  useEffect(() => {
    const fetchData = async () => {
      // const response = await fetch('/api/cart');
      // const data = await response.json();
      // setCartItems(data.items);
      // setTotalPrice(data.totalPrice);
    };

    fetchData();
  }, []);

  const handleRemoveItem = (itemId) => {
    //post requset to remove item from cart
  };

  return (
    <Container className="my-5">
      <h1>Checkout</h1>
      {cartItems.length === 0 ? (
        <p>Your cart is empty.</p>
      ) : (
        <div>
          <ListGroup>
            {cartItems.map((item) => (
              <CartItem key={item.id} item={item} onRemoveItem={handleRemoveItem} />
            ))}
          </ListGroup>
          <hr />
          <h4>Total Price: ${totalPrice}</h4>
          <Button variant="success" size="lg" onClick={() => navigate("/PaymentPage")}>Checkout Now</Button>
        </div>
      )}
    </Container>
  );
}

export default CartPage;
