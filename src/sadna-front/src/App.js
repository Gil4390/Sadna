
import './App.css';

import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Navigation, Footer} from "./components";
import { Home, ShoppingPage, About, LoginPage, RegisterPage } from "./pages";


function sayHello() {
  alert('You clicked me!');
}

function App() {
  return (
    <Router>
      <Navigation />
      <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/ShoppingPage" element={<ShoppingPage />} />
      <Route path="/about" element={<About />} />
      <Route path="/LoginPage" element={<LoginPage />} />
      <Route path="/RegisterPage" element={<RegisterPage />} />
    </Routes>
    <Footer />

    </Router>

  );
}

export default App;
