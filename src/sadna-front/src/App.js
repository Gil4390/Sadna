
import './App.css';

import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Navigation, Footer, Home, About, LoginPage, RegisterPage } from "./components";

function sayHello() {
  alert('You clicked me!');
}

function App() {
  return (
    <Router>
      <Navigation />
      <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/about" element={<About />} />
      <Route path="/LoginPage" element={<LoginPage />} />
      <Route path="/RegisterPage" element={<RegisterPage />} />
    </Routes>
    <Footer />

    </Router>

  );
}

export default App;
