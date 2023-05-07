import React from "react";
import Exit from './Exit.tsx';



function About(props) {
  return (

    <div className="container mt-5">
    <Exit id={props.id}/>
    props.isInit?
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
    </div>
  );
}

export default About;