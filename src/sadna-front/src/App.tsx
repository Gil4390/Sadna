
import './App.css';

import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Navigation, Footer} from "./components";
import { Home, ShoppingPage, CartPage, PaymentPage, About, LoginPage, RegisterPage, StoresManagementPage,
         AdminViewAllUsersPage, AdminManageAllStoresPage, AdminViewAllPurchasesPage, AdminManageComplaintsPage,
         AdminInitializeSystemPage, PoliciesPage, ManageItemsPage, ManageStoreEmployeesPage } from "./pages";
import { ShoppingCartProvider } from "./context/CartContext";


import { useEffect, useState } from 'react';
// @ts-ignore
import { GuestThunks } from './services/guest.thunk.ts';
// @ts-ignore
import { AdminThunks } from './services/admin.thunk.ts';
import React from 'react';

const App:React.FC=()=>{
  const [id, setid] = useState(null);
  const [isInit, setisInit] = useState(null);

  useEffect(() => {
    setisInit(AdminThunks.IsSystemInitialize());
   
    setid(GuestThunks.Enter());
 }, [])
  
  console.log(id);
  console.log(isInit);
  return (
    <ShoppingCartProvider>

      <Router>
        <Navigation />
        <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/ShoppingPage" element={<ShoppingPage id={id} isInit={isInit} />} />
        <Route path="/CartPage" element={<CartPage />} />
        <Route path="/PaymentPage" element={<PaymentPage />} />
        <Route path="/about" element={<About id={id} isInit={isInit} />} />
        <Route path="/LoginPage" element={<LoginPage />} />
        <Route path="/RegisterPage" element={<RegisterPage />} />
        <Route path="/StoresManagementPage" element={<StoresManagementPage />} />

        <Route path="/AdminViewAllUsersPage" element={<AdminViewAllUsersPage />} />
        <Route path="/AdminManageAllStoresPage" element={<AdminManageAllStoresPage />} />
        <Route path="/AdminViewAllPurchasesPage" element={<AdminViewAllPurchasesPage />} />
        <Route path="/AdminManageComplaintsPage" element={<AdminManageComplaintsPage />} />
        <Route path="/AdminInitializeSystemPage" element={<AdminInitializeSystemPage />} />
        
        <Route path="/PoliciesPage" element={<PoliciesPage />} />
        <Route path="/ManageStoreEmployeesPage" element={<ManageStoreEmployeesPage />} />
        <Route path="/ManageItemsPage" element={<ManageItemsPage />} />
      </Routes>
      <Footer />

      </Router>

    </ShoppingCartProvider>
  );
}

export default App;
