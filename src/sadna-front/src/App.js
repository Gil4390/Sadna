
import './App.css';

import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Navigation, Footer} from "./components";
import { Home, ShoppingPage, CartPage, PaymentPage, About, LoginPage, RegisterPage } from "./pages";
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
      </Routes>
      <Footer />

      </Router>

    </ShoppingCartProvider>
  );
}

export default App;
