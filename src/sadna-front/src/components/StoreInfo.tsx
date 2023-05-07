import React, {useState,useEffect} from 'react';
import { Button } from 'react-bootstrap';
import { useNavigate } from "react-router-dom";
import {Permission} from '../models/Permission.tsx';
import {Store} from '../models/Shop.tsx';
import { handleGetStoreInfo, handleCloseStore } from '../actions/MemberActions.tsx';
import { ResponseT, Response } from '../models/Response.tsx';

export function StoreInfo(props) {
  const navigate = useNavigate();

  const [response, setResponse] = useState<ResponseT>();
  const [store, setStore] = useState<Store>();
  const [permission, setPermission] = useState<Permission>(props.permmisionsOnStore);
  const [closeStoreResponse, setCloseStoreResponse]=useState<Response>();
  const [refreshStore,setRefreshStore]=useState(0);

  const getStore =()=>{
    handleGetStoreInfo(props.id,props.store).then(
      value => {
        setResponse(value as ResponseT);
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    if(response !=undefined){
      console.log("err? "+response?.errorOccured);
      console.log("errmsg? "+response?.errorMessage);
      response?.errorOccured ? alert(response?.errorMessage) : setStore(response?.value);
    }
 }, [response])

  useEffect(() => {
    getStore();
  }, [refreshStore])

  useEffect(() => {
    if(closeStoreResponse !=undefined){
      response?.errorOccured ? alert(response?.errorMessage) : setRefreshStore(refreshStore+1);
    }
 }, [closeStoreResponse])

  const handleCloseStorePress = (event) => {
    event.preventDefault();
    handleCloseStore(props.id,props.store).then(
      value => {
        setCloseStoreResponse(value as Response);
      })
      .catch(error => alert(error));
  }

  function handleNavigate(windowName,uId, sId) {
    console.log("user id "+uId+" store id "+sId)
    navigate(windowName, { state: { 
      userId: uId,
      storeId: sId ,
    }, });
  }

  function handleViewPurchasePolicyPage(uId, sId) {
    navigate("/PurchasePoliciesPage", { state: { 
      userId: uId,
      storeId: sId ,
    }, });
  }

  return (
    <div>
      <h2>{store?.name}</h2>
      <Button variant="dark" onClick={() =>handleNavigate("/ManageItemsPage",props.id,props.store)} style={{margin: "5px"}}>
        View Items
      </Button>
      {permission.get_employees_info &&<Button variant="dark" onClick={() => handleNavigate("/ManageStoreEmployeesPage",props.id,props.store)} style={{margin: "5px"}} >
        View Employees
      </Button>}
      {permission.get_store_history && (
        <Button variant="dark" onClick={() => handleNavigate("/PurchasedStoreItemsPage",props.id,props.store)} style={{margin: "5px"}} >
          View Purchase History
        </Button>)}
      <Button variant="dark" onClick={() => navigate("/DiscountPoliciesPage")} style={{margin: "5px"}}>
        Discount Policies
      </Button>
      <Button variant="dark" onClick={() => handleViewPurchasePolicyPage(props.id , props.store)} style={{margin: "5px"}}>
        Purchase Policies
      </Button>
      {(store?.isOpen && permission.founder ? (
        <div> 
          <Button variant="danger" onClick={handleCloseStorePress} style={{margin: "5px"}}>
            close store
          </Button>
        </div>) : 
        <div></div>)}
        {(store?.isOpen==false? (
        <div> Store is currently close
        </div>) : 
        <div></div>)}
    </div>
  );
}
