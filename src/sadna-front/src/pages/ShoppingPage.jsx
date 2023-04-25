import React, { useState } from 'react';
import { Container, Row, Col, Form, Button, Card } from 'react-bootstrap';
import { StoreItem } from '../components/StoreItem';

function ShoppingPage() {
  const [searchQuery, setSearchQuery] = useState('');
  const [searchType, setSearchType] = useState('product');

  const handleSearchQueryChange = (event) => {
    setSearchQuery(event.target.value);
  };

  const handleSearchTypeChange = (event) => {
    setSearchType(event.target.value);
  };

  // handle search form submission
  const handleSearchSubmit = (event) => {
    event.preventDefault();
    console.log(`Search Query: ${searchQuery}, Search Type: ${searchType}`);
    // do something with search query and type
  };

  const items = [
    { id: 1, name: 'Product 1', price: 10.99 },
    { id: 2, name: 'Product 2', price: 19.99 },
    { id: 3, name: 'Product 3', price: 5.99 },
    { id: 4, name: 'Product 4', price: 29.99 },
    { id: 5, name: 'Product 5', price: 9.99 },
    { id: 6, name: 'Product 6', price: 9.99 },
    { id: 7, name: 'Product 7', price: 9.99 },
    { id: 8, name: 'Product 8', price: 9.99 },
    { id: 9, name: 'Product 9', price: 9.99 },
  ];

  return (
    <Container>
      <Row className="mt-3">
        <Col>
          <Form inline onSubmit={handleSearchSubmit}>
            <Row>
              <Col sm={6}>
                <Form.Control type="text" placeholder="Search" className="mr-sm-2" value={searchQuery} onChange={handleSearchQueryChange} />
              </Col>
              <Col sm={2}>
                <Form.Control as="select" className="mr-sm-2" value={searchType} onChange={handleSearchTypeChange}>
                  <option value="product">Product</option>
                  <option value="category">Category</option>
                  <option value="brand">Brand</option>
                </Form.Control>
              </Col>
              <Col>
                <Button variant="outline-primary" type="submit">
                  Search
                </Button>
              </Col>
            </Row>
          </Form>
        </Col>
      </Row>
      <Row className="mt-3">
        {items.map((item) => (
          <Col sm={8} md={5} lg={4} xl={3} key={item.id} className="mt-3">
            <StoreItem {...item} />
          </Col>
          ))}
      </Row>
    </Container>
  );
}

export default ShoppingPage;
