import React, { useState, useEffect } from 'react';
import { Table, Button } from 'react-bootstrap';

const AdminViewAllUsersPage = () => {
  const [users, setUsers] = useState([
    { id: 1, name: 'John Doe', email: 'john.doe@example.com', membershipType: 'member' },
    { id: 2, name: 'Jane Doe', email: 'jane.doe@example.com', membershipType: 'guest' },
    { id: 3, name: 'Bob Smith', email: 'bob.smith@example.com', membershipType: 'member' },
    { id: 4, name: 'Alice Brown', email: 'alice.brown@example.com', membershipType: 'guest' },
  ]);

  useEffect(() => {
    // const fetchUsers = async () => {
    //   const response = await fetch('/api/users');
    //   const data = await response.json();
    //   setUsers(data);
    // };

    // fetchUsers();
  }, []);

  const handleRemoveUser = async (userId) => {
    // Send a request to API to remove user
    const response = await fetch(`/api/users/${userId}`, { method: 'DELETE' });
    if (response.ok) {
      // If user was removed successfully, update the state
      setUsers((prevUsers) => prevUsers.filter((user) => user.id !== userId));
    }
  };

  return (
    <Table striped bordered hover>
      <thead>
        <tr>
          <th>User ID</th>
          <th>Name</th>
          <th>Email</th>
          <th>Membership Type</th>
          <th>Actions</th>
        </tr>
      </thead>
      <tbody>
        {users.map((user) => (
          <tr key={user.id}>
            <td>{user.id}</td>
            <td>{user.name}</td>
            <td>{user.email}</td>
            <td>{user.membershipType}</td>
            <td>
              {user.membershipType === 'member' && (
                <Button variant="danger" onClick={() => handleRemoveUser(user.id)}>
                  Remove
                </Button>
              )}
            </td>
          </tr>
        ))}
      </tbody>
    </Table>
  );
};

export default AdminViewAllUsersPage;
