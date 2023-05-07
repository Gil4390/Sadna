import React, { useEffect, useState } from 'react';
import { Modal, Button } from 'react-bootstrap';
import Exit from "../Exit.tsx";
import { Order, ItemForOrder } from '../../models/Purchase.tsx';
import { handleGetPurchasesOfUser, handleWriteItemReview } from '../../actions/MemberActions.tsx';


interface OrderTableRow {
  itemName: string;
  storeName: string;
  itemPrice: number;
  category: string;
  itemID: string;
}

const ReviewsModal = ({ show, handleClose, userId, product }) => {
  const [review, setReview] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log(`Product: ${product.name}, Review: ${review}`);
    if (review != ""){
      handleWriteItemReview(userId, product.itemID, review);
    }
    else{
      alert("can submit empty review");
    }
    setReview('');
    handleClose();
  };

  return (
    <Modal show={show} onHide={handleClose}>
      <Modal.Header closeButton>
        <Modal.Title>Write a Review</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p>{`Product: ${product.itemName}`}</p>
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="review">Review</label>
            <textarea
              className="form-control"
              id="review"
              value={review}
              onChange={(e) => setReview(e.target.value)}
              required
            />
          </div>
          <Button variant="primary" type="submit">
            Submit
          </Button>
        </form>
      </Modal.Body>
    </Modal>
  );
};

const PurchasedItemsPage = (props) => {
  const [showReviewModal, setShowReviewModal] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState(null);

  const [purchases, setPurchases] = useState<ItemForOrder[]>([]);

  const getUserPurchases = ()=>{
    handleGetPurchasesOfUser(props.id).then(
      value => {   
        setPurchases(value as ItemForOrder[]);
      })
      .catch(error => alert(error));
  }

  const handleReviewClick = (product) => {
    setSelectedProduct(product);
    setShowReviewModal(true);
  };

  const handleCloseReviewModal = () => {
    setSelectedProduct(null);
    setShowReviewModal(false);
  };

  useEffect(() => {
    getUserPurchases();
  }, []);




  const getOrderTableRows = (): OrderTableRow[] => {
    const orderTableRows: OrderTableRow[] = [];
      for (const item of purchases) {
          orderTableRows.push({
            storeName: item.storeName,
            itemName: item.name,
            itemPrice: item.price,
            category: item.category,
            itemID: item.itemID,
          });
        }
    

  return orderTableRows;
  };

  return (
    <div className="container">
      <Exit id={props.id}/>
      <h1>Past Purchases</h1>
      <table className="table" style={{background: "white"}}>
        <thead>
          <tr>
            <th>Item Name</th>
            <th>Store Name</th>
            <th>Price</th>
            <th>Category</th>
            <th>Review</th>
          </tr>
        </thead>
        <tbody>
          {getOrderTableRows().map((row, index) => (
            <tr key={index}>
              <td>{row.itemName}</td>
              <td>{row.storeName}</td>
              <td>{row.itemPrice}$</td>
              <td>{row.category}</td>
              <td>
                <Button onClick={() => handleReviewClick(row)}>
                  Write a Review
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
      {selectedProduct && (
        <ReviewsModal
          show={showReviewModal}
          handleClose={handleCloseReviewModal}
          userId={props.id}
          product={selectedProduct}
        />
      )}
    </div>
  );
};

export default PurchasedItemsPage;
