import React, { useState, useEffect } from 'react';
import { Table, Button } from 'react-bootstrap';
import { Member } from '../../models/User';
import { handleGetAllMembers, handleRemoveUserMembership } from '../../actions/AdminActions.tsx';
import Exit from "../Exit.tsx";
import { ResponseT } from '../../models/Response';

const AdminViewAllUsersPage = (props) => {
  const [members, setMembers] = useState<Member[]>([]);
  const [response, setResponse] = useState<ResponseT>();

  const getAllMembers = ()=>{
    handleGetAllMembers(props.id).then(
      value => {
        
        setMembers(value as Member[]);
      })
      .catch(error => alert(error));
  }


  useEffect(() => {
    getAllMembers();
  }, []);

  useEffect(() => {
    if(response !=undefined){
      response?.errorOccured ? alert(response?.errorMessage) : getAllMembers();
    }
 }, [response])
  

  const handleClickRemoveMember = (memberEmail) => {
    handleRemoveUserMembership(props.id, memberEmail).then(
      value => {
        setResponse(value as ResponseT);
      })
      .catch(error => alert(error));
  };

  return (
    <div style={{paddingBottom: "4rem"}}>
      <Table striped bordered hover>
        <Exit id={props.id}/>
        <thead>
          <tr>
            <th>User ID</th>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Email</th>
            <th>LoggedIn</th>
            <th>Permissions</th>
          </tr>
        </thead>
        <tbody>
          {members.map((member) => (
            <tr key={member.id}>
              <td>{member.id}</td>
              <td>{member.firstName}</td>
              <td>{member.lastName}</td>
              <td>{member.email}</td>
              <td>{member.loggedIn ? "Logged-In" : "Offline"}</td>
              <td>{member.permissions?.map((p) => (
                <div key={p}> {p} </div>
              ))}</td>
              <td>
                <Button variant="danger" onClick={() => handleClickRemoveMember(member.email)}>
                  Remove
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  );
};

export default AdminViewAllUsersPage;
