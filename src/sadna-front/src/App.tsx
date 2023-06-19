
import './App.css';

import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Navigation, Footer} from "./components";
import { Home, ShoppingPage, CartPage, PaymentPage, About, LoginPage, RegisterPage, StoresManagementPage,
         AdminViewAllUsersPage, AdminManageAllStoresPage, AdminViewAllPurchasesPage, AdminManageComplaintsPage,
         AdminInitializeSystemPage, ManageItemsPage, ManageStoreEmployeesPage, PurchasePoliciesPage,
        DiscountPoliciesPage, PurchasedItemsPage, PurchasedStoreItemsPage, MessagesPage,
        AdminRevenuePage, StoreRevenuePage, ManageStoreBidsPage, AdminUserActivityPage } from "./pages";

import { useEffect, useState } from 'react';
import React from 'react';
import { handleEnter, handleIsAdmin } from './actions/GuestActions.tsx';

import { handleLogout } from '../actions/MemberActions.tsx';
import { ResponseT } from "../models/Response.tsx";
import { hubConnection,connection } from 'signalr-no-jquery';
import Popup from './components/Popup.tsx';
import ReactDOM from 'react-dom';
import { handleGetMemberName } from './actions/MemberActions.tsx';
import useSignalRNotifications from './hooks/signalR/useSignalRNotifications.ts';
import { handleIsSystemInit } from './actions/AdminActions.tsx';
//import { useLocalStorage } from './hooks/useLocalStorage.ts'; local storage

const App:React.FC=()=>{

 const [id, setid] = useState<string>();
  //const [id, setid] = useLocalStorage("id", ""); //local storage (should do the same to isInit)
  const [response, setResponse] = useState<ResponseT>();
  const [username, setUsername] = useState<string>();
  const [isInit, setisInit] = useState<boolean>(false);
  const [userType, setUserType] = useState("guest");
  const [login, setLogin] = useState<boolean>(false);
  const [notifications, setNotifications] = useState<string[]>([]);

  useSignalRNotifications('NotificationHub', {
    ['SendNotification']:(userIdToSend,message)=>{
      if(id===userIdToSend){
        setNotifications(prevNotifications => [...prevNotifications, message])
     }
   },
 });

  const handleLogin = (newId) => {
    setid(newId);
    setLogin(true);
  }

  const handleLogout = (newId) => {
    setid(newId);
    setUserType("guest");
    setLogin(false); 
  }

  const handleCloseNotification = () => {
    if(notifications.length>0){
      const updatedNotifications = notifications.filter((item, index) => index !== notifications.length - 1); // creates a new array without the last element
      setNotifications(updatedNotifications);
    }
  }

  useEffect(() => {
    //if(id ===""){
      handleIsSystemInit().then(value => {setisInit(value);}).catch(error => alert(error));
      handleEnter().then(value => {setid(value);}).catch(error => alert(error)); 
   // }
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
        {notifications.map((notification)=><Popup notification={notification} onClose={()=>handleCloseNotification}/>)}
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
        <Route path="/DiscountPoliciesPage" element={<DiscountPoliciesPage/>} />
        <Route path="/PurchasePoliciesPage" element={<PurchasePoliciesPage />} />
        <Route path="/ManageStoreEmployeesPage" element={<ManageStoreEmployeesPage id={id}/>} />
        <Route path="/ManageItemsPage" element={<ManageItemsPage id={id}/>} />
        <Route path="/PurchasedStoreItemsPage" element={<PurchasedStoreItemsPage id={id}/>} />
        <Route path="/StoreRevenuePage" element={<StoreRevenuePage id={id}/>} />
        <Route path="/ManageStoreBidsPage" element={<ManageStoreBidsPage id={id}/>} />

        <Route path="/AdminViewAllUsersPage" element={<AdminViewAllUsersPage id={id}/>} />
        <Route path="/AdminManageAllStoresPage" element={<AdminManageAllStoresPage id={id}/>} />
        <Route path="/AdminViewAllPurchasesPage" element={<AdminViewAllPurchasesPage id={id}/>} />
        <Route path="/AdminManageComplaintsPage" element={<AdminManageComplaintsPage id={id}/>} />
        <Route path="/AdminInitializeSystemPage" element={<AdminInitializeSystemPage id={id} setIsInit={setisInit}/>} />
        <Route path="/AdminRevenuePage" element={<AdminRevenuePage id={id}/>} />
        <Route path="/AdminUserActivityPage" element={<AdminUserActivityPage id={id}/>} />
        
      </Routes>
      <Footer />

      </Router>
  );
}

export default App;