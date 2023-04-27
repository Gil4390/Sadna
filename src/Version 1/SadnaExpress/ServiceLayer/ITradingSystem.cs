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

namespace SadnaExpress.ServiceLayer
{
    public interface ITradingSystem
    {
        ResponseT<Guid> Enter(); //1.1
        Response Exit(Guid userID); //1.2
        Response Register(Guid userID, string email, string firstName, string lastName, string password); //1.3
        ResponseT<Guid> Login(Guid userID, string email, string password); //1.4
        ResponseT<List<Store>> GetAllStoreInfo(); //2.1
        // 2.2
        ResponseT<List<Item>> GetItemsByName(Guid userID, string itemName, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1);
        ResponseT<List<Item>> GetItemsByCategory(Guid userID, string category, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, int ratingStore = -1);
        ResponseT<List<Item>> GetItemsByKeysWord(Guid userID, string keyWords, int minPrice = 0, int maxPrice = Int32.MaxValue, int ratingItem = -1, string category = null, int ratingStore = -1);
        Response AddItemToCart(Guid userID, Guid storeID, Guid itemID, int itemAmount); //2.3
        //2.4
        Response RemoveItemFromCart(Guid userID, Guid storeID, Guid itemID);
        Response EditItemFromCart(Guid userID, Guid storeID, Guid itemID,  int itemAmount);
        ResponseT<ShoppingCart> GetDetailsOnCart(Guid userID);
        Response PurchaseCart(Guid userID, string paymentDetails, string usersDetail); //2.5
        ResponseT<Guid> Logout(Guid userID); //3.1
        ResponseT<Guid> OpenNewStore(Guid userID, string storeName); //3.2
        Response WriteItemReview(Guid userID, Guid storeID, Guid itemID, string reviewText); //3.3
        ResponseT<ConcurrentDictionary<Guid, List<string>>> GetItemReviews(Guid storeID, Guid itemID);
        Response RateItem(Guid userID, int itemID, int score); //3.4 (not in this version)
        Response WriteMessageToStore(Guid userID, Guid storeID, string message); //3.5  (not in this version)
        Response ComplainToAdmin(Guid userID, string message); //3.6 (not in this version)
        Response GetPurchasesInfoUser(Guid userID); //3.7 (not in this version)
        //3.8 and 3.9 (not in this version)
        //4.1
        ResponseT<Guid> AddItemToStore(Guid userID, Guid storeID, string itemName, string itemCategory, double itemPrice, int quantity);
        Response RemoveItemFromStore(Guid userID, Guid storeID, Guid itemID);
        Response EditItemCategory(Guid userID, Guid storeID,  Guid itemID, string category);
        Response EditItemPrice(Guid userID, Guid storeID,  Guid itemID, int price); 
        Response EditItemName(Guid userID, Guid storeID,  Guid itemID, string name); 
        Response EditItemQuantity(Guid userID, Guid storeID,  Guid itemID, int quantity); 
        Response AppointStoreOwner(Guid userID, Guid storeID, string userEmail); //4.4
        Response RemoveStoreOwner(Guid userID1, Guid storeID, Guid userID2); //4.5 (not in this version)
        Response AppointStoreManager(Guid userID, Guid storeID, string userEmail); //4.6
        // 4.7
        Response AddStoreManagerPermissions(Guid userID, Guid storeID, string userEmail, string permission);
        Response RemoveStoreManagerPermissions(Guid userID, Guid storeID, string userEmail, string permission);
        Response RemoveStoreManager(Guid userID1, Guid storeID, Guid userID2); //4.8 (not in this version)
        Response CloseStore(Guid userID, Guid storeID); //4.9
        Response ReopenStore(Guid userID, Guid storeID); //4.10 (not in this version)
        ResponseT<List<PromotedMember>> GetEmployeeInfoInStore(Guid userID, Guid storeID);  //4.11
        ResponseT<List<Order>> GetStorePurchases(Guid userID, Guid storeID);//4.13 
        ResponseT<Dictionary<Guid, List<Order>>> GetAllStorePurchases(Guid userID);//6.4
        
        void CleanUp();
        ResponseT<bool> InitializeTradingSystem(Guid userID);
        Response DeleteStore(Guid userID, Guid storeID);
        Response DeleteMember(Guid userID1, Guid userID2);
        ResponseT<Guid> UpdateFirst(Guid userID, string newFirst);
        ResponseT<Guid> UpdateLast(Guid userID, string newLast);
        ResponseT<Guid> UpdatePassword(Guid userID, string newPassword);
        ResponseT<Guid> SetSecurityQA(Guid userID,string q, string a);

        //helpers
        void SetPaymentService(IPaymentService paymentService);
        void SetSupplierService(ISupplierService supplierService);
        void SetIsSystemInitialize(bool isInitialize);
        ResponseT<User> GetUser(Guid userID);
        ResponseT<Member> GetMember(Guid userID);
        ResponseT<ShoppingCart> GetUserShoppingCart(Guid userID);
        ResponseT<Store> GetStore(Guid storeID);
        void SetTSOrders(IOrders orders);
        ResponseT<Item> GetItemByID(Guid storeID, Guid itemID);

        ResponseT<List<Notification>> GetNotifications(Guid userID);



    }
}
