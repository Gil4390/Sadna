
import { Container, ListGroup } from 'react-bootstrap';
import React, { useState,useEffect } from 'react';
import { Button, Modal, Form , Row, Col, Card } from 'react-bootstrap';
import { useNavigate } from "react-router-dom";
import Exit from "../Exit.tsx";
import { handleAddPolicy, handleGetAllPolicy ,handleCreateSimplePolicy,handleCreateComplexPolicy} from '../../actions/MemberActions.tsx';
import { useLocation } from 'react-router-dom';
import { Policy} from '../../models/Shop.tsx';
import { DiscountCondition } from  '../../components/DiscountCondition.tsx';



function DiscountPoliciesPage(props) {

  const location = useLocation();
  const { userId, storeId } = location.state;

  
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

  const handleButtonClick = () => {
    setShowTable(!showTable);
  };
  const [showTable2, setShowTable2] = useState(false);

  const handleButtonClick2 = () => {
    setShowTable2(!showTable2);
  };
  const [policysList, setPolicyList] = useState<Policy[]>([]);
  

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
        console.log(value)
        setPolicyList(value as Policy[]);
      })
      .catch(error => alert(error));
  }
  useEffect(() => {
    GetDiscountPolicy();
 }, [])
 useEffect(() => {
    GetDiscountPolicy();
 }, [])

 const [startDate, setSelectedDate] = useState('');
 const [endDate, setSelectedDate2] = useState('');

 function handleDateChange(event) {
    setSelectedDate(event.target.value);
  }
  function handleDateChange2(event) {
    setSelectedDate2(event.target.value);
  }
 

  const Create_new_Simple_Discount_Policy = () => {
    handleCreateSimplePolicy(storeId ,entity,condValue,startDate,endDate).then(
        value => {
          setPolicyList(value as Policy[]);
        })
        .catch(error => alert(error));
  };
  const Create_new_Condition_Policy_1 = () => {
    handleCreateComplexPolicy(storeId ,op_1,[cond1_1,cond2_1,policy_1]).then(
        value => {
          setPolicyList(value as Policy[]);
        })
        .catch(error => alert(error));
  };
  const Create_new_Condition_Policy_2 = () => {
    handleCreateComplexPolicy(storeId ,op_2,[cond2_2,policy_2]).then(
        value => {
          setPolicyList(value as Policy[]);
        })
        .catch(error => alert(error));
  };
  const Create_new_Condition_Policy_3 = () => {
    handleCreateComplexPolicy(storeId ,op_3,[policy1_3,policy2_3]).then(
        value => {
          setPolicyList(value as Policy[]);
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
      <Button variant="dark"  onClick={handleButtonClick} style={{ marginLeft: '10px' }}>
        Create  {showTable ? ' ' : ' '}
      </Button>
       <span>                                               </span>
      <Button variant="dark" onClick={handleButtonClick2} style={{ marginLeft: '10px' }}>
        Compund
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
            <Form.Group>
              <Form.Control
                type="ID"
                placeholder="Enter Name"
                value={entity}
                onChange={(e) => setEntity(e.target.value)}
              />
            </Form.Group>
            <Form.Group>
              <Form.Control
                type="value"
                placeholder="Amount"
                value={condValue}
                onChange={(e) => setCondValue(e.target.value)}
              />
            </Form.Group>
        <input type="date" value={startDate} style={styles.input} onChange={handleDateChange}/>
        <input type="date" value={endDate} style={styles.input} onChange={handleDateChange2}/>
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
                <option>and</option>
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
          {policysList.length===0? (<div>  No Items </div>): (policysList.map((cond) => (
            // <div key={item.name}>{item.name} </div>
            <Col  key={cond.policyID} className="mt-3">
              <DiscountCondition { ...cond } handleRemove={GetDiscountPolicy}></DiscountCondition>
            </Col>
            )))}
        </Row>
    </div>
  );
}


export default DiscountPoliciesPage;