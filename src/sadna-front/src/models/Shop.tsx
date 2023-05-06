
export interface ItemCart  {
    itemId: string, 
    name: string,
    category: string,
    price: number,
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