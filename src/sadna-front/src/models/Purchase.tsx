export interface UserPurchase { 
  userId: string,
  items: ItemUserPurchase[],
};
export interface ItemUserPurchase {
  ItemId: string,
  storeId: string,
  price: number,
  quantity: number,
};


export interface StorePurchase {
  storeId: string,
  items: ItemStorePurchase[],
};
export interface ItemStorePurchase {
  ItemId: string,
  userId: string,
  price: number,
  quantity: number,
};