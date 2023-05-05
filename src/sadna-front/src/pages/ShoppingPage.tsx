import React, { useEffect, useState } from 'react';
import { Container, Row, Col, Form, Button } from 'react-bootstrap';
import { StoreItem } from '../components/StoreItem.tsx';
import SystemNotInit from './SystemNotInit.tsx';
import { handleItemByKeyWord } from '../actions/GuestActions.tsx';
import {Item} from '../models/Shop.tsx';

function ShoppingPage(props) {
  const [allItems, setAllItems] = useState<Item[]>([]);

  const getAllItems =()=>{
    handleItemByKeyWord(props.id,"").then(
      value => {
       setAllItems(value as Item[]);
      })
      .catch(error => alert(error));
  }

  useEffect(() => {
    getAllItems();
 }, [])


  const [searchQuery, setSearchQuery] = useState('');
  const [searchType, setSearchType] = useState('product');
  const [minPrice, setMinPrice] = useState<number>();
  const [maxPrice, setMaxPrice] = useState<number>();
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
            <Form onSubmit={handleSearchSubmit}>
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
          {allItems.length===0? (<div>  No Items </div>): allItems.map((item) => (
            <Col sm={8} md={5} lg={4} xl={3} key={item.id} className="mt-3">
              <StoreItem {...item} />
            </Col>
            ))}
        </Row>
      </Container>
      
    </div>):(<SystemNotInit/>)
  );
}

export default ShoppingPage;
