using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store.DiscountPolicy;

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
        List<Review> GetItemReviews(Guid storeID, Guid itemID);
        double PurchaseCart(Dictionary<Guid, Dictionary<Guid, int>> items, ref List<ItemForOrder> itemsForOrder, string email);
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
        Store GetStore(Guid storeID);
        Item GetItemByID(Guid storeID, Guid itemID); //for tests
        void SetTSOrders(IOrders orders);
        Condition GetCondition<T, M>(Guid store ,T entity, string type, double value, DateTime dt=default, M entityRes=default, string typeRes=default, double valueRes=default);
        Condition AddCondition<T, M>(Guid store ,T entity, string type, double value,DateTime dt=default,  M entityRes=default, string typeRes=default, double valueRes=default);
        void RemoveCondition(Guid store ,Condition cond);
        PurchaseCondition[]  GetAllConditions(Guid store);
        Condition AddDiscountCondition<T>(Guid store, T entity, string type, double value);
        DiscountPolicy.DiscountPolicy CreateSimplePolicy<T>(Guid store, T level, int percent, DateTime startDate, DateTime endDate);
        DiscountPolicy.DiscountPolicy CreateComplexPolicy(Guid store, string op, object[] policys);
        DiscountPolicyTree AddPolicy(Guid store, DiscountPolicy.DiscountPolicy discountPolicy); 
        void RemovePolicy(Guid store, DiscountPolicy.DiscountPolicy discountPolicy);

        void LoadData(Store store1, Store store2);
        Guid GetItemStoreId(Guid Itemid);
        int GetItemByQuantity(Guid storeid, Guid itemid);
        void EditItem(Guid userId, Guid storeId, Guid itemId, string itemName, string itemCategory, double itemPrice, int quantity);
        List<Item> GetItemsInStore(Guid storeId);
        double GetItemAfterDiscount(Guid storeId, Item item);
        Dictionary<Guid, Dictionary<Item, double>> GetCartItems(Dictionary<Guid, Dictionary<Guid, int>> cart);
        Store GetStoreInfo(Guid storeId);

    }
}