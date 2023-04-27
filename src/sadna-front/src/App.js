
import './App.css';

import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Navigation, Footer} from "./components";
import { Home, ShoppingPage, CartPage, PaymentPage, About, LoginPage, RegisterPage, StoresManagementPage,
         AdminViewAllUsersPage, AdminManageAllStoresPage, AdminViewAllPurchasesPage, AdminManageComplaintsPage} from "./pages";
import { ShoppingCartProvider } from "./context/CartContext";


function App() {
  return (
    <ShoppingCartProvider>

      <Router>
        <Navigation />
        <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/ShoppingPage" element={<ShoppingPage />} />
        <Route path="/CartPage" element={<CartPage />} />
        <Route path="/PaymentPage" element={<PaymentPage />} />
        <Route path="/about" element={<About />} />
        <Route path="/LoginPage" element={<LoginPage />} />
        <Route path="/RegisterPage" element={<RegisterPage />} />
        <Route path="/StoresManagementPage" element={<StoresManagementPage />} />

        <Route path="/AdminViewAllUsersPage" element={<AdminViewAllUsersPage />} />
        <Route path="/AdminManageAllStoresPage" element={<AdminManageAllStoresPage />} />
        <Route path="/AdminViewAllPurchasesPage" element={<AdminViewAllPurchasesPage />} />
        <Route path="/AdminManageComplaintsPage" element={<AdminManageComplaintsPage />} />
      </Routes>
      <Footer />

      </Router>

    </ShoppingCartProvider>
  );
}

export default App;
