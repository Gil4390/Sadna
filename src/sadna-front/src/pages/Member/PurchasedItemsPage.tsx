import React, { useState } from 'react';
import { Modal, Button } from 'react-bootstrap';
import Exit from "../Exit.tsx";

const purchases = [
  {
    name: 'Product 1',
    price: 10.99,
    category: 'Electronics',
    quantity: 2,
  },
  {
    name: 'Product 2',
    price: 15.99,
    category: 'Home',
    quantity: 1,
  },
  {
    name: 'Product 3',
    price: 8.99,
    category: 'Clothing',
    quantity: 3,
  },
];

const ReviewsModal = ({ show, handleClose, product }) => {
  const [review, setReview] = useState('');

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log(`Product: ${product.name}, Review: ${review}`);
    setReview('');
    handleClose();
  };

  return (
    <Modal show={show} onHide={handleClose}>
      <Modal.Header closeButton>
        <Modal.Title>Write a Review</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <p>{`Product: ${product.name}`}</p>
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

  const handleReviewClick = (product) => {
    setSelectedProduct(product);
    setShowReviewModal(true);
  };

  const handleCloseReviewModal = () => {
    setSelectedProduct(null);
    setShowReviewModal(false);
  };

  return (
    <div className="container">
      <Exit id={props.id}/>
      <h1>Past Purchases</h1>
      <table className="table" style={{background: "white"}}>
        <thead>
          <tr>
            <th>Name</th>
            <th>Price</th>
            <th>Category</th>
            <th>Quantity</th>
            <th>Review</th>
          </tr>
        </thead>
        <tbody>
          {purchases.map((product) => (
            <tr key={product.name}>
              <td>{product.name}</td>
              <td>{product.price}</td>
              <td>{product.category}</td>
              <td>{product.quantity}</td>
              <td>
                <Button onClick={() => handleReviewClick(product)}>
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
          product={selectedProduct}
        />
      )}
    </div>
  );
};

export default PurchasedItemsPage;
