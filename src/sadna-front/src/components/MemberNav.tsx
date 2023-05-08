import React, { useState , useEffect } from 'react';
import { NavLink } from "react-router-dom";
import CartImg from '../images/cart.png'


function MemberNav(props) {
  const handleLogoutPress = () => {
    props.handleLogout();
  }

  return (
    <div className="navigation">
      <nav className="navbar navbar-expand navbar-dark" style={{backgroundColor: "black"}}>
        <div className="container">

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
                <NavLink className="nav-link" to="/"  onClick={() => handleLogoutPress()}>
                  Logout
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/MessagesPage">
                  Messages
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/PurchasedItemsPage">
                  Purchased-Items
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
              className="img-fluid rounded mb-4 mb-lg-0"
              src={CartImg}
              alt="logo"
              style={{padding: "0px", transform: "translate(0%, 10%)"} }
            />
          </NavLink>

        </div>
      </nav>
    </div>
  );
}

export default MemberNav;