import React, { useState, useEffect } from 'react';
import { Table, Button } from 'react-bootstrap';
import { Member } from '../../models/User';
import {Response} from '../../models/Response.tsx';

const AdminViewAllUsersPage = (props) => {
  const [members, setMembers] = useState<Member[]>([]);


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

  const handleClickRemoveMember = (memberId) => {
    handleRemoveMember(props.id, memberId).then(
      value => {
        getAllMembers();
      })
      .catch(error => alert(error));
  };

  return (
    <Table striped bordered hover>
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
            <td>{member.loggedIn}</td>
            <td>{member.permissions}</td>
            <td>
              <Button variant="danger" onClick={() => handleClickRemoveMember(member.id)}>
                Remove
              </Button>
            </td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default AdminViewAllUsersPage;
