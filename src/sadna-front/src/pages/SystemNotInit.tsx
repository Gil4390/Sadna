import React from "react";
import Exit from "./Exit.tsx";

function SystemNotInit(props) {
  return (
   <div >
          <Exit id={props.id}/>
     <div className="container">
    
        <div className="row align-items-center my-5" style={{textAlign:"center"}}>   
        <h2>
          Unfortunately System is not avialble now
        </h2>
    </div >
   </div >
 </div >  
    )
}

export default SystemNotInit;