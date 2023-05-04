import React from "react";

function About(id, isInit) {
  return (
    isInit?
    (<div className="about">
      <div className="container">
        <div className="row align-items-center my-5">
          <div className="col-lg-5">
            <h1 className="font-weight-light">About</h1>
            <p>
              text
            </p>
          </div>
        </div>
      </div>
    </div>):(<div> <h1 className="font-weight-light">About</h1>
            <p>
              "system is not initialized"
            </p></div>)
  );
}

export default About;