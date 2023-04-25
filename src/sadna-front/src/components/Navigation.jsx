import React from "react";
import { NavLink } from "react-router-dom";
import { Button, Container, Nav, Navbar as NavbarBs } from "react-bootstrap"
import CartImg from './cart.png'

function Navigation() {
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
            </ul>
          </div>
        

          <Button 
            style={{ width : "4rem",  height : "3rem", padding: "1px", position: "relative", background: "darkgray"}}
            variant="cart"
          >
            <img
              class="img-fluid rounded mb-4 mb-lg-0"
              src={CartImg}
              alt="logo"
            />
            <div className="rounded-circle bg-danger d-flex justify-content-center align-items-center" 
            style={{color: "white", width: "1.5rem", height: "1.5rem", position: "absolute", bottom: 0, right: 0, transform: "translate(25%, 25%)"}}>
              3
            </div>
          </Button>

        </div>
      </nav>
    </div>
  );
}

export default Navigation;