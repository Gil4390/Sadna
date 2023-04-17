using SadnaExpress.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.Store
{
    public interface IStoreFacade
    {
        Guid OpenNewStore(string storeName);
        void CloseStore(Guid storeID);
        void DeleteStore(Guid storeID);
        void ReopenStore(Guid storeID);
        List<Order> GetStorePurchases(Guid storeID);
        Dictionary<Guid, List<Order>> GetAllStorePurchases();
        Guid AddItemToStore(Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity);
        void WriteItemReview(Guid userID, Guid storeID, Guid itemID, string reviewText);
        ConcurrentDictionary<Guid, List<string>> GetItemReviews(Guid storeID, Guid itemID);
        double PurchaseCart(Dictionary<Guid, Dictionary<Guid, int>> items, ref List<ItemForOrder> itemsForOrder);
        void RemoveItemFromStore(Guid storeID, Guid itemID);
        void EditItemName(Guid storeID, Guid itemID, string category);
        void EditItemCategory(Guid storeID, Guid itemID, string category);
        void EditItemPrice(Guid storeID, Guid itemID, int price);
        void EditItemQuantity(Guid storeID, Guid itemID, int price);
        List<Store> GetAllStoreInfo();
        List<Item> GetItemsByName(string itemName, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1);
        List<Item> GetItemsByCategory(string category, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, int ratingStore = -1);
        List<Item> GetItemsByKeysWord(string keyWords, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1);
        void AddItemToCart(Guid storeID, Guid itemID, int quantity);
        void AddItemToStores(Dictionary<Guid, Dictionary<Guid, int>> items);
        void CleanUp();
        ConcurrentDictionary<Guid, Store> GetStores();
        void SetIsSystemInitialize(bool isInitialize);
        Item GetItemByID(Guid storeID, Guid itemID); //for tests
    }
}