import React, { useState } from 'react';
import { Container, Row, Col, Form, Button } from 'react-bootstrap';
import { StoreItem } from '../components/StoreItem';

function ShoppingPage(props) {

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

  const [allItems, setAllItems] = useState(items);
  //get request with all items


  const [searchQuery, setSearchQuery] = useState('');
  const [searchType, setSearchType] = useState('product');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [minStoreRating, setMinStoreRating] = useState('');
  const [minItemRating, setMinItemRating] = useState('');

  const handleSearchQueryChange = (event) => {
    setSearchQuery(event.target.value);
  };

  const handleSearchTypeChange = (event) => {
    setSearchType(event.target.value);
  };

  const handleMinPriceChange = (event) => {
    setMinPrice(event.target.value);
  };

  const handleMaxPriceChange = (event) => {
    setMaxPrice(event.target.value);
  };

  const handleMinStoreRatingChange = (event) => {
    setMinStoreRating(event.target.value);
  };

  const handleMinItemRatingChange = (event) => {
    setMinItemRating(event.target.value);
  };

  // handle search form submission
  const handleSearchSubmit = (event) => {
    event.preventDefault();
    console.log(`Search Query: ${searchQuery}, Search Type: ${searchType}`,`Min Price: ${minPrice}, Max Price: ${maxPrice}, Min Store Rating: ${minStoreRating}, Min Item Rating: ${minItemRating}`);
    // do something with search query and type
  };

  return (
    props.isInit?
    (<div>
      <Container>
        <Row className="mt-3">
          <Col>
            <Form inline onSubmit={handleSearchSubmit}>
              <Row>
                <Col sm={6}>
                  <Form.Control type="text" placeholder="Search" className="mr-sm-2" value={searchQuery} onChange={handleSearchQueryChange} />
                </Col>
                <Col sm={3}>
                  <Form.Control as="select" className="mr-sm-2" value={searchType} onChange={handleSearchTypeChange}>
                    <option value="product">Product</option>
                    <option value="category">Category</option>
                  </Form.Control>
                </Col>
                <Col>
                  <Button variant="outline-primary" type="submit">
                    Search
                  </Button>
                </Col>
              </Row>
              <Row style={{padding: "0.5rem"}}>
                <Col sm={2}>
                  <span className="fs-2">Min Price:</span>
                  <Form.Control type="number" placeholder="0" value={minPrice} onChange={handleMinPriceChange} />
                </Col>
                <Col sm={2}>
                  <span className="fs-2">Max Price:</span>
                  <Form.Control type="number" value={maxPrice} onChange={handleMaxPriceChange} />
                </Col>
                <Col sm={3}>
                  <span className="fs-2">Min Store Rating:</span>
                  <Form.Control as="select" value={minStoreRating} onChange={handleMinStoreRatingChange}>
                    <option value="1">1</option>
                    <option value="2">2</option>
                    <option value="3">3</option>
                    <option value="4">4</option>
                    <option value="5">5</option>
                  </Form.Control>
                </Col>
                <Col sm={3}>
                  <span className="fs-2">Min Item Rating:</span>
                  <Form.Control as="select" value={minItemRating} onChange={handleMinItemRatingChange}>
                    <option value="1">1</option>
                    <option value="2">2</option>
                    <option value="3">3</option>
                    <option value="4">4</option>
                    <option value="5">5</option>
                  </Form.Control>
                </Col>
              </Row>
            </Form>
          </Col>
        </Row>
        <Row className="mt-3">
          {allItems.map((item) => (
            <Col sm={8} md={5} lg={4} xl={3} key={item.id} className="mt-3">
              <StoreItem {...item} />
            </Col>
            ))}
        </Row>
      </Container>
      
    </div>):(<div><h1 class="font-weight-light">About</h1>
            <p>
              "system is not initialized"
            </p></div>)
  );
}

export default ShoppingPage;
