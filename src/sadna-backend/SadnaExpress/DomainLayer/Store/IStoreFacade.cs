using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.DataLayer;

namespace SadnaExpress.DomainLayer.Store
{
    public interface IStoreFacade
    {
        Guid OpenNewStore(string storeName);
        void CloseStore(Guid storeID);
        List<Order> GetStorePurchases(Guid storeID);
        Dictionary<Guid, List<Order>> GetAllStorePurchases();
        Guid AddItemToStore(Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity);
        void WriteItemReview(Guid userID, Guid itemID, string reviewText);
        List<Review> GetItemReviews(Guid itemID);
        double PurchaseCart(DatabaseContext db,Dictionary<Guid, Dictionary<Guid, int>> items, ref List<ItemForOrder> itemsForOrder, string email);
        void RemoveItemFromStore(Guid storeID, Guid itemID);
        List<Store> GetAllStoreInfo();
        List<Item> GetItemsByKeysWord(string keyWords, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1);
        void AddItemToCart(Guid storeID, Guid itemID, int quantity);
        void AddItemToStores(DatabaseContext db, Dictionary<Guid, Dictionary<Guid, int>> items);
       
        Condition AddCondition(Guid store ,string entity, string entityName, string type, object value, DateTime dt=default, string op= default, int opCond= default);
        void RemoveCondition(Guid storeID ,int condID);
        List<Condition> GetAllConditions(Guid store);
        DiscountPolicy CreateSimplePolicy<T>(Guid store, T level, int percent, DateTime startDate, DateTime endDate);
        DiscountPolicy CreateComplexPolicy(Guid store, string op, int[] policys);
        DiscountPolicyTree AddPolicy(Guid store, int discountPolicy); 
        void RemovePolicy(Guid store, int discountPolicy , string type);
        (Dictionary<DiscountPolicy, bool>, Dictionary<Condition, bool>) GetAllPolicy(Guid storeID);
        Guid GetItemStoreId(Guid Itemid);
        int GetItemByQuantity(Guid storeid, Guid itemid);
        void EditItem(Guid storeId, Guid itemId, string itemName, string itemCategory, double itemPrice, int quantity);
        List<Item> GetItemsInStore(Guid storeId);
        double GetItemAfterDiscount(Guid storeId, Item item);
        Dictionary<Guid, Dictionary<Item, double>> GetCartItems(Dictionary<Guid, Dictionary<Guid, int>> cart);
        Store GetStoreInfo(Guid storeId);
        void CheckPurchaseConditions(Dictionary<Guid, Dictionary<Guid, int>> value);

        void LoadStoresFromDB();
        Item GetItemByID(Guid storeID, Guid itemID); //for tests
        void SetTSOrders(IOrders orders);
        void SetIsSystemInitialize(bool isInitialize);
        Store GetStore(Guid storeID);
        Store GetStore(String name);
        void CleanUp();
    }
}