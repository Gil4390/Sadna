
export interface ItemCart  {
    itemId: string, 
    name: string,
    category: string,
    price: number,
    priceDiscount: number,
    rating : number, 
    storeId: string,   
    inStock: boolean,  
    count: number,  
};

export interface Store  {
    storeId: string, 
    name: string,
    isOpen: boolean,  
};

export interface Item  {
    itemID: string, 
    name: string,
    category: string,
    price: number,
    rating : number,  
    quantity: number,  
};

export interface PurcahseCondition {
    condID: number, 
    entity: string
    entityID: string,
    entityName: string,
    type: string,
    value: number,
    op : string,  
    entityRes: string,  
    entityIDRes: string, 
    entityNameRes: string, 
    typeRes: string,  
    valueRes: number,  
    opCond: number,  
};


