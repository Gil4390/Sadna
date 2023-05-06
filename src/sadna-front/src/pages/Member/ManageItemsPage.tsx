import React, { useState,useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import { Table, Button, Modal, Form } from 'react-bootstrap';
import Exit from "../Exit.tsx";
import { Item } from '../../models/Shop.tsx';
import { handleAddItemStore, handleGetItems, handleEditItemStore, handleRemoveItemStore } from '../../actions/MemberActions.tsx';
import { ResponseT,Response } from '../../models/Response.tsx';


const ManageItemsPage = (props) => {
  const location = useLocation();
  const { userId, storeId } = location.state;
  //const [userId, setUserId]=useState<string>(location.state.userId);
  //const [storeId, setStoreId]=useState<string>(location.state.storeId);
  console.log("hereeee user id "+userId+" store id "+storeId);

  const [showAddModal, setShowAddModal] = useState(false);
  const [showEditModal, setShowEditModal] = useState(false);
  const [editedItem, setEditedItem] = useState<Item>();
  const [editedItemCopy, setEditedItemCopy] = useState<Item>();

  const [itemsList, setItemsList] = useState<Item[]>([]);

  const [itemName, setItemName] = useState<string>('');
  const [itemCategory, setItemCategory] = useState<string>('');
  const [itemPrice, setItemPrice] = useState<number>(0);
  const [itemQuantity, setItemQuantity] = useState<number>(0);
  const [addItemResponse, setAddItemResponse]=useState<ResponseT>();
  const [editItemResponse, setEditItemResponse]=useState<Response>();
  const [removeItemResponse, setRemoveItemResponse]=useState<Response>();

  const setAllValues=()=>{
    setItemName('');
    setItemCategory('');
    setItemPrice(0);
    setItemQuantity(0);
  }
  
  const handleAddModalShow = () => setShowAddModal(true);
  const handleAddModalClose = () => {
    setAllValues();
    setShowAddModal(false);
  }

  const handleEditModalClose = () => {
    setShowEditModal(false)
  };

  const handleEditModalShow = (item) => {
    setEditedItem(item);
    setEditedItemCopy(item);
    setShowEditModal(true);
  };

  const getStoreItems =()=>{
    handleGetItems(userId,storeId).then(
      value => {
        setItemsList(value as Item[]);

      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    getStoreItems();
 }, [])

  const handleAddItem = (event) => {
    event.preventDefault();
    handleAddItemStore(userId,storeId, itemName, itemCategory, itemPrice, itemQuantity).then(
      value => {
        setAddItemResponse(value as ResponseT);
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    if(addItemResponse!=undefined)
      if(addItemResponse?.errorOccured)
        alert(addItemResponse?.errorMessage) 
      else{
        setShowAddModal(false);
        setAllValues();
        getStoreItems();
      }
      setAddItemResponse(undefined);
  }, [addItemResponse])
 
  const handleNameChange = (event) => {
    setItemName(event.target.value);
  }

  const handleCategoryChange = (event) => {
    setItemCategory(event.target.value);
  }

  const handlePriceChange = (event) => {
    setItemPrice(event.target.value);
  }

  const handleQuantityChange = (event) => {
    setItemQuantity(event.target.value);
  }

  const handleEditedItemNameChange = (event) => {
    if(editedItem!=undefined)
      editedItem.name=event.target.value;
  }

  const handleEditedItemCategoryChange = (event) => {
    if(editedItem!=undefined)
      editedItem.category=event.target.value;
  }

  const handleEditedItemPriceChange = (event) => {
    if(editedItem!=undefined)
      editedItem.price=event.target.value;
  }

  const handleEditedItemQuantityChange = (event) => {
    if(editedItem!=undefined)
      editedItem.quantity=event.target.value;
  }

  const handleEditItem = (event) => {
    event.preventDefault();
    handleEditItemStore(userId,storeId, editedItem?.itemID, editedItem?.name, editedItem?.category, editedItem?.price, editedItem?.quantity).then(
      value => {
        setEditItemResponse(value as Response);
      })
      .catch(error => {alert(error); setEditedItem(editedItemCopy);});
  };

  useEffect(() => {
    if(editItemResponse!=undefined)
      if(editItemResponse?.errorOccured){
        alert(editItemResponse?.errorMessage)
        setEditedItem(editedItemCopy);
      }
      else{
        setShowEditModal(false)
        getStoreItems();
      }
      setEditItemResponse(undefined);
  }, [editItemResponse])

  const handleRemoveItem = (item) => {
    handleRemoveItemStore(userId,storeId, item.itemID).then(
      value => {
        setRemoveItemResponse(value as Response);
      })
      .catch(error => alert(error));
  };

  useEffect(() => {
    if(removeItemResponse!=undefined)
      if(removeItemResponse?.errorOccured){
        alert(removeItemResponse?.errorMessage)
      }
      else{
        getStoreItems();
      }
      setRemoveItemResponse(undefined);
  }, [removeItemResponse])


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
            <th>Name</th>
            <th>Category</th>
            <th>Price</th>
            <th>Rating</th>
            <th>Quantity</th>
          </tr>
        </thead>
        <tbody>
          {itemsList.length==0?(<div>Store has no items</div>):(itemsList?.map((item) => (
            <tr key={item.itemID}>
              <td>{item.name}</td>
              <td>{item.category}</td>
              <td>{item.price}</td>
              <td>{item.rating}</td>
              <td>{item.quantity}</td>
              <td>
                <Button
                  variant="primary"
                  onClick={() => handleEditModalShow(item)}
                >
                  Edit
                </Button>
              </td>
              <td>
                <Button
                  variant="danger"
                  onClick={() => handleRemoveItem(item)}
                >
                  Remove
                </Button>
              </td>
            </tr>
          )))}
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
              <Form.Control type="text" placeholder="Enter name" value={itemName} onChange={handleNameChange}  />
            </Form.Group>
            <Form.Group controlId="category">
              <Form.Label>Category</Form.Label>
              <Form.Control type="text" placeholder="Enter category" value={itemCategory} onChange={handleCategoryChange} />
            </Form.Group>
            <Form.Group controlId="price">
              <Form.Label>Price</Form.Label>
              <Form.Control type="number" placeholder="Enter price" value={itemPrice} onChange={handlePriceChange}/>
            </Form.Group>   
            <Form.Group controlId="quantity">
              <Form.Label>Quantity</Form.Label>
              <Form.Control type="number" placeholder="Enter quantity" value={itemQuantity} onChange={handleQuantityChange}/>
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
                onChange={handleEditedItemNameChange}
              />
            </Form.Group>

            <Form.Group controlId="category">
              <Form.Label>Category</Form.Label>
              <Form.Control
                type="text"
                placeholder="Enter category"
                defaultValue={editedItem ? editedItem.category : ''}
                onChange={handleEditedItemCategoryChange}
              />
            </Form.Group>

            <Form.Group controlId="price">
              <Form.Label>Price</Form.Label>
              <Form.Control
                type="number"
                placeholder="Enter price"
                step={0.1}
                defaultValue={editedItem ? editedItem.price : ''}
                onChange={handleEditedItemPriceChange}
              />
            </Form.Group>
          
            <Form.Group controlId="quantity">
              <Form.Label>Quantity</Form.Label>
              <Form.Control
                type="number"
                placeholder="Enter quantity"
                defaultValue={editedItem ? editedItem.quantity : ''}
                onChange={handleEditedItemQuantityChange}
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

