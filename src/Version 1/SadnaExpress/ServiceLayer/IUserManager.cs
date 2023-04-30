using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.Services;

namespace SadnaExpress.ServiceLayer
{
    public interface IUserManager
    {
        // get function
        IUserFacade GetUserFacade();
        //all users actions
        ResponseT<Guid> Enter(); //1.1
        Response Exit(Guid userID); //1.2
        Response Register(Guid userID, string email, string firstName, string lastName, string password); //1.3
        ResponseT<Guid> Login(Guid userID, string email, string password); //1.4
        ResponseT<Guid> Logout(Guid userID); //3.1
        Response AppointStoreOwner(Guid userID, Guid storeID, string userEmail); //4.4
        Response RemoveStoreOwner(Guid userID1, Guid storeID, string userEmail); //4.5 
        Response AppointStoreManager(Guid userID, Guid storeID, string userEmail); //4.6
        // 4.7
        Response AddStoreManagerPermissions(Guid userID, Guid storeID, string userEmail, string permission);
        Response RemoveStoreManagerPermissions(Guid userID, Guid storeID, string userEmail, string permission);
        ResponseT<List<PromotedMember>> GetEmployeeInfoInStore(Guid userID, Guid storeID); //4.11

        //system manager actions 
        ResponseT<bool> InitializeTradingSystem(Guid userID); //1.1
        void CleanUp();
       
        ConcurrentDictionary<Guid, User> GetCurrent_Users();
        ConcurrentDictionary<Guid, Member> GetMembers();
        ResponseT<ShoppingCart> ShowShoppingCart(Guid userID);
        ResponseT<Guid> SetSecurityQA(Guid userID,string q, string a);

        ResponseT<ShoppingCart> GetShoppingCartById(Guid userID);
        ResponseT<Guid> UpdateFirst(Guid userID, string newFirst);
        ResponseT<Guid> UpdateLast(Guid userID, string newLast);
        ResponseT<Guid> UpdatePassword(Guid userID,string newPassword);
        void SetPaymentService(IPaymentService paymentService);
        void SetSupplierService(ISupplierService supplierService);
        void SetIsSystemInitialize(bool isInitialize);
        ResponseT<User> GetUser(Guid userID);
        ResponseT<Member> GetMember(Guid userID);
        ResponseT<ShoppingCart> GetUserShoppingCart(Guid userID);
        ResponseT<List<Notification>> GetNotifications(Guid userId);
        ResponseT<List<Member>> getAllStoreOwners(ConcurrentDictionary<Guid, Store> stores);
        ResponseT<List<Member>> GetStoreOwnerOfStores(List<Guid> stores);

        bool IsSystemInitialize();
    }
}