
import { Container, ListGroup } from 'react-bootstrap';
import React, { useState,useEffect } from 'react';
import { Button, Modal, Form , Row, Col } from 'react-bootstrap';
import Exit from "../Exit.tsx";
import { useLocation } from 'react-router-dom';
import { handleAddPurchaseCondition, handleGetAllPurchaseConditions, handleRemovePurchaseCondition } from '../../actions/MemberActions.tsx';
import { PurcahseCondition , Item , Store, Policy} from '../../models/Shop.tsx';
import { DiscountCondition } from '../../components/DiscountCondition.tsx';


function PurchasePoliciesPage(props) {
  const location = useLocation();
  const { userId, storeId } = location.state;

  const [policysList, setPolicyList] = useState<Policy[]>([]);


  const [showModal, setShowModal] = useState(false);
  const [entity, setEntity] = useState('');
  const [EntityChoice, setEntityChoice] = useState('');
  const [WhichCond, setWhichCond] = useState('');
  const [condValue, setCondValue] = useState('');
  const [entity2, setEntity2] = useState('');
  const [EntityChoice2, setEntityChoice2] = useState('');
  const [otherCondition, setOtherCondition] = useState('');
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


  const GetPurcahsePolicys =()=>{

    handleGetAllPurchaseConditions(storeId).then(
      value => {
        console.log(value)
        setPolicyList(value as Policy[]);
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    GetPurcahsePolicys();
 }, [])

  const handleSubmit = () => {
    handleAddPurchaseCondition(storeId ,EntityChoice, entity,WhichCond,condValue,selectedOption,otherCondition).then(
      value => {
        setPolicyList(value as Policy[]);
        setWhichCond("");
        setEntity("");
        setEntityChoice("");
        setCondValue("");
        setOtherCondition("");
        setShowModal(false);
      })
      .catch(error => alert(error));
  };

return (
    <div className="container mt-5">
      <Exit id={props.id}/>
      <ListGroup>
        <h5>Create New Store Policy</h5>
      </ListGroup>
      <div style={{ display: 'flex', alignItems: 'center', gap: '10px' }}>
      <Button variant="dark"  onClick={openModal} style={{ marginLeft: '10px' }}>
        Add Condition
      </Button>
       <span>                                               </span>
      {showTextbox && (
        <div>
          <label>Enter condition's ID:</label>
          <input type="text" />
        </div>
      )}
      </div>
      <p></p>

                <Row className="mt-3">
          {policysList.length===0? (<div>  No Items </div>): (policysList.map((cond) => (
            // <div key={item.name}>{item.name} </div>
            <Col  key={cond.policyID} className="mt-3">
              <DiscountCondition { ...cond } handleRemove={GetPurcahsePolicys} storID={storeId} ></DiscountCondition>
            </Col>
            )))}
        </Row>
        



      <Modal show={showModal} onHide={closeModal}>
        <Modal.Header closeButton>
          <Modal.Title>Add Condition</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
          <Form.Group>
              <Form.Label>Entity</Form.Label>
              <Form.Control
                as="select"
                value={EntityChoice}
                onChange={(e) => setEntityChoice(e.target.value)}
              >
                <option>Type</option>
                <option>Store</option>
                <option>Category</option>
                <option>Item</option>
              </Form.Control>
            </Form.Group>
            {EntityChoice != 'Store' && (
            <Form.Group>
              <Form.Label>Entity Name</Form.Label>
              <Form.Control
                type="ID"
                placeholder="Enter entity's name"
                value={entity}
                onChange={(e) => setEntity(e.target.value)}
              />
            </Form.Group>
            )}
            <Form.Group>
              <Form.Label>Which Condition</Form.Label>
              <Form.Control
                as="select"
                value={WhichCond}
                onChange={(e) => setWhichCond(e.target.value)}
              >
                <option>Select type</option>
                <option>min value</option>
                <option>max value</option>
                <option>min quantity</option>
                <option>max quantity</option>
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
            <option value="option">Only condition</option>
            <option value="and">AND</option>
            <option value="or">OR</option>
            <option value="if">IF</option>
          </Form.Control>
          </Form.Group>
          {(selectedOption === 'and'|| selectedOption === 'or'|| selectedOption == "if") && (<div>
            <Form.Group>
                      <Form.Label>Other condition</Form.Label>
                      <Form.Control
                        type="text"
                        placeholder="Enter the condition to build with"
                        value={otherCondition}
                        onChange={(e) => setOtherCondition(e.target.value)}
                      />
                    </Form.Group>
          </div>)}
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