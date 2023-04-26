import React from "react";
import { Button, Card } from "react-bootstrap"
import { useShoppingCart } from "../context/CartContext";


export function StoreItem({ id, name, price }) {
  const {getItemQuantity,
    increaseCartQuantity,
    decreaseCartQuantity,
    removeFromCart } = useShoppingCart()
  const amountInCart = getItemQuantity(id)
  return (
    <Card className="h-100">
      <Card.Body className="d-flex flex-column">
        <Card.Title className="d-flex justify-content-between align-items-baseline mb-4">
          <span className="fs-2">{name}</span>
          <span className="ms-2 text-muted">{price} ₪</span>

        </Card.Title>
        <Card.Text>
          <span className="ms-2 text-muted">items left: 5</span>
        </Card.Text>
        <div className="mt-auto">
          {amountInCart === 0 ? (
            <Button className="w-20" onClick={()=> increaseCartQuantity(id)}> Add To Cart</Button>
          ) : (<div className="d-flex align-items-center flex-column" style={{gap:".5rem"}}>
                <div className="d-flex align-items-center justify-content-center" style={{ gap: ".5rem" }}> 
                  <Button variant="warning" onClick={()=> decreaseCartQuantity(id)}>-</Button>
                  <div>
                    <span>{amountInCart} in cart</span> 
                  </div>
                  <Button variant="warning" onClick={()=> increaseCartQuantity(id)}>+</Button>
                </div>
                <Button variant="danger" size="sm" onClick={()=> removeFromCart(id)}>Remove From Cart</Button>
              </div>)}

        </div>

      </Card.Body>
    </Card>
  )
}
