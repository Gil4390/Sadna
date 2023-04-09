using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.ServiceLayer.ServiceObjects
{
    public interface IStoreManager
    {
        ResponseT<Guid> OpenNewStore(int id, string storeName); //3.2
        Response CloseStore(int id ,Guid storeID); //4.9
        Response DeleteStore(int id ,Guid storeID); 
        Response ReopenStore(int id ,Guid storeID);
        Response GetPurchasesInfo(int id ,Guid storeID);//4.13 //6.4

        
        Response AddItemToStore(int id, int storeID, string itemName, string itemCategory, float itemPrice);

        Response PurchaseItems(int id, string paymentDetails); //2.5

        Response WriteReview(int id, int itemID, string review); //3.3
        //4.1
 
        Response RemoveItemFromStore(int id, int itemID);
        Response EditItemCategory(string storeID, string itemName, string category);
        Response EditItemPrice(string storeID, string itemName, int price); 


     
                
        ResponseT<List<S_Store>> GetAllStoreInfo(int id); //2.1
        // 2.2
        ResponseT<List<S_Item>> GetItemsByName(int id, string itemName);
        ResponseT<List<S_Item>> GetItemsByCategory(int id, string category);
        ResponseT<List<S_Item>> GetItemsByKeysWord(int id, string keyWords);
        ResponseT<List<S_Item>> GetItemsByPrices(int id, int minPrice, int maxPrice);
        ResponseT<List<S_Item>> GetItemsByItemRating(int id, int rating);
        ResponseT<List<S_Item>> GetItemsByStoreRating(int id, int rating);
        void CleanUp();
        ConcurrentDictionary<Guid, Store> GetStores();
    }
}