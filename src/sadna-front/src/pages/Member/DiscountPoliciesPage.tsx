import { Container, ListGroup } from 'react-bootstrap';
import React, { useState,useEffect } from 'react';
import { Button, Modal, Form , Row, Col, Card } from 'react-bootstrap';
import { useNavigate } from "react-router-dom";
import Exit from "../Exit.tsx";
import { handleAddPolicy, handleGetAllPolicy ,handleCreateSimplePolicy,handleCreateComplexPolicy , handleAddDiscountCondition} from '../../actions/MemberActions.tsx';
import { useLocation } from 'react-router-dom';
import { Policy } from '../../models/Shop.tsx';
import { DiscountCondition , } from  '../../components/DiscountCondition.tsx';
import { ResponseT,Response } from '../../models/Response.tsx';


function DiscountPoliciesPage(props) {
  
  const styles = {
    container: {
      background: 'white',
      width: 'fit-content',
      borderRadius: '8px',
      boxShadow: '0px 2px 4px rgba(0, 0, 0, 0.1)',
      padding: '16px',
      boxSizing: 'border-box',
    },
    form: {
      display: 'flex',
      flexDirection: 'row',
      flexWrap: 'wrap',
      alignItems: 'center',
      justifyContent: 'space-between',
    },
    select: {
      flex: 1,
      margin: '8px',
      padding: '8px',
      borderRadius: '4px',
    },
    input: {
      flex: 1,
      margin: '8px',
      padding: '8px',
      borderRadius: '4px',
      border: '1px solid #ccc',
      minWidth: '200px',
    },
    buttonContainer: {
      display: 'flex',
      justifyContent: 'flex-end',
      marginTop: '10px',
    },
    button: {
      padding: '8px 16px',
      borderRadius: '4px',
      background: '#4caf50',
      color: 'white',
      border: 'none',
      cursor: 'pointer',
      fontSize: '14px',
      fontWeight: 'bold',
    },
  };
  
  const location = useLocation();
  const { userId, storeId } = location.state;
  const [entity2, setEntity2] = useState('');
  const [EntityChoice2, setEntityChoice2] = useState('');

  const [cond1_1, setCond1_1] = useState('');
  const [op_1, setOp_1] = useState('');
  const [op_2, setOp_2] = useState('');

  const [cond2_1, setCond2_1] = useState('');
  const [policy_1, setPolicy_1] = useState('');
  const [cond2_2, setCond2_2] = useState('');
  const [policy_2, setPolicy_2] = useState('');
  const [policy1_3, setPolicy1_3] = useState('');
  const [op_3, setOp_3] = useState('');
  const [policy2_3, setPolicy2_3] = useState('');

  const [condValue2, setCondValue2] = useState('');
  const [entity, setEntity] = useState('');
  const [EntityChoice, setEntityChoice] = useState('');
  const [condValue, setCondValue] = useState('');
  const [showTable, setShowTable] = useState(false);

  const [showModal, setShowModal] = useState(false);
  const [entityCondInDiscount, setentityCondInDiscount] = useState('');
  const [EntityChoiceInDiscount, setEntityChoiceInDiscount] = useState('');
  const [WhichCondInDiscount, setWhichCondInDiscount] = useState('');
  const [condValueInDiscount, setcondValueInDiscount] = useState('');
  const [policysList, setPolicyList] = useState<Policy[]>([]);
  const [policyResponse, setPolicyResponse]=useState<ResponseT>();
  const [showTable2, setShowTable2] = useState(false);

  const [startDate, setSelectedDate] = useState('');
  const [endDate, setSelectedDate2] = useState('');
  const [selectedOption, setSelectedOption] = useState('');
  const [otherCondInDiscount, setotherCondInDiscount] = useState('');

  const [showTextbox, setShowTextbox] = useState(false);

  const handleButtonClick = () => {
    setShowTable(!showTable);
  };

  const handleButtonClick2 = () => {
    setShowTable2(!showTable2);
  };
 
  

 const filteredList = policysList.filter((item) => item.type === "Condition");

 const condFilter = filteredList.map((item2) => (
   <option key={item2.policyID} value={item2.policyID}>
     {item2.policyID}
   </option>
 ));

 const filteredList2 = policysList.filter((item) => item.type === "Policy");
 
 const policyFilter = filteredList2.map((item2) => (
   <option key={item2.policyID} value={item2.policyID}>
     {item2.policyID}
   </option>
 ));

  const GetDiscountPolicy =()=>{
    handleGetAllPolicy(userId ,storeId).then(
      value => {
        setPolicyList(value as Policy[]);
      })
      .catch(error => alert(error));
  }

 useEffect(() => {
    GetDiscountPolicy();
 }, [ ])

 function handleDateChange(event) {
    setSelectedDate(event.target.value);
  }

  function handleDateChange2(event) {
    setSelectedDate2(event.target.value);
  }


  const Create_new_Simple_Discount_Policy = () => {
    setShowTable(!showTable);
    handleCreateSimplePolicy(userId,storeId ,EntityChoice+entity,condValue,startDate,endDate).then(
        value => {
          setSelectedDate('')
          setSelectedDate2('')
          setCondValue('')
          setEntity('')
          setEntityChoice('')
          setPolicyResponse(value as ResponseT);
        })
        .catch(error => alert(error));
  };

  const Create_new_Condition_Policy_1 = () => {
    setShowTable2(!showTable);
    handleCreateComplexPolicy(userId,storeId ,op_1,[cond1_1,cond2_1,policy_1]).then(
        value => {
          setPolicyResponse(value as ResponseT);
        })
        .catch(error => alert(error));
  };

  const Create_new_Condition_Policy_2 = () => {
    setShowTable2(!showTable);
    handleCreateComplexPolicy(userId,storeId ,op_2,[cond2_2,policy_2]).then(
        value => {
          setPolicyResponse(value as ResponseT);
        })
        .catch(error => alert(error));
  };

  const Create_new_Condition_Policy_3 = () => {
    setShowTable2(!showTable);
    handleCreateComplexPolicy(userId,storeId ,op_3,[policy1_3,policy2_3]).then(
        value => {
          setPolicyResponse(value as ResponseT);
        })
        .catch(error => alert(error));
  };

  useEffect(() => {
    if(policyResponse!=undefined)
      if(policyResponse?.errorOccured)
        alert(policyResponse?.errorMessage) 
      else{
        GetDiscountPolicy();
        setTimeout(() => {
          alert("New Policy added successfully")
        }, 0);

      }
      setPolicyResponse(undefined);
  }, [policyResponse])


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
    handleAddDiscountCondition(userId , storeId ,entityCondInDiscount, EntityChoiceInDiscount,WhichCondInDiscount,condValueInDiscount,selectedOption,"","","","",otherCondInDiscount).then(
      value => {
        setPolicyResponse(value as ResponseT);
        // setPolicyList(value as Policy[]);
        setentityCondInDiscount("");
        setEntityChoiceInDiscount("");
        setEntityChoiceInDiscount("");
        setWhichCondInDiscount("");
        setcondValueInDiscount("")
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
         <Button variant="dark"  onClick={openModal} style={{ marginLeft: '10px' }}>Create Condition</Button>
      <Button variant="dark"  onClick={handleButtonClick} style={{ marginLeft: '10px' }}>
        Create Policy  {showTable ? ' ' : ' '}
      </Button>
       <span>                                               </span>
      <Button variant="dark" onClick={handleButtonClick2} style={{ marginLeft: '10px' }}>
        Compund Policy
      </Button>


      </div>
      <p></p>
      {showTable && <div style={styles.container}>
        <h2>Create new Simple Discount Policy</h2>
      <div style={styles.form}>
      <Form.Group>
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
                  {/* <Form.Label>{EntityChoice} Name:</Form.Label> */}
                  <Form.Control
                    type="ID"
                    placeholder="EnterName"
                    value={entity}
                    onChange={(e) => setEntity(e.target.value)}
                    required
                  />
                </Form.Group>
              )}
            <Form.Group>
              <Form.Control
                type="value"
                placeholder="Amount"
                value={condValue}
                onChange={(e) => setCondValue(e.target.value)}
                required
              />
            </Form.Group>
        <input type="date" value={startDate} style={styles.input} onChange={handleDateChange} required/>
        <input type="date" value={endDate} style={styles.input} onChange={handleDateChange2} min={startDate} required/>
        <div style={styles.buttonContainer}>
          <button style={styles.button} onClick={Create_new_Simple_Discount_Policy}>Create</button>
        </div>
      </div>
    </div>}
    {showTable2 && <div style={styles.container}>
      <h2>Compound Store Policies</h2>
      <div style={styles.form}>
      <Form.Group>
              <Form.Control
                as="select"
                value={cond1_1}
                onChange={(e) => setCond1_1(e.target.value)}
              >
                <option>Choose condition</option>
                {condFilter}
              </Form.Control>
            </Form.Group>
            <Form.Group>
              <Form.Control
                as="select"
                value={op_1}
                onChange={(e) => setOp_1(e.target.value)}
              >
                <option>Operator</option>
                <option>and</option>
                <option>or</option>
                <option>xor</option>
                <option>if</option>
              </Form.Control>
            </Form.Group>
            <Form.Group>
            <Form.Control
                as="select"
                value={cond2_1}
                onChange={(e) => setCond2_1(e.target.value)}
              >
                <option>Choose condition</option>
                {condFilter}
              </Form.Control>
            </Form.Group>
            <Form.Group>
            <Form.Control
                as="select"
                value={policy_1}
                onChange={(e) => setPolicy_1(e.target.value)}
              >
                <option>Choose policy</option>
                {policyFilter}
              </Form.Control>
            </Form.Group>
        <div style={styles.buttonContainer}>
          <button style={styles.button} onClick={Create_new_Condition_Policy_1}>Create</button>
        </div>
      </div>
      <div style={styles.form}>
      <Form.Group>
              <Form.Control
                as="select"
                value={op_2}
                onChange={(e) => setOp_2(e.target.value)}
              >
                <option>Operator</option>
                <option>if</option>
              </Form.Control>
            </Form.Group>
      <Form.Group>
              <Form.Control
                as="select"
                value={cond2_2}
                onChange={(e) => setCond2_2(e.target.value)}
              >
                <option>Choose condition</option>
                {condFilter}
              </Form.Control>
            </Form.Group>
            <Form.Group>
            <Form.Control
                as="select"
                value={policy_2}
                onChange={(e) => setPolicy_2(e.target.value)}
              >
                <option>Choose policy</option>
                {policyFilter}
              </Form.Control>
            </Form.Group>
        <div style={styles.buttonContainer}>
          <button style={styles.button} onClick={Create_new_Condition_Policy_2}>Create</button>
        </div>
      </div>
      <div style={styles.form}>
      <Form.Group>
              <Form.Control
                as="select"
                value={policy1_3}
                onChange={(e) => setPolicy1_3(e.target.value)}
              >
                <option>Choose policy</option>
                {policyFilter}
              </Form.Control>
            </Form.Group>
            <Form.Group>
              <Form.Control
                as="select"
                value={op_3}
                onChange={(e) => setOp_3(e.target.value)}
              >
                <option>Operator</option>
                <option>add</option>
                <option>max</option>
              </Form.Control>
            </Form.Group>
            <Form.Group>
            <Form.Control
                as="select"
                value={policy2_3}
                onChange={(e) => setPolicy2_3(e.target.value)}
              >
                <option>Choose policy</option>
                {policyFilter}
              </Form.Control>
            </Form.Group>
        <div style={styles.buttonContainer}>
          <button style={styles.button} onClick={Create_new_Condition_Policy_3}>Create</button>
        </div>
      </div>
    </div>}
         <Row className="mt-3">
          {policysList.length===0? (<div>  There is no policies </div>): (policysList.map((cond) => (
            // <div key={item.name}>{item.name} </div>
            <Col  key={(cond.policyID+cond.type)} className="mt-3">
              <DiscountCondition obj={cond} onPolicyChanged={GetDiscountPolicy} purchase={false} storeID={storeId} userID={userId}  ></DiscountCondition>
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
                value={entityCondInDiscount}
                onChange={(e) => setentityCondInDiscount(e.target.value)}
              >
                <option>Type</option>
                <option>Store</option>
                <option>Category</option>
                <option>Item</option>
              </Form.Control>
            </Form.Group>
            {entityCondInDiscount != 'Store' && entityCondInDiscount != 'Type' && (
                          <Form.Group>
                          <Form.Label>Entity Name</Form.Label>
                          <Form.Control
                            type="ID"
                            placeholder="Enter entity's name"
                            value={EntityChoiceInDiscount}
                            onChange={(e) => setEntityChoiceInDiscount(e.target.value)}
                          />
                        </Form.Group>
            )}
            <Form.Group>
              <Form.Label>Which Condition</Form.Label>
              <Form.Control
                as="select"
                value={WhichCondInDiscount}
                onChange={(e) => setWhichCondInDiscount(e.target.value)}
              >
                <option>Select type</option>
                <option>min value</option>
                <option>max value</option>
                <option>min quantity</option>
                <option>max quantity</option>
                <option>before date</option>
                <option>after date</option>
              </Form.Control>
            </Form.Group>
            {(WhichCondInDiscount == 'before date' || WhichCondInDiscount == 'after date' ) ? (<Form.Group>
              <Form.Label>Condition Value</Form.Label>
              <Form.Control
                type="date"
                placeholder="Enter value"
                value={condValueInDiscount}
                onChange={(e) => setcondValueInDiscount(e.target.value)}
              />
            </Form.Group>):(<Form.Group>
              <Form.Label>Condition Value</Form.Label>
              <Form.Control
                type="value"
                placeholder="Enter value"
                value={condValueInDiscount}
                onChange={(e) => setcondValueInDiscount(e.target.value)}
              />
            </Form.Group>)}

            </Form>
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


export default DiscountPoliciesPage;