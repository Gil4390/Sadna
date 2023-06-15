using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;
using SadnaExpress.ServiceLayer.SModels;
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
        ResponseT<SBid[]> GetBidsInStore(Guid userID, Guid storeID); //4.2
        Dictionary<Guid, KeyValuePair<double, bool>> GetBidsOfUser(Guid userID);
        Response RemovePermission(Guid userID, Guid storeID, string userEmail, string permission); //4.5 + remove of 4.7
        Response AppointStoreOwner(Guid userID, Guid storeID, string userEmail); //4.4
        Response ReactToJobOffer(Guid userID, Guid storeID, Guid newEmpID, bool offerResponse);
        Response AppointStoreManager(Guid userID, Guid storeID, string userEmail); //4.6
        // 4.7
        Response AddStoreManagerPermissions(Guid userID, Guid storeID, string userEmail, string permission);
        ResponseT<List<SMemberForStore>> GetEmployeeInfoInStore(Guid userID, Guid storeID); //4.11
        Response RemoveUserMembership(Guid userID, string email);
        //system manager actions 
        
        ResponseT<bool> InitializeTradingSystem(Guid userID); //1.1
        void CleanUp();
       
        ConcurrentDictionary<Guid, User> GetCurrent_Users();
        ConcurrentDictionary<Guid, Member> GetMembers(Guid userID);
        void SetPaymentService(IPaymentService paymentService);
        void SetSupplierService(ISupplierService supplierService);
        void SetIsSystemInitialize(bool isInitialize);
        ResponseT<User> GetUser(Guid userID);
        ResponseT<Member> GetMember(Guid userID);
        ResponseT<Member> GetMember(String email);

        ResponseT<List<Notification>> GetNotifications(Guid userId);
        ResponseT<List<Member>> getAllStoreOwners(ConcurrentDictionary<Guid, Store> stores);
        ResponseT<List<Member>> GetStoreOwnerOfStores(List<Guid> stores);

        bool IsSystemInitialize();
        ResponseT<bool> isAdmin(Guid userID);

        ResponseT<Dictionary<Guid, SPermission>> GetMemberPermissions(Guid userID);
        Response MarkNotificationAsRead(Guid userID, Guid notificationID);
        Response Handshake();
        void CreateSystemManager(Guid userID);

        ResponseT<List<int>> GetSystemUserActivity(Guid userID, DateTime fromDate, DateTime toDate);
    }
}