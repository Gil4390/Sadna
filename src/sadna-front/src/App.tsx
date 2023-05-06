
import './App.css';

import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Navigation, Footer} from "./components";
import { Home, ShoppingPage, CartPage, PaymentPage, About, LoginPage, RegisterPage, StoresManagementPage,
         AdminViewAllUsersPage, AdminManageAllStoresPage, AdminViewAllPurchasesPage, AdminManageComplaintsPage,
         AdminInitializeSystemPage, ManageItemsPage, ManageStoreEmployeesPage, PurchasePoliciesPage,
        DiscountPoliciesPage, PurchasedItemsPage, PurchasedStoreItemsPage } from "./pages";

import { useEffect, useState } from 'react';
import React from 'react';
import { handleIsSystemInit, handleEnter, handleIsAdmin } from './actions/GuestActions.tsx';


const App:React.FC=()=>{
  const [id, setid] = useState<string>();
  const [isInit, setisInit] = useState<boolean>(false);
  const [userType, setUserType] = useState("guest");
  const [login, setLogin] = useState<boolean>(false);

  const handleLogin = (newId) => {
    setid(newId);
    setLogin(true);
  }

  const handleLogout = (newId) => {
    setid(newId);
    setUserType("guest");
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
  }
}, [login])
  
  console.log(id);
  console.log(isInit);
  return (
      <Router>
        <Navigation id={id} userType={userType} onLogout={handleLogout}/>
        <Routes>
        <Route path="/" element={<Home id={id} />} />
        <Route path="/ShoppingPage" element={<ShoppingPage id={id} isInit={isInit} />} />
        <Route path="/CartPage" element={<CartPage id={id} isInit={isInit}/>} />
        <Route path="/PaymentPage" element={<PaymentPage />} />
        <Route path="/about" element={<About id={id} isInit={isInit} />} />
        <Route path="/LoginPage" element={<LoginPage id={id} onIdChange={handleLogin} isInit={isInit}/>} />
        <Route path="/RegisterPage" element={<RegisterPage id={id} isInit={isInit}/>} />

        <Route path="/StoresManagementPage" element={<StoresManagementPage id={id}/>} />
        <Route path="/PurchasedItemsPage" element={<PurchasedItemsPage />} />
        <Route path="/DiscountPoliciesPage" element={<DiscountPoliciesPage />} />
        <Route path="/PurchasePoliciesPage" element={<PurchasePoliciesPage />} />
        <Route path="/ManageStoreEmployeesPage" element={<ManageStoreEmployeesPage />} />
        <Route path="/ManageItemsPage" element={<ManageItemsPage />} />
        <Route path="/PurchasedStoreItemsPage" element={<PurchasedStoreItemsPage />} />

        <Route path="/AdminViewAllUsersPage" element={<AdminViewAllUsersPage />} />
        <Route path="/AdminManageAllStoresPage" element={<AdminManageAllStoresPage />} />
        <Route path="/AdminViewAllPurchasesPage" element={<AdminViewAllPurchasesPage />} />
        <Route path="/AdminManageComplaintsPage" element={<AdminManageComplaintsPage />} />
        <Route path="/AdminInitializeSystemPage" element={<AdminInitializeSystemPage />} />
        
      </Routes>
      <Footer />

      </Router>
  );
}

export default App;