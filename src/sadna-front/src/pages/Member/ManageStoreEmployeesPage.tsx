import React, { useState } from "react";
import { Button, Modal, Table, Form } from "react-bootstrap";
import Exit from "../Exit.tsx";

type Employee = {
  firstName: string,
  lastName: string,
  permissions: string,
}

const ManageStoreEmployeesPage = (props) => {
  const [employees, setEmployees] = useState<Employee[]>([
    { firstName: "John", lastName: "Doe", permissions: "Founder" },
    { firstName: "Jane", lastName: "Doe", permissions: "Manager" },
    { firstName: "Bob", lastName: "Smith", permissions: "Manager, addEmployees" },
  ]);
  const [showModal, setShowModal] = useState(false);
  const [showAddModal, setShowAddModal] = useState(false);
  const [selectedEmployee, setSelectedEmployee] = useState<Employee|null>(null);

  const handleAddModalClose = () => setShowAddModal(false);
  const handleAddModalShow = () => setShowAddModal(true);

  const handleRemoveEmployee = (index) => {
    const newEmployees = [...employees];
    newEmployees.splice(index, 1);
    setEmployees(newEmployees);
  };

  const handleAddPermission = (employee) => {
    setSelectedEmployee(employee);
    setShowModal(true);
  };

  const handleModalSubmit = (permissions) => {
    const newEmployees = [...employees];
    const index = newEmployees.indexOf( {firstName: "John", lastName: "Doe", permissions: "Founder" });
    newEmployees[index].permissions = permissions;
    setEmployees(newEmployees);
    setShowModal(false);
  };

  const handleAddEmployee = (event) => {
    event.preventDefault();
    const formData = new FormData(event.target);
    const newEmp : Employee= {
      firstName: formData.get('firstName') as string,
      lastName: formData.get('LastName') as string,
      permissions: formData.get('permissions') as string
    };
    setEmployees([...employees, newEmp]);
    setShowAddModal(false);
  };

  return (
    <div>
      <Exit id={props.id}/>
      <h1>Employees</h1>
      <Button variant="primary" onClick={handleAddModalShow} style={{margin: "0.5rem"}}>
        Add New Employee
      </Button>

      <Modal show={showAddModal} onHide={handleAddModalClose}>
      <Modal.Header closeButton>
        <Modal.Title>Add Employee</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form onSubmit={handleAddEmployee}>
          <Form.Group controlId="firstName">
            <Form.Label>Employee First Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter first name"
            />
          </Form.Group>
          <Form.Group controlId="lastName">
            <Form.Label>Employee Last Name</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter last name"
            />
          </Form.Group>
          <Form.Group controlId="permissions">
            <Form.Label>Permissions</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter permissions: perm1,perm2"
            />
          </Form.Group>
          <Button variant="primary" type="submit" style={{margin: "0.5rem"}}>
            Add
          </Button>
        </Form>
      </Modal.Body>
      </Modal>

      <Table striped bordered hover>
        <thead>
          <tr>
            <th>First Name</th>
            <th>Last Name</th>
            <th>Permissions</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {employees.map((employee, index) => (
            <tr key={index}>
              <td>{employee.firstName}</td>
              <td>{employee.lastName}</td>
              <td>{employee.permissions}</td>
              <td>
                <Button
                  variant="danger"
                  onClick={() => handleRemoveEmployee(index)}
                >
                  Remove
                </Button>{" "}
                <Button
                  variant="primary"
                  onClick={() => handleAddPermission(employee)}
                >
                  Add Permission
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
      <Modal show={showModal} onHide={() => setShowModal(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Add Permission</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <p>
            Adding permission for {selectedEmployee && selectedEmployee.firstName}{" "}
            {selectedEmployee && selectedEmployee.lastName}
          </p>
          <input type="text" placeholder="Enter permissions" />
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>
            Cancel
          </Button>
          <Button variant="primary" onClick={() => handleModalSubmit("Admin")}>
            Save
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default ManageStoreEmployeesPage;
