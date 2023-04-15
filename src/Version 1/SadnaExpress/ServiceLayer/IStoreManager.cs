using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.ServiceLayer.ServiceObjects
{
    public interface IStoreManager
    {
        ResponseT<List<Store>> GetAllStoreInfo(Guid userID); //2.1
        // 2.2
        ResponseT<List<Item>> GetItemsByName(Guid userID, string itemName, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore);
        ResponseT<List<Item>> GetItemsByCategory(Guid userID, string category, int minPrice, int maxPrice, int ratingItem, int ratingStore);
        ResponseT<List<Item>> GetItemsByKeysWord(Guid userID, string keyWords, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore);

        Response PurchaseCart(Guid id, string paymentDetails); //2.5
        ResponseT<Guid> OpenNewStore(Guid id, string storeName); //3.2
        Response WriteReview(Guid id, Guid itemID, string review); //3.3
        //4.1
        ResponseT<Guid> AddItemToStore(Guid userID, Guid storeID,  string itemName, string itemCategory, double itemPrice,
            int quantity);
        Response RemoveItemFromStore(Guid userID, Guid storeID, Guid itemID);
        Response EditItemCategory(Guid userID, Guid storeID, Guid itemID, string category);
        Response EditItemPrice(Guid userID, Guid storeID, Guid itemID, int price); 
        Response EditItemName(Guid userID, Guid storeID, Guid itemID, string name); 
        Response EditItemQuantity(Guid userID, Guid storeID, Guid itemID, int quantity); 
        Response CloseStore(Guid userID, Guid storeID); //4.9
        Response DeleteStore(Guid userID, Guid storeID); 
        Response ReopenStore(Guid userID, Guid storeID);

        Response GetPurchasesInfo(Guid userID, int storeID);//4.13 //6.4
        void CleanUp();
        ConcurrentDictionary<Guid, Store> GetStores();
        void SetIsSystemInitialize(bool isInitialize);
    }
}