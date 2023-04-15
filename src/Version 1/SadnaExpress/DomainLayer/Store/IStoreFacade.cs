using SadnaExpress.Services;
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
        void WriteItemReview(Guid userID, Guid storeID, int itemID, string reviewText);

        void PurchaseItems(string storeName, List<string> itemsName);

        void RemoveItemFromStore(Guid storeID, int itemId);
        void EditItemCategory(string storeName, string itemName, string category);
        void EditItemPrice(string storeName, string itemName, int price);

        void GetAllStoreInfo(string storeName);

        List<Item> GetItemsByName(string itemName, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1);
        List<Item> GetItemsByCategory(string category, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, int ratingStore = -1);
        List<Item> GetItemsByKeysWord(string keyWords, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1);
        void CleanUp();
        ConcurrentDictionary<Guid, Store> GetStores();
        void SetSupplierService(ISupplierService supplierService);
        bool PlaceSupply(string orderDetails, string userDetails);





        Store GetStoreById(Guid storeId);
    }
}