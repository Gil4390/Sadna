import React from "react";

function About(id, isInit) {
  return (
    isInit?
    (<div className="about">
      <div class="container">
        <div class="row align-items-center my-5">
          <div class="col-lg-5">
            <h1 class="font-weight-light">About</h1>
            <p>
              text
            </p>
          </div>
        </div>
      </div>
    </div>):(<div> <h1 class="font-weight-light">About</h1>
            <p>
              "system is not initialized"
            </p></div>)
  );
}

export default About;