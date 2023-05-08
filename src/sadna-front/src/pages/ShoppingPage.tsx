import React, { useEffect, useState } from 'react';
import { Container, Row, Col, Form, Button } from 'react-bootstrap';
import { StoreItem } from '../components/StoreItem.tsx';
import SystemNotInit from './SystemNotInit.tsx';
import { handleSearchItems } from '../actions/GuestActions.tsx';
import {Item} from '../models/Shop.tsx';
import { ResponseT } from '../models/Response.tsx';
import Exit from "./Exit.tsx";



function ShoppingPage(props) {
  const [allItems, setAllItems] = useState<Item[]>([]);
  const [response, setResponse] = useState<ResponseT>();

  const getAllItems =()=>{
    handleSearchItems(props.id).then(
      value => {
        setAllItems(value as Item[]);
      })
      .catch(error => alert(error));
  }


  useEffect(() => {
    if (props.isInit){
      getAllItems()
    } 
 }, [])


  const [keyWord, setkeyWord] = useState<string>('');
  const [category, setCategory] = useState<string>('');
  const [minPrice, setMinPrice] = useState<number>(0);
  const [maxPrice, setMaxPrice] = useState<number>(-1);
  const [minStoreRating, setMinStoreRating] = useState<number>(0);
  const [minItemRating, setMinItemRating] = useState<number>(0);

  const [message, setMessage] = useState<string>('');

  const handlekeyWordChange = (event) => {
    setkeyWord(event.target.value);
  };

  const handleCategoryChange = (event) => {
    setCategory(event.target.value);
  };

  const handleMinPriceChange = (event) => {
    setMinPrice(event.target.value);
  };

  const handleMaxPriceChange = (event) => {   
    if (event.target.value > 0) {
      setMaxPrice(event.target.value);
    }
    else
    {
      setMaxPrice(-1);
    }
  };

  const handleMinStoreRatingChange = (event) => {
    if (event.target.value >= 0) {
    setMinStoreRating(event.target.value);
    }
  };

  const handleMinItemRatingChange = (event) => {
    setMinItemRating(event.target.value);
  };


  const isValidItemNameOrCategory = (name) => {
    // item name or category contains only letters, numbers, undescore, and spaces and length between 0-20 characters.
    const itemNameRegex = /^[a-zA-Z0-9_ ]{0,20}$/;
    return itemNameRegex.test(name);
  }


  // handle search form submission
  const handleSearchSubmit = (event) => {
    event.preventDefault();
    //console.log(`keyWord: ${keyWord}, Caetgory: ${category}`,`Min Price: ${minPrice}, Max Price: ${maxPrice}, Min Store Rating: ${minStoreRating}, Min Item Rating: ${minItemRating}`);
    // do something with search query and type

    const validPrices = ((Number(minPrice) >= 0 && Number(minPrice) <= maxPrice) || maxPrice == -1);
    if(!isValidItemNameOrCategory(keyWord) || !isValidItemNameOrCategory(category) || !validPrices)
    {
        setMessage("Make Sure All Search Field Are Valid And Then Try Again!");
    }
    else
    {
      setMessage("Search Results");

      handleSearchItems(props.id,keyWord,minPrice ,maxPrice,minItemRating, category,minStoreRating).then(
        value => {
        setAllItems(value as Item[]);
        })
        .catch(error => alert(error));
    }
  };

  return (
    props.isInit?
    (<div>
      <Exit id={props.id}/>
      <Container>
        <Row className="mt-3">
          <Col>
            <Form>
              <Row>
                <Col sm={4}>
                <span className="fs-2">Item Name:</span>
                  <Form.Control type="text" placeholder="Search" className="mr-sm-2" value={keyWord} onChange={handlekeyWordChange} 
                  style={{borderColor: isValidItemNameOrCategory(keyWord) || keyWord.length === 0 ? '#28a745' : '#dc3534'}} />
                  { !isValidItemNameOrCategory(keyWord) && keyWord.length > 0 && <Form.Text className='text-danger'>item name contains only letters, numbers, undescore, and spaces and length between 0-20 characters.</Form.Text>}
                </Col>
                <Col sm={4}>
                <span className="fs-2">Category:</span>
                  <Form.Control type="text" placeholder="Search" className="mr-sm-2" value={category} onChange={handleCategoryChange}  
                  style={{borderColor: isValidItemNameOrCategory(category) || category.length === 0 ? '#28a745' : '#dc3534'}} />
                  { !isValidItemNameOrCategory(category) && category.length > 0 && <Form.Text className='text-danger'>Category contains only letters, numbers, undescore, and spaces and length between 0-20 characters.</Form.Text>}
                </Col>
                <Col>
                  <Button variant="outline-primary" type="submit" onClick={handleSearchSubmit}>
                    Search
                  </Button>
                </Col>
              </Row>
              <Row style={{padding: "0.5rem"}}>
                <Col sm={2}>
                  <span className="fs-2">Min Price:</span>
                  <Form.Control type="number" placeholder="0" value={minPrice} onChange={handleMinPriceChange}  
                  style={{borderColor: ((Number(minPrice) >= 0 && Number(minPrice) <= maxPrice) || maxPrice == -1) ? '#28a745' : '#dc3534'}} />
                  { !(( (Number(minPrice) >= 0) && (Number(minPrice) <= maxPrice )) || maxPrice == -1) && <Form.Text className='text-danger'>Min price should be non negative and less than max price.</Form.Text>}
                </Col>
                <Col sm={2}>
                  <span className="fs-2">Max Price:</span>
                  <Form.Control type="number" placeholder="-1" value={maxPrice} onChange={handleMaxPriceChange}  style={{borderColor: ((Number(minPrice) >= 0 && Number(minPrice) <= maxPrice) || maxPrice == -1) ? '#28a745' : '#dc3534'}} />
                  { !((Number(minPrice) >= 0 && Number(minPrice) <= maxPrice) || maxPrice == -1) && <Form.Text className='text-danger'>Min price should be non negative and less than max price.</Form.Text>}
                </Col>
                <Col sm={3}>
                  <span className="fs-2">Min Store Rating:</span>
                  <Form.Control as="select" value={minStoreRating} onChange={handleMinStoreRatingChange}>
                    <option value="0">0</option>
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
                    <option value="0">0</option>
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
        <div style={{fontWeight: "bold"}} className="text-center"> 
            {message}
          </div>
        <Row className="mt-3">
          {allItems.length===0? (<div>  No Items </div>): (allItems.map((item) => (
            // <div key={item.name}>{item.name} </div>
            <Col sm={8} md={5} lg={4} xl={3} key={item.itemID} className="mt-3">
              <StoreItem id={props.id} item={item} />
            </Col>
            )))}
        </Row>
      </Container>
      
    </div>):(<SystemNotInit/>)
  );
}

export default ShoppingPage;
