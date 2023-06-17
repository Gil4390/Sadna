using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store.Policy;
using SadnaExpress.ServiceLayer.Obj;
using SadnaExpress.ServiceLayer.SModels;

namespace SadnaExpress.ServiceLayer
{
    public interface ITradingSystem
    {
        ResponseT<Guid> Enter(); //1.1
        Response Exit(Guid userID); //1.2
        Response Register(Guid userID, string email, string firstName, string lastName, string password); //1.3
        ResponseT<Guid> Login(Guid userID, string email, string password); //1.4
        ResponseT<List<Store>> GetAllStoreInfo(); //2.1
        ResponseT<List<SItem>> GetItemsForClient(Guid userID, string keyWords, int minPrice = 0,
            int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1);// 2.2
        Response AddItemToCart(Guid userID, Guid storeID, Guid itemID, int itemAmount); //2.3
        //2.4
        Response RemoveItemFromCart(Guid userID, Guid storeID, Guid itemID);
        Response EditItemFromCart(Guid userID, Guid storeID, Guid itemID,  int itemAmount);
        ResponseT<ShoppingCart> GetDetailsOnCart(Guid userID);
        ResponseT<List<ItemForOrder>> PurchaseCart(Guid userID, SPaymentDetails paymentDetails, SSupplyDetails usersDetail); //2.5
        ResponseT<Guid> Logout(Guid userID); //3.1
        ResponseT<Guid> OpenNewStore(Guid userID, string storeName); //3.2
        Response WriteItemReview(Guid userID, Guid itemID, string reviewText); //3.3
        ResponseT<List<SReview>> GetItemReviews(Guid itemID);
        ResponseT<Dictionary<Guid, List<Order>>> GetPurchasesInfoUser(Guid userID); //3.7 (not in this version)
        ResponseT<List<ItemForOrder>> GetPurchasesInfoUserOnlu(Guid userID); //3.7 (not in this version)

        //3.8 and 3.9 (not in this version)
        //4.1
        ResponseT<Guid> AddItemToStore(Guid userID, Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity);
        Response RemoveItemFromStore(Guid userID, Guid storeID, Guid itemID);
        Response EditItemCategory(Guid userID, Guid storeID,  Guid itemID, string category);
        Response EditItemPrice(Guid userID, Guid storeID,  Guid itemID, int price); 
        Response EditItemName(Guid userID, Guid storeID,  Guid itemID, string name); 
        Response EditItemQuantity(Guid userID, Guid storeID,  Guid itemID, int quantity);
        public Response EditItem(Guid userID, Guid storeID,  Guid itemID, string itemName, string itemCategory, double itemPrice,
            int quantity);
        //4.2 Discount policy
        ResponseT<List<SPolicy>> GetAllPolicy(Guid userID, Guid storeID);
        ResponseT<DiscountPolicy> CreateSimplePolicy<T>(Guid userID,Guid store, T level, int percent, DateTime startDate,
            DateTime endDate);
        ResponseT<DiscountPolicy> CreateComplexPolicy(Guid userID,Guid store, string op, params int[] policys);
        Response AddPolicy(Guid userID,Guid store, int discountPolicy);
        Response RemovePolicy(Guid userID,Guid store, int discountPolicy , string type);
        Response AppointStoreOwner(Guid userID, Guid storeID, string userEmail); //4.4
        Response AppointStoreManager(Guid userID, Guid storeID, string userEmail); //4.6
        Response RemovePermission(Guid userID, Guid storeID, string userEmail, string permission);//4.5 + remove of 4.7
        // 4.7
        Response AddStoreManagerPermissions(Guid userID, Guid storeID, string userEmail, string permission);
        Response RemoveStoreManager(Guid userID1, Guid storeID, Guid userID2); //4.8 (not in this version)
        Response CloseStore(Guid userID, Guid storeID); //4.9
        ResponseT<List<SMemberForStore>> GetEmployeeInfoInStore(Guid userID, Guid storeID);  //4.11
        ResponseT<List<ItemForOrder>> GetStorePurchases(Guid userID, Guid storeID);//4.13 
        Response RemoveUserMembership(Guid userID, string email); //6.2
        ResponseT<Dictionary<Guid, List<Order>>> GetAllStorePurchases(Guid userID);//6.4
        ResponseT<List<SMember>> GetMembers(Guid userID); //6.6
        void CleanUp();
        ResponseT<bool> InitializeTradingSystem(Guid userID);

        //helpers
        void SetPaymentService(IPaymentService paymentService);
        void SetSupplierService(ISupplierService supplierService);
        void SetIsSystemInitialize(bool isInitialize);
        ResponseT<User> GetUser(Guid userID);
        ResponseT<Member> GetMember(Guid userID);
        ResponseT<Member> GetMember(String email);

        ResponseT<Store> GetStore(Guid storeID);
        ResponseT<Store> GetStore(String name);

        void SetTSOrders(IOrders orders);
        ResponseT<Item> GetItemByID(Guid storeID, Guid itemID);

        ResponseT<List<Notification>> GetNotifications(Guid userID);
        bool IsSystemInitialize();

        ResponseT<SPolicy[]> GetAllConditions(Guid userID , Guid store);

        ResponseT<Condition> AddCondition(Guid userID , Guid store ,string entity, string entityName, string type, object value, DateTime dt=default, string entityRes = default,string entityResName=default,
            string typeRes = default, double valueRes = default , string op= default, int opCond= default);
        Response RemoveCondition(Guid userID , Guid storeID ,int condID);
        
        // this functions needs to notify to offline members their notifications.
        public void getNotificationsForOfflineMembers();
        
        ResponseT<List<SItem>> GetCartItems(Guid userID);
        ResponseT<ConcurrentDictionary<Guid, Store>> GetStores();

        ResponseT<List<Member>> GetStoreOwners();

        ResponseT<List<Member>> GetStoreOwnerOfStores(List<Guid> stores);
        ResponseT<List<Item>> GetItemsInStore(Guid userID,Guid storeId);
        ResponseT<bool> IsAdmin(Guid userID);
        ResponseT<Dictionary<Guid, SPermission>> GetMemberPermissions(Guid userID);
        ResponseT<SStore> GetStoreInfo(Guid userID, Guid storeId);
        Response MarkNotificationAsRead(Guid userID, Guid notificationID);
        Response CheckPurchaseConditions(Guid userID);
        ResponseT<string> GetMemberName(Guid userID);
        ResponseT<double> GetStoreRevenue(Guid userID, Guid storeID, DateTime date);
        ResponseT<double> GetSystemRevenue(Guid userID, DateTime date);
        ResponseT<SBid> PlaceBid(Guid userID, Guid itemID, double price);
        ResponseT<SBid[]> GetBidsInStore(Guid userID, Guid storeID);
        Response ReactToBid(Guid userID, Guid itemID, Guid bidID, string bidResponse);
        Response ReactToJobOffer(Guid userID, Guid storeID, Guid newEmpID, bool offerResponse);
        ResponseT<List<int>> GetSystemUserActivity(Guid userID, DateTime fromDate, DateTime toDate);
    }
}
