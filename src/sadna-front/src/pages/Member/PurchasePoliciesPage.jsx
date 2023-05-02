
import { Container, ListGroup } from 'react-bootstrap';
import React, { useState } from 'react';
import { Button, Modal, Form } from 'react-bootstrap';

function PurchasePoliciesPage() {
  const [showModal, setShowModal] = useState(false);
  const [entity, setEntity] = useState('');
  const [EntityChoice, setEntityChoice] = useState('');
  const [WhichCond, setWhichCond] = useState('');
  const [condValue, setCondValue] = useState('');
  const [entity2, setEntity2] = useState('');
  const [EntityChoice2, setEntityChoice2] = useState('');
  const [WhichCond2, setWhichCond2] = useState('');
  const [condValue2, setCondValue2] = useState('');
  const [selectedOption, setSelectedOption] = useState('');

  const [showTextbox, setShowTextbox] = useState(false);

  const handleButtonClick = () => {
    setShowTextbox(!showTextbox);
  }
  
  const handleOptionChange = (event) => {
    setSelectedOption(event.target.value);
  }

  const openModal = () => {
    setShowModal(true);
  };

  const closeModal = () => {
    setShowModal(false);
  };


  const handleSubmit = () => {
    // Handle form submission here
    console.log('Submitted!');
  };
return (
    <div className="container mt-5">
        <ListGroup>
          <h5>Create New Store Policy</h5>
         </ListGroup>
         <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
      <Button variant="primary" onClick={openModal} style={{ marginLeft: '10px' }}>
        Add Condition
      </Button>
       <span>                                               </span>
      <Button onClick={handleButtonClick} style={{ marginLeft: '10px' }}>
        Remove Condition
      </Button>
      {showTextbox && (
        <div>
          <label>Enter condition's ID:</label>
          <input type="text" />
        </div>
      )}
      </div>
      <p></p>

      <table className="table" style={{background: "white"}}>
        <thead>
          <tr>
            <th>Number</th>
            <th>Entity</th>
            <th>ID</th>
            <th>Term</th>
            <th>Term Value</th>
            <th>Operator on next Condition</th>
          </tr>
        </thead>
        <tbody>
        </tbody>
      </table>

      <Modal show={showModal} onHide={closeModal}>
        <Modal.Header closeButton>
          <Modal.Title>Add Condition</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
          <Form.Group>
              <Form.Label>Entity</Form.Label>
              <Form.Control
                as="Select"
                value={EntityChoice}
                onChange={(e) => setEntityChoice(e.target.value)}
              >
                <option>Store</option>
                <option>Shooping cart</option>
                <option>Category</option>
                <option>Item</option>
              </Form.Control>
            </Form.Group>
            <Form.Group>
              <Form.Label>Entity ID</Form.Label>
              <Form.Control
                type="ID"
                placeholder="Enter entity's ID"
                value={entity}
                onChange={(e) => setEntity(e.target.value)}
              />
            </Form.Group>
            <Form.Group>
              <Form.Label>Which Condition</Form.Label>
              <Form.Control
                as="Select"
                value={WhichCond}
                onChange={(e) => setWhichCond(e.target.value)}
              >
                <option>Select type</option>
                <option>Min Value</option>
                <option>Max Value</option>
                <option>Min Quantity</option>
                <option>Max Quantity</option>
                <option>Time Condition</option>
              </Form.Control>
            </Form.Group>
            <Form.Group>
              <Form.Label>Condition Value</Form.Label>
              <Form.Control
                type="value"
                placeholder="Enter value"
                value={condValue}
                onChange={(e) => setCondValue(e.target.value)}
              />
            </Form.Group>
            </Form>
            <Form.Group>
            <Form.Label>Operator</Form.Label>
            <Form.Control as="select" onChange={handleOptionChange}>
            <option value="">Select an option</option>
            <option value="option1">AND</option>
            <option value="option2">OR</option>
            <option value="option3">Conditional</option>
          </Form.Control>
          </Form.Group>
          {selectedOption === 'option3' && (
            <div>
                    <Form.Group>
                      <Form.Label>Entity</Form.Label>
                      <Form.Control
                        as="Select"
                        value={EntityChoice2}
                        onChange={(e) => setEntityChoice2(e.target.value)} >
                        <option>Store</option>
                        <option>Shooping cart</option>
                        <option>Category</option>
                        <option>Item</option>
                      </Form.Control>
                    </Form.Group>
                    <Form.Group>
                      <Form.Label>Entity ID</Form.Label>
                      <Form.Control
                        type="ID"
                        placeholder="Enter entity's ID"
                        value={entity2}
                        onChange={(e) => setEntity2(e.target.value)}
                      />
                    </Form.Group>
                    <Form.Group>
                      <Form.Label>Which Condition</Form.Label>
                      <Form.Control
                        as="Select"
                        value={WhichCond2}
                        onChange={(e) => setWhichCond2(e.target.value)}
                      >
                        <option>Select type</option>
                        <option>Min Value</option>
                        <option>Max Value</option>
                        <option>Min Quantity</option>
                        <option>Max Quantity</option>
                        <option>Time Condition</option>
                      </Form.Control>
                    </Form.Group>
                    <Form.Group>
                      <Form.Label>Condition Value</Form.Label>
                      <Form.Control
                        type="value"
                        placeholder="Enter value"
                        value={condValue2}
                        onChange={(e) => setCondValue2(e.target.value)}
                      />
                    </Form.Group>
            </div>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={closeModal}>
            Close
          </Button>
          <Button variant="primary" onClick={handleSubmit}>
            Create
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
}
export default PurchasePoliciesPage;