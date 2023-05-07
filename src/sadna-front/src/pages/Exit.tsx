import React, {useState, useEffect } from "react";
import { handleExit } from "../actions/GuestActions.tsx";
import { ResponseT } from "../models/Response";

function Exit(props) {

const [response, setResponse] = useState<ResponseT>();
  useEffect(() => {
    const handleBeforeUnload = (event) => {
      event.preventDefault();
      event.returnValue = "Are you sure you want to leave?";
    };

    const handleUnload = () => {
            handleExit(props.id).then(
        value => {
          setResponse(value as ResponseT);
        })
        .catch(error => alert(error));
    };

    window.addEventListener("beforeunload", handleBeforeUnload);
    window.addEventListener("unload", handleUnload);

    return () => {
      window.removeEventListener("beforeunload", handleBeforeUnload);
      window.removeEventListener("unload", handleUnload);
    };
  }, []);

    return null;
}

export default Exit;