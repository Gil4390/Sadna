using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public interface IStoreFacade
    {
        Guid OpenNewStore(string storeName);
        void CloseStore(Guid storeId);
        void DeleteStore(Guid storeId);
        void ReopenStore(Guid storeId);
        LinkedList<Order> GetStorePurchases(Guid storeId);

        void AddItemToStore(Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity);
        void WriteReview(string storeName, string itemName, int rating);

        void PurchaseItems(string storeName, List<string> itemsName);

        void RemoveItemFromStore(Guid storeID, int itemId);
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
        Store GetStoreById(Guid storeId);
    }
}