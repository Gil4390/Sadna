
import './App.css';

import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Navigation, Footer} from "./components";
import { Home, ShoppingPage, CartPage, PaymentPage, About, LoginPage, RegisterPage, StoresManagementPage,
         AdminViewAllUsersPage, AdminManageAllStoresPage, AdminViewAllPurchasesPage, AdminManageComplaintsPage,
         AdminInitializeSystemPage, ManageItemsPage, ManageStoreEmployeesPage, PurchasePoliciesPage,
        DiscountPoliciesPage, PurchasedItemsPage, PurchasedStoreItemsPage, DiscountPoliciesCreatePage, DiscountPoliciesCompoundPage } from "./pages";

import { useEffect, useState } from 'react';
import React from 'react';
import { handleIsSystemInit, handleEnter } from './actions/GuestActions.tsx';


const App:React.FC=()=>{
  const [id, setid] = useState<string>("");
  const [isInit, setisInit] = useState<boolean>(false);

  const handleIdChange = (newId) => {
    setid(newId);
  }

  useEffect(() => {
    handleIsSystemInit().then(value => {setisInit(value);}).catch(error => alert(error));
    handleEnter().then(value => {setid(value);}).catch(error => alert(error)); 
 }, [])
  
  console.log(id);
  console.log(isInit);
  return (
      <Router>
        <Navigation />
        <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/ShoppingPage" element={<ShoppingPage id={id} isInit={isInit} />} />
        <Route path="/CartPage" element={<CartPage />} />
        <Route path="/PaymentPage" element={<PaymentPage />} />
        <Route path="/about" element={<About id={id} isInit={isInit} />} />
        <Route path="/LoginPage" element={<LoginPage id={id} onIdChange={handleIdChange} isInit={isInit}/>} />
        <Route path="/RegisterPage" element={<RegisterPage id={id} isInit={isInit}/>} />

        <Route path="/StoresManagementPage" element={<StoresManagementPage />} />
        <Route path="/PurchasedItemsPage" element={<PurchasedItemsPage />} />
        <Route path="/DiscountPoliciesPage" element={<DiscountPoliciesPage />} />
        <Route path="/DiscountPoliciesCompoundPage" element={<DiscountPoliciesCompoundPage />} />
        <Route path="/DiscountPoliciesCreatePage" element={<DiscountPoliciesCreatePage />} />
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
