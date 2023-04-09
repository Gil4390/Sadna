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
        //all users actions
        ResponseT<int> Enter(); //1.1
        Response Exit(int id); //1.2
        Response Register(int id, string email, string firstName, string lastName, string password); //1.3
        ResponseT<int> Login(int id, string email, string password); //1.4
        Response AddItemToCart(int id, int itemID, int itemAmount); //2.3
        //2.4
        Response RemoveItemFromCart(int id, int itemID,  int itemAmount);
        Response EditItemFromCart(int id, int itemID,  int itemAmount);
        ResponseT<Dictionary<string,List<string>>> getDetailsOnCart();
        ResponseT<int> Logout(int id); //3.1
        Response AppointStoreOwner(int id, Guid storeID, string userEmail); //4.4
        Response AppointStoreManager(int id, Guid storeID, string userEmail); //4.6
        // 4.7
        Response AddStoreManagerPermissions(int id, Guid storeID, string userEmail, string permission);
        Response RemoveStoreManagerPermissions(int id, Guid storeID, string userEmail, string permission);
        ResponseT<List<S_Member>> GetEmployeeInfoInStore(int id, Guid storeID); //4.11
        
        
        /// add function for get user purchases - dina/noga
        void CleanUp();
        bool InitializeTradingSystem(int id);
        ConcurrentDictionary<int, User> GetCurrent_Users();
        ConcurrentDictionary<int, Member> GetMembers();
        ResponseT<ShoppingCart> ShowShoppingCart(int id);
    }
}