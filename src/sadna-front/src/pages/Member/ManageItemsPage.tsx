import React, { useState } from 'react';
import { Table, Button, Modal, Form } from 'react-bootstrap';
import Exit from "../Exit.tsx";

type Item = {
  id: number,
  name: string,
  price: number,
  category : string,
  quantity : number
}

const items:Item[] = [
  { id: 1, name: 'Item 1', price: 10, category: 'Category 1', quantity: 5 },
  { id: 2, name: 'Item 2', price: 20, category: 'Category 2', quantity: 10 },
  { id: 3, name: 'Item 3', price: 30, category: 'Category 3', quantity: 15 },
];

const ManageItemsPage = (props) => {
  const [showAddModal, setShowAddModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [editedItem, setEditedItem] = useState<Item>();
  const [itemsList, setItemsList] = useState(items);

  const handleAddModalClose = () => setShowAddModal(false);
  const handleAddModalShow = () => setShowAddModal(true);

  const handleEditModalClose = () => setShowEditModal(false);
  const handleEditModalShow = (item) => {
    setEditedItem(item);
    setShowEditModal(true);
  };

  const handleAddItem = (event) => {}
  // const handleAddItem = (event) => {
  //   event.preventDefault();
  //   const formData = new FormData(event.target);
  //   const newItem:Item = {
  //     id: Math.max(...itemsList.map((item) => item.id)) + 1,
  //     name: formData.get('name') as string,
  //     price: parseInt(formData.get('price')),
  //     category: formData.get('category') as string,
  //     quantity: formData.get('quantity') as number,
  //   };
   // setItemsList([...itemsList, newItem]);
    //setShowAddModal(false);
  //};

  const handleEditItem = (event) => {
    // event.preventDefault();
    // const formData = new FormData(event.target);
    // const updatedItem = {
    //   id: editedItem.id,
    //   name: formData.get('name'),
    //   price: formData.get('price'),
    //   category: formData.get('category'),
    //   quantity: formData.get('quantity'),
    // };
    // const updatedItemsList = itemsList.map((item) =>
    //   item.id === editedItem.id ? updatedItem : item
    // );
    // setItemsList(updatedItemsList);
    // setShowEditModal(false);
  };

  return (
    <div>
      <Exit id={props.id}/>
      <h1>Manage Items</h1>
      <Button variant="primary" onClick={handleAddModalShow} style={{margin: "0.5rem"}}>
        Add New Item
      </Button>
      <Table striped bordered hover>
        <thead>
          <tr>
            <th>#</th>
            <th>Name</th>
            <th>Price</th>
            <th>Category</th>
            <th>Quantity</th>
          </tr>
        </thead>
        <tbody>
          {itemsList.map((item) => (
            <tr key={item.id}>
              <td>{item.id}</td>
              <td>{item.name}</td>
              <td>{item.price}</td>
              <td>{item.category}</td>
              <td>{item.quantity}</td>
              <td>
                <Button
                  variant="primary"
                  onClick={() => handleEditModalShow(item)}
                >
                  Edit
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      <Modal show={showAddModal} onHide={handleAddModalClose}>
        <Modal.Header closeButton>
          <h5 className="modal-title">Add New Item</h5>
        </Modal.Header>
        <Modal.Body>
          <Form onSubmit={handleAddItem}>
            <Form.Group controlId="name">
              <Form.Label>Name</Form.Label>
              <Form.Control type="text" placeholder="Enter name" />
            </Form.Group>
            <Form.Group controlId="price">
              <Form.Label>Price</Form.Label>
              <Form.Control type="number" placeholder="Enter price" />
            </Form.Group>
            <Form.Group controlId="category">
              <Form.Label>Category</Form.Label>
              <Form.Control type="text" placeholder="Enter category" />
            </Form.Group>
            <Form.Group controlId="quantity">
              <Form.Label>Quantity</Form.Label>
              <Form.Control type="number" placeholder="Enter quantity" />
            </Form.Group>
            <Button variant="primary" type="submit" style={{margin: "0.5rem"}}>
              Add
            </Button>
          </Form>
        </Modal.Body>
      </Modal>
      <Modal show={showEditModal} onHide={handleEditModalClose}>
        <Modal.Header closeButton>
          <Modal.Title>Edit Item</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form onSubmit={handleEditItem}>
            <Form.Group controlId="name">
              <Form.Label>Name</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter name"
                defaultValue={editedItem ? editedItem.name : ''}
              />
            </Form.Group>
            <Form.Group controlId="price">
              <Form.Label>Price</Form.Label>
              <Form.Control
                type="number"
                placeholder="Enter price"
                defaultValue={editedItem ? editedItem.price : ''}
              />
            </Form.Group>
            <Form.Group controlId="category">
              <Form.Label>Category</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter category"
                defaultValue={editedItem ? editedItem.category : ''}
              />
            </Form.Group>
            <Form.Group controlId="quantity">
              <Form.Label>Quantity</Form.Label>
              <Form.Control
                type="number"
                placeholder="Enter quantity"
                defaultValue={editedItem ? editedItem.quantity : ''}
              />
            </Form.Group>
            <Button variant="primary" type="submit" style={{margin: "0.5rem"}}>
              Save Changes
            </Button>
          </Form>
        </Modal.Body>
      </Modal>
    </div>
  );
};

export default ManageItemsPage;

