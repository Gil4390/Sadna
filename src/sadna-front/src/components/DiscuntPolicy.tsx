import { Button, Card } from "react-bootstrap"
import { PurcahseCondition , Item , Store} from '../../models/Shop.tsx';
import { handleRemovePurchaseCondition } from "../actions/MemberActions.tsx";
import React, { useState,useEffect } from 'react';



const cartItemStyles = {
  width: "100%",
  marginBottom: "1rem"
};

export function Condition (props)
{
    const [policysList, setPolicyList] = useState<PurcahseCondition[]>([]);
    const handleRemovePress = () => {
      console.log("phase1")
      handleRemovePurchaseCondition(props.storID ,props.condID).then(
        value => {
          setPolicyList(value as PurcahseCondition[]);
        })
        .catch(error => alert(error));
    };
}