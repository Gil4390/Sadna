import React, {useState } from "react";
import Logo from "../images/sadna_express_logo.png";
import Exit from "./Exit.tsx";


function Home(props) {

  return (
    <div className="home">
      <Exit id={props.id}/>
      <div className="container">
        <div className="row align-items-center my-5">
          <div className="col-lg-7">
            <img
              className="img-fluid rounded mb-4 mb-lg-0"
              src={Logo}
              alt="logo"
            />
          </div>
          <div className="col-lg-5">
            <h1 className="font-weight-light">Welcome</h1>
            <p>
              Welcome to our online shopping website! We offer a wide range of
              high-quality products at competitive prices. From clothing and
              accessories to electronics and home goods, we have something for
              everyone.
            </p>
            <p>
              We take pride in our excellent customer service and are always
              here to help. If you have any questions or concerns, please don't
              hesitate to reach out to us.
            </p>
            <p>
              Thank you for choosing our online shopping website for all of
              your shopping needs. We look forward to serving you!
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Home;