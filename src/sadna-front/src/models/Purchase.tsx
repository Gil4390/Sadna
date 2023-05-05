export interface Orders { 
  orders: Map<string, Order[]>
};

export interface Order { 
  order: ItemForOrder[],
};

export interface ItemForOrder {
  ItemId: string,
  storeId: string,
  name: string,
  category: string,
  price: number,
  rating: number,
  userEmail: string,
  storeName: string,
};
