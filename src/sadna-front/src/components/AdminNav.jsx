import React, { useState } from 'react';
import { NavLink } from "react-router-dom";


function MemberNav(handleLogout) {
  
  return (
    <div className="navigation">
      <nav className="navbar navbar-expand navbar-dark" style={{backgroundColor: "black"}}>
        <div className="container">

          <div>
            <ul className="navbar-nav" >
              <li className="nav-item">
                <NavLink className="nav-link" to="/">
                  Home
                  <span className="sr-only">(current)</span>
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/" onClick={() => handleLogout()}>
                  Logout
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/AdminInitializeSystemPage">
                  Initialize-System
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/AdminViewAllUsersPage">
                  View-All-Users
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/AdminManageAllStoresPage">
                  Manage-All-Stores
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/AdminViewAllPurchasesPage">
                  View-All-Purchases
                </NavLink>
              </li>
              <li className="nav-item">
                <NavLink className="nav-link" to="/AdminManageComplaintsPage">
                  Manage-Complaints
                </NavLink>
              </li>
            </ul>
          </div>

        </div>
      </nav>
    </div>
  );
}

export default MemberNav;