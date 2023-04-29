import React, { useState } from 'react';
import { NavLink } from "react-router-dom";

import { useShoppingCart } from "../context/CartContext";
import GuestNav from './GuestNav';
import MemberNav from './MemberNav';
import AdminNav from './AdminNav';

function Navigation() {
  const { cartQuantity } = useShoppingCart()
  const [UserType, setUserType] = useState("member")

  const handleLogout = () => {
    setUserType("guest");
  }

  return (
    <div className="navigation">
      <nav className="navbar navbar-expand navbar-dark" style={{backgroundColor: "black"}}>
        <div className="container">
          <NavLink className="navbar-brand" to="/">
            Sadna Express
          </NavLink>
          <div>
            <ul className="navbar-nav">
              
              <div>

              {UserType === 'guest' ? (
                <div>
                  <GuestNav />
                </div>
                ) : UserType === 'admin' ? (
                  <div>
                    <AdminNav handleLogout={handleLogout}/>
                  </div>
                  ) : (
                    <div>
                      <MemberNav handleLogout={handleLogout}/>
                    </div>
                  )}
              </div>
            </ul>
          </div>
        

        </div>
      </nav>
    </div>
  );
}

export default Navigation;