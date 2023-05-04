import React, { useState } from 'react';
import { NavLink } from "react-router-dom";
import GuestNav from './GuestNav.tsx';
import MemberNav from './MemberNav.tsx';
import AdminNav from './AdminNav.tsx';

function Navigation() {
  const [UserType, setUserType] = useState("guest")

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