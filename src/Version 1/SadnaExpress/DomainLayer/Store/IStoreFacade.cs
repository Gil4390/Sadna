using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public interface IStoreFacade
    {
        Guid OpenNewStore(string storeName);
        void CloseStore(int id , Guid storeId);
        void DeleteStore(int id , Guid storeId);
        void ReopenStore(int id , Guid storeId);
        LinkedList<Order> GetStorePurchases(Guid storeId);

        void AddItemToStore(string storeName, string itemName, string category, int price);
        void WriteReview(string storeName, string itemName, int rating);

        void PurchaseItems(string storeName, List<string> itemsName);

        void RemoveItemFromStore(string storeName, string itemName, string category, int price);
        void EditItemCategory(string storeName, string itemName, string category);
        void EditItemPrice(string storeName, string itemName, int price);

        void GetAllStoreInfo(string storeName);

        List<Item> GetItemsByName(string itemName);
        List<Item> GetItemsByCategory(string category);
        List<Item> GetItemsByKeysWord(string keyWords);
        List<Item> GetItemsByPrices(int minPrice, int maxPrice);
        List<Item> GetItemsByItemRating(int rating);
        List<Item> GetItemsByStoreRating(int rating);
        void CleanUp();
        ConcurrentDictionary<Guid, Store> GetStores();
        
        

    }
}