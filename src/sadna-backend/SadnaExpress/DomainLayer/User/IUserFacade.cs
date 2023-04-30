using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.ServiceLayer;
using SadnaExpress.Services;

namespace SadnaExpress.DomainLayer.User
{
    public interface IUserFacade
    {
        Guid Enter();
        void Exit(Guid userID);
        void Register(Guid userID, string email, string firstName, string lastLame, string password);
        Guid Login(Guid userID, string email, string password);
        Guid Logout(Guid userID);
        void AddItemToCart(Guid userID,Guid storeID, Guid itemID,  int itemAmount);
        void RemoveItemFromCart(Guid userID,Guid storeID, Guid itemID);
        void EditItemFromCart(Guid userID,Guid storeID, Guid itemID,  int itemAmount);
        ShoppingCart GetDetailsOnCart(Guid userID);
        void OpenNewStore(Guid userID,Guid storeID);
        void AddItemToStore(Guid userID,Guid storeID);
        void RemoveItemFromStore(Guid userID,Guid storeID);
        void EditItem(Guid userID,Guid storeID);
        void AppointStoreOwner(Guid userID,Guid storeID, string email);
        void RemoveStoreOwner(Guid userID,Guid storeID, string email);
        void AppointStoreManager(Guid userID, Guid storeID, string email);
        void AddStoreManagerPermissions(Guid userID,Guid storeID, string email, string Permission);
        void RemoveStoreManagerPermissions(Guid userID,Guid storeID, string email, string Permission);
        void CloseStore(Guid userID,Guid storeID);
        List<PromotedMember> GetEmployeeInfoInStore(Guid userID, Guid storeID);
        void UpdateFirst(Guid userID, string newFirst);
        void UpdateLast(Guid userID, string newLast);
        void UpdatePassword(Guid userID, string newPassword);
        bool InitializeTradingSystem(Guid userID);
        void CleanUp();
        ConcurrentDictionary<Guid, User> GetCurrent_Users();
        ConcurrentDictionary<Guid, Member> GetMembers(Guid userID);
        bool hasPermissions(Guid userId, Guid storeID, List<string> per);
        ShoppingCart ShowShoppingCart(Guid userID);
        void SetSecurityQA(Guid userID,string q, string a);
        void SetPaymentService(IPaymentService paymentService);
        bool PlacePayment(double amount, string transactionDetails);
        void SetSupplierService(ISupplierService supplierService);
        bool PlaceSupply(string orderDetails, string userDetails);
        ShoppingCart GetShoppingCartById(Guid userID);
        bool isLoggedIn(Guid userID);
        void SetIsSystemInitialize(bool isInitialize);
        User GetUser(Guid userID);
        Member GetMember(Guid userID);
        ShoppingCart GetUserShoppingCart(Guid userID);
        void GetStorePurchases(Guid userId, Guid storeId);
        void GetAllStorePurchases(Guid userId);
        void PurchaseCart(Guid userID);
        bool CancelPayment(double amount, string transactionDetails);
        List<Notification> GetNotifications(Guid userId);
        List<Member> getAllStoreOwners(ConcurrentDictionary<Guid, Store.Store> stores);
        List<Member> GetStoreOwnerOfStores(List<Guid> stores);

        bool IsSystemInitialize();
    }
}