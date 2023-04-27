import React from "react";
import { NavLink } from "react-router-dom";
import CartImg from './cart.png'
import { useShoppingCart } from "../context/CartContext";

function Navigation() {
  const { cartQuantity } = useShoppingCart()
  return (
    <div className="navigation">
      <nav className="navbar navbar-expand navbar-dark" style={{backgroundColor: "black"}}>
        <div className="container">
          <NavLink className="navbar-brand" to="/">
            Sadna Express
          </NavLink>
          <div>
            <ul className="navbar-nav">
              <li className="nav-item">
                <NavLink className="nav-link" to="/">
                  Home
                  <span className="sr-only">(current)</span>
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/ShoppingPage">
                  Shop
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/LoginPage">
                  Login
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/RegisterPage">
                  Register
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/About">
                  About
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/StoresManagementPage">
                  Manage-Stores
                </NavLink>
              </li>
            </ul>
          </div>
        

          <NavLink 
            to="/CartPage"
            style={{ width : "4rem",  height : "3rem", padding: "0.5px", position: "relative", background: "darkgray"}}
          >
            <img
              class="img-fluid rounded mb-4 mb-lg-0"
              src={CartImg}
              alt="logo"
              style={{padding: "0px", transform: "translate(0%, 10%)"} }
            />
            <div className="rounded-circle bg-danger d-flex justify-content-center align-items-center" 
            style={{color: "white", width: "1.5rem", height: "1.5rem", position: "absolute", bottom: 0, right: 0, transform: "translate(35%, 10%)"}}>
              {cartQuantity}
            </div>
          </NavLink>

        </div>
      </nav>
    </div>
  );
}

export default Navigation;