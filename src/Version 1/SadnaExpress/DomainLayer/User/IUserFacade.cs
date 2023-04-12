using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.DomainLayer.User
{
    public interface IUserFacade
    {
        int Enter();
        void Exit(int id);
        void Register(int id, string email, string firstName, string lastLame, string password);
        int Login(int id, string email, string password);
        int Logout(int id);
        void AddItemToCart(int id,Guid storeID, int itemID,  int itemAmount);
        void RemoveItemFromCart(int id,Guid storeID, int itemID);
        void EditItemFromCart(int id,Guid storeID, int itemID,  int itemAmount);
        Dictionary<string,List<string>> getDetailsOnCart();
        void PurchaseCart(int id);
        void EditItemCart(int id,Guid storeID, string itemName);
        void OpenNewStore(int id,Guid storeID);
        void AddReview(int id,Guid storeID, string itemName);
        void AddItemInventory(int id,Guid storeID, string itemName);
        void RemoveItemInventory(int id,Guid storeID, string itemName);
        void EditItemInventory(int id,Guid storeID, string itemName);
        void AppointStoreOwner(int id,Guid storeID, string email);
        void AppointStoreManager(int id, Guid storeID, string email);
        void AddStoreManagerPermissions(int id,Guid storeID, string email, string Permission);
        void RemoveStoreManagerPermissions(int id,Guid storeID, string email, string Permission);
        void CloseStore(int id,Guid storeID);
        void GetDetailsOnStore(int id,Guid storeID);
        List<PromotedMember> GetEmployeeInfoInStore(int id, Guid storeID);
        void UpdateFirst(int id, string newFirst);
        void UpdateLast(int id, string newLast);
        void UpdatePassword(int id, string newPassword);
        bool InitializeTradingSystem(int id);
        void CleanUp();
        ConcurrentDictionary<int, User> GetCurrent_Users();
        ConcurrentDictionary<int, Member> GetMembers();
        bool hasPermissions(int userId, Guid storeID, List<string> per);
        ShoppingCart ShowShoppingCart(int id);
        void SetSecurityQA(int id,string q, string a);

        ShoppingCart GetShoppingCartById(int id);

        bool isLogin(int idx);

    }
}