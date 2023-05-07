export interface Orders { 
  [key: string]:Order[]
};

export interface Order { 
  listItems: ItemForOrder[],
};

export interface ItemForOrder {
  itemID: string,
  storeID: string,
  name: string,
  category: string,
  price: number,
  rating: number,
  userEmail: string,
  storeName: string,
};
