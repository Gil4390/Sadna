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
        ResponseT<List<Item>> GetItemsByKeysWord(Guid userID, string keyWords, int minPrice, int maxPrice, int ratingItem, string category, int ratingStore);
        Response AddItemToCart(Guid userID, Guid storeID, Guid itemID, int itemAmount); //2.3
        //2.4
        Response RemoveItemFromCart(Guid userID, Guid storeID, Guid itemID);
        Response EditItemFromCart(Guid userID, Guid storeID, Guid itemID,  int itemAmount);
        ResponseT<ShoppingCart> GetDetailsOnCart(Guid userID);
        ResponseT <List<ItemForOrder>> PurchaseCart(Guid id, SPaymentDetails paymentDetails, SSupplyDetails usersDetail); //2.5
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
         //4.2
        Response PlaceBid(Guid userID, Guid itemID, double price);
        Response ReactToBid(Guid userID, Guid itemID, string bidResponse); 
        Response CloseStore(Guid userID, Guid storeID); //4.9
        Response DeleteStore(Guid userID, Guid storeID); 
        Response ReopenStore(Guid userID, Guid storeID);
        ResponseT<List<Order>> GetStorePurchases(Guid userID, Guid storeID); //4.13                                                   
        ResponseT<Dictionary<Guid, List<Order>>> GetAllStorePurchases(Guid userID); //6.4

        ResponseT<double> GetStoreRevenue(Guid userID, Guid storeID, DateTime date);
        
        ResponseT<double> GetSystemRevenue(Guid userID, DateTime date);
        void CleanUp();
        ConcurrentDictionary<Guid, Store> GetStores();
        void SetIsSystemInitialize(bool isInitialize);
        ResponseT<Store> GetStore(Guid storeID);
        void SetTSOrders(IOrders orders);
        ResponseT<Item> GetItemByID(Guid storeID, Guid itemID);
        Response AddCondition(Guid userID , Guid store ,string entity, string entityName, string type, object value, DateTime dt=default, string entityRes = default,string entityResName=default,
            string typeRes = default, double valueRes = default , string op= default, int opCond= default);  
        Response RemoveCondition(Guid userID,Guid storeID ,int condID);
        ResponseT<SPolicy[]> GetAllConditions(Guid userID,Guid store);
        ResponseT<DiscountPolicy> CreateSimplePolicy<T>(Guid userID,Guid store, T level, int percent, DateTime startDate, DateTime endDate);
        ResponseT<DiscountPolicy> CreateComplexPolicy(Guid userID,Guid store, string op, int[] policys);
        Response AddPolicy(Guid userID,Guid store, int discountPolicy);
        Response RemovePolicy(Guid userID,Guid store, int discountPolicy , string type);
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
        void LoadStoresFromDB();
    }
}