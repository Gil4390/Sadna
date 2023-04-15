using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
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
        void AddItemToCart(Guid userID,Guid storeID, int itemID,  int itemAmount);
        void RemoveItemFromCart(Guid userID,Guid storeID, int itemID);
        void EditItemFromCart(Guid userID,Guid storeID, int itemID,  int itemAmount);
        Dictionary<string,List<string>> getDetailsOnCart();
        void PurchaseCart(Guid userID);
        void EditItemCart(Guid userID,Guid storeID, string itemName);
        void OpenNewStore(Guid userID,Guid storeID);
        void AddReview(Guid userID,Guid storeID, string itemName);
        void AddItemInventory(Guid userID,Guid storeID, string itemName);
        void RemoveItemInventory(Guid userID,Guid storeID, string itemName);
        void EditItemInventory(Guid userID,Guid storeID, string itemName);
        void AppointStoreOwner(Guid userID,Guid storeID, string email);
        void AppointStoreManager(Guid userID, Guid storeID, string email);
        void AddStoreManagerPermissions(Guid userID,Guid storeID, string email, string Permission);
        void RemoveStoreManagerPermissions(Guid userID,Guid storeID, string email, string Permission);
        void CloseStore(Guid userID,Guid storeID);
        void GetDetailsOnStore(Guid userID,Guid storeID);
        List<PromotedMember> GetEmployeeInfoInStore(Guid userID, Guid storeID);
        void UpdateFirst(Guid userID, string newFirst);
        void UpdateLast(Guid userID, string newLast);
        void UpdatePassword(Guid userID, string newPassword);
        bool InitializeTradingSystem(Guid userID);
        void CleanUp();
        ConcurrentDictionary<Guid, User> GetCurrent_Users();
        ConcurrentDictionary<Guid, Member> GetMembers();
        bool hasPermissions(Guid userId, Guid storeID, List<string> per);
        ShoppingCart ShowShoppingCart(Guid userID);
        void SetSecurityQA(Guid userID,string q, string a);
        void SetPaymentService(IPaymentService paymentService);
        bool PlacePayment(string transactionDetails);
        void SetSupplierService(ISupplierService supplierService);
        bool PlaceSupply(string orderDetails, string userDetails);
        ShoppingCart GetShoppingCartById(Guid userID);
        bool isLoggedIn(Guid userID);

    }
}