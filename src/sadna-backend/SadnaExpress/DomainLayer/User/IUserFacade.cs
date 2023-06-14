using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;
using SadnaExpress.ServiceLayer.SModels;
using SadnaExpress.Services;
using SadnaExpress.DataLayer;

namespace SadnaExpress.DomainLayer.User
{
    public interface IUserFacade
    {
        Guid Enter();
        void Exit(Guid userID);
        void Register(Guid userID, string email, string firstName, string lastLame, string password);
        Guid Login(Guid userID, string email, string password);
        Guid Logout(Guid userID, bool exited = false);
        void AddItemToCart(Guid userID,Guid storeID, Guid itemID,  int itemAmount);
        void RemoveItemFromCart(Guid userID,Guid storeID, Guid itemID);
        void EditItemFromCart(Guid userID,Guid storeID, Guid itemID,  int itemAmount);
        ShoppingCart GetDetailsOnCart(Guid userID);
        void OpenNewStore(Guid userID,Guid storeID);
        void AddItemToStore(Guid userID,Guid storeID);
        void RemoveItemFromStore(Guid userID,Guid storeID);
        void EditItem(Guid userID,Guid storeID);
        void RemovePermission(Guid userID, Guid storeID, string userEmail, string permission);
        void AppointStoreOwner(Guid userID,Guid storeID, string email);
        void AppointStoreManager(Guid userID, Guid storeID, string email);
        void AddStoreManagerPermissions(Guid userID,Guid storeID, string email, string Permission);
        void CloseStore(Guid userID,Guid storeID);
        List<PromotedMember> GetEmployeeInfoInStore(Guid userID, Guid storeID);
        bool InitializeTradingSystem(Guid userID);
        void CleanUp();
        ConcurrentDictionary<Guid, User> GetCurrent_Users();
        ConcurrentDictionary<Guid, Member> GetMembers(Guid userID);
        bool hasPermissions(Guid userId, Guid storeID, List<string> per);
        void RemoveUserMembership(Guid userID, string email);
        void SetPaymentService(IPaymentService paymentService);
        int PlacePayment(double amount, SPaymentDetails transactionDetails);
        void SetSupplierService(ISupplierService supplierService);
        int PlaceSupply(SSupplyDetails userDetails);
        bool isLoggedIn(Guid userID);
        void SetIsSystemInitialize(bool isInitialize);
        User GetUser(Guid userID);
        Member GetMember(Guid userID);
        Member GetMember(String email);

        void GetStorePurchases(Guid userId, Guid storeId);
        void GetAllStorePurchases(Guid userId);
        void PurchaseCart(DatabaseContext db, Guid userID);
        bool CancelPayment(double amount, int transaction_id);
        List<Notification> GetNotifications(Guid userId);
        List<Member> getAllStoreOwners(ConcurrentDictionary<Guid, Store.Store> stores);
        List<Member> GetStoreOwnerOfStores(List<Guid> stores);
        Bid PlaceBid(Guid userID, Guid storeID, Guid itemID, string itemName, double price);
        Dictionary<Guid, KeyValuePair<double, bool>> GetBidsOfUser(Guid userID);
        List<Bid> GetBidsInStore(Guid userID, Guid storeID);
        void ReactToBid(Guid userID, Guid storeID, Guid bid, string bidResponse);
        bool IsSystemInitialize();
        int GetItemQuantityInCart(Guid userID, Guid storeID, Guid itemID);
        public bool IsUserAdmin(Guid userID);
        string GetUserEmail(Guid userID);

        ConcurrentDictionary<Guid, List<String>> GetMemberPermissions(Guid userID);
        void MarkNotificationAsRead(Guid userID, Guid notificationID);
        string Handshake();
        void NotifyBuyerPurchase(Guid userID, DatabaseContext db);
        
        void CreateSystemManager(Guid userID);
        void CheckIsValidMemberOperation(Guid userID);

        List<int> GetSystemUserActivity(Guid userID, DateTime fromDate, DateTime toDate);
    }
}