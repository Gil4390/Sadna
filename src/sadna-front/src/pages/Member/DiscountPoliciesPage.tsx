
import { Container, ListGroup } from 'react-bootstrap';
import React, { useState } from 'react';
import { Button, Modal, Form , Row, Col, Card } from 'react-bootstrap';
import { useNavigate } from "react-router-dom";
import { Condition } from '../../components/Condition';
import Exit from "../Exit.tsx";

function DiscountPoliciesPage() {
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
  const navigate = useNavigate();
  const items = [
    { id: 1 ,  entity:'Item' , entityID:11 , type:'min quantity' , value:0},
  ];
  const [allItems, setAllItems] = useState(items);
  const [showTable, setShowTable] = useState(false);

  const handleButtonClick = () => {
    setShowTable(!showTable);
  };
  const [showTable2, setShowTable2] = useState(false);

  const handleButtonClick2 = () => {
    setShowTable2(!showTable2);
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
                as="Select"
                value={EntityChoice}
                onChange={(e) => setEntityChoice(e.target.value)}
              >
                <option>Type</option>
                <option>Store</option>
                <option>Item</option>
                <option>Basket</option>
              </Form.Control>
            </Form.Group>
            <Form.Group>
              <Form.Control
                type="ID"
                placeholder="Enter ID"
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
        <input type="date" style={styles.input} />
        <input type="date" style={styles.input} />
        <div style={styles.buttonContainer}>
          <button style={styles.button}>Create</button>
        </div>
      </div>
      <div style={styles.form}>
      <h2>Create new Condition Policy</h2>
      <Form.Group>
              <Form.Control
                as="select"
                value={EntityChoice2}
                onChange={(e) => setEntityChoice2(e.target.value)}
              >
                <option>Type</option>
                <option>Store</option>
                <option>Item</option>
                <option>Basket</option>
              </Form.Control>
            </Form.Group>
            <Form.Group>
              <Form.Control
                type="ID"
                placeholder="Enter ID"
                value={entity2}
                onChange={(e) => setEntity2(e.target.value)}
              />
            </Form.Group>
            <Form.Group>
              <Form.Control
                type="value"
                placeholder="Amount"
                value={condValue2}
                onChange={(e) => setCondValue2(e.target.value)}
              />
            </Form.Group>
        <input type="date" style={styles.input} />
        <input type="date" style={styles.input} />
        <div style={styles.buttonContainer}>
          <button style={styles.button}>Create</button>
        </div>
      </div>
    </div>}
    {showTable2 && <div style={styles.container}>
      <h2>Compound Store Policies</h2>
      <div style={styles.form}>
      <Form.Group>
              <Form.Control
                as="Select"
                value={cond1_1}
                onChange={(e) => setCond1_1(e.target.value)}
              >
                <option>Choose condition</option>
              </Form.Control>
            </Form.Group>
            <Form.Group>
              <Form.Control
                type="text"
                placeholder="Operator"
                value={op_1}
                onChange={(e) => setOp_1(e.target.value)}
              />
            </Form.Group>
            <Form.Group>
            <Form.Control
                as="Select"
                value={cond2_1}
                onChange={(e) => setCond2_1(e.target.value)}
              >
                <option>Choose condition</option>
              </Form.Control>
            </Form.Group>
            <Form.Group>
            <Form.Control
                as="select"
                value={policy1_3}
                onChange={(e) => setPolicy_1(e.target.value)}
              >
                <option>Choose policy</option>
              </Form.Control>
            </Form.Group>
        <div style={styles.buttonContainer}>
          <button style={styles.button}>Create</button>
        </div>
      </div>
      <div style={styles.form}>
      <Form.Group>
              <Form.Control
                as="select"
                value={cond2_2}
                onChange={(e) => setCond2_2(e.target.value)}
              >
                <option>Choose condition</option>
              </Form.Control>
            </Form.Group>
            <Form.Group>
            <Form.Control
                as="select"
                value={policy_2}
                onChange={(e) => setPolicy_2(e.target.value)}
              >
                <option>Choose policy</option>
              </Form.Control>
            </Form.Group>
        <div style={styles.buttonContainer}>
          <button style={styles.button}>Create</button>
        </div>
      </div>
      <div style={styles.form}>
      <Form.Group>
              <Form.Control
                as="select"
                value={policy1_3}
                onChange={(e) => setPolicy1_3(e.target.value)}
              >
                <option>Choose condition</option>
              </Form.Control>
            </Form.Group>
            <Form.Group>
            <Form.Control
                as="select"
                value={policy2_3}
                onChange={(e) => setPolicy2_3(e.target.value)}
              >
                <option>Choose policy</option>
              </Form.Control>
            </Form.Group>
        <div style={styles.buttonContainer}>
          <button style={styles.button}>Create</button>
        </div>
      </div>
    </div>}
      <Row className="mt-3">
          {allItems.map((item) => (
            <Col sm={8} md={5} lg={4} xl={3} key={item.id} className="mt-3">
              <Condition {...item}></Condition>
            </Col>
            ))}
        </Row>
    </div>
  );
}


export default DiscountPoliciesPage;