using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.ServiceLayer.Obj;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpress.ServiceLayer.ServiceObjects
{
    public interface IStoreManager
    {
        ResponseT<List<Store>> GetAllStoreInfo(); //2.1
        // 2.2
        ResponseT<List<Item>> GetItemsByName(Guid userID, string itemName, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore);
        ResponseT<List<Item>> GetItemsByCategory(Guid userID, string category, int minPrice, int maxPrice, int ratingItem, int ratingStore);
        ResponseT<List<Item>> GetItemsByKeysWord(Guid userID, string keyWords, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore);
        Response AddItemToCart(Guid userID, Guid storeID, Guid itemID, int itemAmount); //2.3
        //2.4
        Response RemoveItemFromCart(Guid userID, Guid storeID, Guid itemID);
        Response EditItemFromCart(Guid userID, Guid storeID, Guid itemID,  int itemAmount);
        ResponseT<ShoppingCart> GetDetailsOnCart(Guid userID);
        ResponseT <List<ItemForOrder>> PurchaseCart(Guid id, string paymentDetails, string usersDetail); //2.5
        ResponseT<Guid> OpenNewStore(Guid id, string storeName); //3.2
        Response WriteItemReview(Guid id, Guid itemID, string review); //3.3
        ResponseT<List<SReview>> GetItemReviews(Guid itemID);
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
        ResponseT<List<Order>> GetStorePurchases(Guid userID, Guid storeID); //4.13                                                   
        ResponseT<Dictionary<Guid, List<Order>>> GetAllStorePurchases(Guid userID); //6.4
        void CleanUp();
        ConcurrentDictionary<Guid, Store> GetStores();
        void SetIsSystemInitialize(bool isInitialize);
        ResponseT<Store> GetStore(Guid storeID);
        void SetTSOrders(IOrders orders);
        ResponseT<Item> GetItemByID(Guid storeID, Guid itemID);
        Response AddCondition(Guid store ,string entity, string entityName, string type, double value, DateTime dt=default, string entityRes = default,string entityResName=default,
            string typeRes = default, double valueRes = default , string op= default, int opCond= default);  
        Response RemoveCondition(Guid storeID ,int condID);
        ResponseT<SPolicy[]> GetAllConditions(Guid store);
        ResponseT<DiscountPolicy> CreateSimplePolicy<T>(Guid store, T level, int percent, DateTime startDate, DateTime endDate);
        ResponseT<DiscountPolicy> CreateComplexPolicy(Guid store, string op, int[] policys);
        Response AddPolicy(Guid store, int discountPolicy);
        Response RemovePolicy(Guid store, int discountPolicy);
        ResponseT<List<SPolicy>> GetAllPolicy(Guid userID, Guid storeID);
        void LoadData();
        Guid GetItemStoreId(Guid Itemid);
        ResponseT<List<SItem>> GetCartItems(Guid userID);
        ResponseT<int> GetItemQuantityInCart(Guid userID, Guid storeID, Guid itemID);
        Response EditItem(Guid userId, Guid storeId, Guid itemId, string itemName, string itemCategory, double itemPrice, int quantity);
        ResponseT<List<Item>> GetItemsInStore(Guid userID, Guid storeId);
        ResponseT<SStore> GetStoreInfo(Guid userID,Guid storeId);
        double GetItemAfterDiscount(Guid itemStoreid, Item item);
        Response CheckPurchaseConditions(Guid userId);
    }
}