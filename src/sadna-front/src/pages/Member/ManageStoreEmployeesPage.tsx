import React, { useState, useEffect } from "react";
import { Button, Modal, Table, Form } from "react-bootstrap";
import Exit from "../Exit.tsx";
import { Member } from "../../models/User.tsx";
import { useLocation } from 'react-router-dom';
import { handleAddStoreManagerPermission,handleRemoveStoreManagerPermission, handleEmployeesOfStore
,handleAppointStoreOwner,handleAppointStoreManager, handleReactToJobOffer } from "../../actions/MemberActions.tsx";
import { ResponseT,Response } from "../../models/Response.tsx";

const ManageStoreEmployeesPage = (props) => {

  const location = useLocation();
  const { userId, storeId } = location.state;
  const [employees, setEmployees] = useState<Member[]>([]);

  const [showEditModal, setShowEditModal] = useState(false);
  const [editPermission, setEditPermission] = useState<string>('owner permissions');

  const [showAddModal, setShowAddModal] = useState(false);
  const [appointEmail, setAppointEmail] = useState<string>('');
  const [appointType, setAppointType] = useState<string>('Appoint store owner');
  
  const [selectedEmployee, setSelectedEmployee] = useState<Member>();
  const [getEmployeesResponse, setGetEmployeesResponse]=useState<ResponseT>();

  const [editPermissionResponse, setEditPermissionResponse]=useState<Response>();
  const [addEmployeeResponse, setAddEmployeeResponse]=useState<Response>();

  const getStoreEmployees =()=>{
    handleEmployeesOfStore(userId,storeId).then(
      value => {
        setGetEmployeesResponse(value as ResponseT);
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    getStoreEmployees();
 }, [])

useEffect(() => {
  if(getEmployeesResponse !=undefined){
    getEmployeesResponse?.errorOccured ? alert(getEmployeesResponse?.errorMessage) : setEmployees(getEmployeesResponse?.value as Member[]);
  }
}, [getEmployeesResponse])

  const handleEditModalClose = () => {setShowEditModal(false); setEditPermission('owner permissions');}

  const handleAddModalClose = () => {setShowAddModal(false); setAppointEmail(''); setAppointType('Appoint store owner');}
  const handleAddModalShow = () => setShowAddModal(true);


  const handleAddPermission = (event) => {
    event.preventDefault();
      handleAddStoreManagerPermission(userId, storeId,selectedEmployee?.email, editPermission ).then(
        value => {
          setEditPermissionResponse(value as Response);
        })
        .catch(error => alert(error));
  };

  const handleRemovePermission = (event) => {
    event.preventDefault();
    handleRemoveStoreManagerPermission(userId, storeId,selectedEmployee?.email, editPermission ).then(
      value => {
        setEditPermissionResponse(value as Response);
      })
      .catch(error => alert(error));
  };

  const handleEditPermission = (employee) => {
    setSelectedEmployee(employee);
    setShowEditModal(true);
  };

  useEffect(() => {
    if(editPermissionResponse !=undefined){
      editPermissionResponse?.errorOccured ? alert(editPermissionResponse?.errorMessage) : EditPermissionSuccess();
    }
  }, [editPermissionResponse])
  
  const EditPermissionSuccess=()=>{
    getStoreEmployees();
    handleEditModalClose();
    alert("Permissions edited suucessfully")
  }

  const isValidEmail = (email) => 
  {
    // regex for email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    // test email againt regex pattern
    return emailRegex.test(email);
  }

  const handleAddEmployee = (event) => {
    event.preventDefault();
    if(isValidEmail(appointEmail))
    {
      if(appointType==="Appoint store owner"){
        handleAppointStoreOwner(userId, storeId,appointEmail).then(
          value => {
            
            setAddEmployeeResponse(value as Response);
          })
          .catch(error => alert(error));
      }
      else if(appointType==="Appoint store manager"){
        handleAppointStoreManager(userId, storeId,appointEmail).then(
          value => {
            setAddEmployeeResponse(value as Response);
          })
          .catch(error => alert(error));
        }
    }
  };

  useEffect(() => {
    if(addEmployeeResponse !=undefined){
      addEmployeeResponse?.errorOccured ? alert(addEmployeeResponse?.errorMessage) : AddEmployeeSuccess();
    }
  }, [addEmployeeResponse])
  
  const AddEmployeeSuccess=()=>{
    handleAddModalClose();
    getStoreEmployees();
    alert("Permission added suucessfully to user") 
  }

  const handlePermissionChange = (event) => {
    setEditPermission(event.target.value);
  }

  const handleAppointTypeChange = (event) => {
    setAppointType(event.target.value);
  }

  const handleAppointEmailChange = (event) => {
    setAppointEmail(event.target.value);
  }



  const reactToJobOffer = (memberID, res) => {
    handleReactToJobOffer(userId, storeId, memberID, res).then(
      value => {
        getStoreEmployees();
      })
      .catch(error => alert(error));
  };
  

  return (
    <div>
      <Exit id={props.id}/>

      <h1>Employees</h1>
      <Button variant="primary" onClick={handleAddModalShow} style={{margin: "0.5rem"}}>
        Add New Employee
      </Button>

      <Modal show={showAddModal} onHide={() =>handleAddModalClose()}>
      <Modal.Header closeButton>
        <Modal.Title>Add Employee</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form >
          <Form.Group controlId="email">
            <Form.Label>Employee email in system</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter employee email"
              value={appointEmail}
              onChange={handleAppointEmailChange}
              style={{borderColor: isValidEmail(appointEmail) || appointEmail.length === 0 ? '#28a745' : '#dc3534'}} />
              {!isValidEmail(appointEmail) && appointEmail.length > 0 && <Form.Text className='text-danger'>Not Valid Email! Try Again!</Form.Text>}
          </Form.Group>
          <span className="fs-2">Choose Appoint type:</span>
          <Form.Control as="select" value={appointType} onChange={handleAppointTypeChange}>
            <option value="Appoint store owner">Appoint store owner</option>
            <option value="Appoint store manager">Appoint store manager</option>
          </Form.Control>
          <Button variant="primary" style={{margin: "0.5rem"}} onClick={handleAddEmployee}>
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
          <th>Email</th>
          <th>Status</th>
          <th>Permissions</th>
          </tr>
        </thead>
        <tbody>
        {employees.map((member) => (
          <tr key={member.id}>
            <td>{member.firstName}</td>
            <td>{member.lastName}</td>
            <td>{member.email}</td>
            <td>{member.didApprove ? (
              <div> 
                {member.approvers.length === 0 ? <span>Employed</span> : 
                <span>
                  <strong>Approvers Left: </strong>
                  <div>
                    {member.approvers.map((approver, index) => (<div key={index}> {approver} </div>))}
                  </div>
                </span>
                }
              </div>) : (
              <div>
                <Button variant="success" onClick={() => reactToJobOffer(member.id, true)} style={{margin: "0.3rem"}}>
                  Approve 
                </Button>
                <Button variant="danger" onClick={() => reactToJobOffer(member.id, false)} style={{margin: "0.3rem"}}>
                  Reject 
                </Button>
              </div>)}
            </td>
            <td>{member.permissions?.map((p) => (
              <div> {p} </div>
            ))}</td>
          <td>
            <Button
                  variant="primary"
                  onClick={() => handleEditPermission(member)}
                >
                  Edit Permissions
                </Button>
             </td>
          </tr>
        ))}
      </tbody>
      </Table>
      <Modal show={showEditModal} onHide={() => handleEditModalClose()}>
        <Modal.Header closeButton>
          <Modal.Title>Edit Permissions</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <p>
            Editing permissions for {selectedEmployee && selectedEmployee.email}
          </p>
         
          <span className="fs-2">Choose Permission:</span>
          <Form.Control as="select" value={editPermission} onChange={handlePermissionChange}>
            <option value="owner permissions">owner permissions</option>
            <option value="edit manager permissions">edit manager permissions</option>
            <option value="get store history">get store history</option>
            <option value="add new owner">add new owner</option>
            <option value="remove owner">remove owner</option>
            <option value="add new manager">add new manager</option>
            <option value="get employees info">get employees info</option>
            <option value="product management permissions">product management permissions</option>
            <option value="policies permission">policies permission</option>
          </Form.Control>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => handleEditModalClose()}>
            Cancel
          </Button>
          <Button variant="primary" onClick={handleAddPermission}>
            Add Permission
          </Button>
          <Button variant="primary" onClick={handleRemovePermission}>
            Remove Permission
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default ManageStoreEmployeesPage;
