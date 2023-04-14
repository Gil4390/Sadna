using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;
using SadnaExpress.DomainLayer.User;
using SadnaExpress.ServiceLayer.ServiceObjects;

namespace SadnaExpress.ServiceLayer
{
    public interface IUserManager
    {
        // get function
        IUserFacade GetUserFacade();
        //all users actions
        ResponseT<int> Enter(); //1.1
        Response Exit(int id); //1.2
        Response Register(int id, string email, string firstName, string lastName, string password); //1.3
        ResponseT<int> Login(int id, string email, string password); //1.4
        Response AddItemToCart(int id, Guid storeID, int itemID, int itemAmount); //2.3
        //2.4
        Response RemoveItemFromCart(int id, Guid storeID, int itemID);
        Response EditItemFromCart(int id, int itemID,  int itemAmount);
        ResponseT<Dictionary<string,List<string>>> getDetailsOnCart();
        ResponseT<int> Logout(int id); //3.1
        Response AppointStoreOwner(int id, Guid storeID, string userEmail); //4.4
        Response AppointStoreManager(int id, Guid storeID, string userEmail); //4.6
        // 4.7
        ResponseT<List<S_Order>> GetStorePurchases(int id, Guid storeID, string email);
        ResponseT<Dictionary<Guid, S_Order>> GetAllAStorePurchases(int id, Guid storeID,string email);

        Response AddStoreManagerPermissions(int id, Guid storeID, string userEmail, string permission);
        Response RemoveStoreManagerPermissions(int id, Guid storeID, string userEmail, string permission);
        ResponseT<List<S_Member>> GetEmployeeInfoInStore(int id, Guid storeID); //4.11

        //system manager actions 
        ResponseT<bool> InitializeTradingSystem(int id); //1.1

        /// add function for get user purchases - dina/noga
        void CleanUp();
       
        ConcurrentDictionary<int, User> GetCurrent_Users();
        ConcurrentDictionary<int, Member> GetMembers();
        ResponseT<ShoppingCart> ShowShoppingCart(int id);
        ResponseT<int> SetSecurityQA(int id,string q, string a);

        ResponseT<ShoppingCart> GetShoppingCartById(int id);

        bool isLogin(int idx);
        ResponseT<int> UpdateFirst(int id, string newFirst);
        ResponseT<int> UpdateLast(int id, string newLast);
        ResponseT<int> UpdatePassword(int id,string newPassword);

    }
}