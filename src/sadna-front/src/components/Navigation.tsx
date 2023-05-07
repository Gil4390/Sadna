import React, { useState , useEffect } from 'react';
import { NavLink } from "react-router-dom";
import GuestNav from './GuestNav.tsx';
import MemberNav from './MemberNav.tsx';
import AdminNav from './AdminNav.tsx';
import { handleLogout } from '../actions/MemberActions.tsx';
import { ResponseT } from "../models/Response";

function Navigation(props) {
  const [response, setResponse] = useState<ResponseT>();

  const handleLogoutPressing = () => {
    console.log("water")
    handleLogout(props.id).then(
      value => {
        setResponse(value as ResponseT);
      })
      .catch(error => alert(error));
  
  }
  useEffect(() => {
    if(response !=undefined){
      console.log("err? "+response?.errorOccured);
      console.log("errmsg? "+response?.errorMessage);
      response?.errorOccured ? alert(response?.errorMessage) : LogoutSuccess();
    }
 }, [response])

 const LogoutSuccess = () =>{

  props.onLogout(response?.value);
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

              {props.userType === 'guest' ? (
                <div>
                  <GuestNav />
                </div>
                ) : props.userType === 'admin' ? (
                  <div>
                    <AdminNav handleLogout={handleLogoutPressing}/>
                  </div>
                  ) : (
                    <div>
                      <MemberNav handleLogout={handleLogoutPressing}/>
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