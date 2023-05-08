
import './App.css';

import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Navigation, Footer} from "./components";
import { Home, ShoppingPage, CartPage, PaymentPage, About, LoginPage, RegisterPage, StoresManagementPage,
         AdminViewAllUsersPage, AdminManageAllStoresPage, AdminViewAllPurchasesPage, AdminManageComplaintsPage,
         AdminInitializeSystemPage, ManageItemsPage, ManageStoreEmployeesPage, PurchasePoliciesPage,
        DiscountPoliciesPage, PurchasedItemsPage, PurchasedStoreItemsPage, MessagesPage } from "./pages";

import { useEffect, useState } from 'react';
import React from 'react';
import { handleIsSystemInit, handleEnter, handleIsAdmin } from './actions/GuestActions.tsx';
import { handleLogout } from '../actions/MemberActions.tsx';
import { ResponseT } from "../models/Response.tsx";
import { hubConnection,connection } from 'signalr-no-jquery';
import Popup from './components/Popup.tsx';
import ReactDOM from 'react-dom';
import { handleGetMemberName } from './actions/MemberActions.tsx';

const App:React.FC=()=>{

  const [response, setResponse] = useState<ResponseT>();
  const [id, setid] = useState<string>();
  const [username, setUsername] = useState<string>();
  const [isInit, setisInit] = useState<boolean>(false);
  const [userType, setUserType] = useState("guest");
  const [login, setLogin] = useState<boolean>(false);

  const [notification, setNotification] = useState<string>('');

  useEffect(() => {
    console.log("!!!!!!!!  MY ID IS: "+id);
    const connection = hubConnection('http://localhost:8081/signalR');
    const hubProxy = connection.createHubProxy('NotificationHub');
    
    // set up event listeners i.e. for incoming "message" event
    hubProxy.on('SendNotification', function(idToSend,message) {
        console.log("id= "+idToSend+" "+"message: "+message);

        if(idToSend===id){
          console.log("my id is: "+id);
          console.log("found!! ")
          setNotification(message);
        }
    });
    
    // connect
    connection.start({ jsonp: true })
    .done(function(){ console.log('Now connected, connection ID=' + connection.id); })
    .fail(function(){ console.log('Could not connect'); });

    return () => {
      connection.stop();
    };
  }, [id]);

  useEffect(() => {
    // Show the popup when notifications state is updated
    
    if(notification!=''){
      // Render the popup component with the latest notification
      ReactDOM.render(
         <Popup message={notification} onClose={() => setNotification('')} />,
        document.getElementById('popup-root')
      );
    }
    
  }, [notification]);

  const handleLogin = (newId) => {
    setid(newId);
    setLogin(true);
  }

  const handleLogout = (newId) => {
    setid(newId);
    setUserType("guest");
    setLogin(false);
    
  }

  useEffect(() => {
    handleIsSystemInit().then(value => {setisInit(value);}).catch(error => alert(error));
    handleEnter().then(value => {setid(value);}).catch(error => alert(error)); 
 }, [])


 useEffect(() => {
  if (login){
    handleIsAdmin(id).then(
      value => {       
        if (value.value as boolean) {
          setUserType("admin")
        }
        else{
          setUserType("member")
        }
      }
    ).catch(error=>alert(error));


    handleGetMemberName(id).then(
      value => {
        setUsername(value);
      }
    ).catch(error=>alert(error));
  }
}, [login])
  
  return (
      <Router>
        <Navigation id={id} userType={userType} onLogout={handleLogout} username={username}/>
        <div id="popup-root"></div>
        <Routes>
        <Route path="/" element={<Home id={id} />} />
        <Route path="/ShoppingPage" element={<ShoppingPage id={id} isInit={isInit} />} />
        <Route path="/CartPage" element={<CartPage id={id} isInit={isInit}/>} />
        <Route path="/PaymentPage" element={<PaymentPage />} />
        <Route path="/about" element={<About id={id} isInit={isInit} />} />
        <Route path="/LoginPage" element={<LoginPage id={id} onIdChange={handleLogin} isInit={isInit}/>} />
        <Route path="/RegisterPage" element={<RegisterPage id={id} isInit={isInit}/>} />

        <Route path="/MessagesPage" element={<MessagesPage id={id}/>} />
        <Route path="/StoresManagementPage" element={<StoresManagementPage id={id}/>} />
        <Route path="/PurchasedItemsPage" element={<PurchasedItemsPage id={id} />} />
        <Route path="/DiscountPoliciesPage" element={<DiscountPoliciesPage id={id}/>} />
        <Route path="/PurchasePoliciesPage" element={<PurchasePoliciesPage id={id}/>} />
        <Route path="/ManageStoreEmployeesPage" element={<ManageStoreEmployeesPage id={id}/>} />
        <Route path="/ManageItemsPage" element={<ManageItemsPage id={id}/>} />
        <Route path="/PurchasedStoreItemsPage" element={<PurchasedStoreItemsPage id={id}/>} />

        <Route path="/AdminViewAllUsersPage" element={<AdminViewAllUsersPage id={id}/>} />
        <Route path="/AdminManageAllStoresPage" element={<AdminManageAllStoresPage id={id}/>} />
        <Route path="/AdminViewAllPurchasesPage" element={<AdminViewAllPurchasesPage id={id}/>} />
        <Route path="/AdminManageComplaintsPage" element={<AdminManageComplaintsPage id={id}/>} />
        <Route path="/AdminInitializeSystemPage" element={<AdminInitializeSystemPage id={id} setIsInit={setisInit}/>} />
        
      </Routes>
      <Footer />

      </Router>
  );
}

export default App;