export interface Bid {
  bidID: string,
  itemName: string,
  itemID: string,
  bidderEmail: string,
  offerPrice: number,
  approvers: string[],
  isActive: boolean
};