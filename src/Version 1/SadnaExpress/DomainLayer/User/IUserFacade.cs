using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.User
{
    public interface IUserFacade
    {
        void Enter(int id);
        void Exit(int id);
        void Register(int id, string email, string firstName, string lastLame, string password);
        void Login(int id, string email, string password);
        void Logout(int id);
        void AddItemToBag(int id,string storeName, string itemName);
        Dictionary<string,List<string>> getDetailsOnCart();
        void PurchaseCart(int id);
        void AddItemCart(int id,string storeName, string itemName);
        void RemoveCart(int id,string storeName, string itemName);
        void EditItemCart(int id,string storeName, string itemName);
        void OpenStore(int id,string storeName);
        void AddReview(int id,string storeName, string itemName);
        void AddItemInventory(int id,string storeName, string itemName);
        void RemoveItemInventory(int id,string storeName, string itemName);
        void EditItemInventory(int id,string storeName, string itemName);
        void AddOwner(int id,string storeName, string email);
        void AddManager(int id,string storeName, string email);
        void AddPermissionsToManager(int id,string storeName, string email, string Permission);
        void CloseStore(int id,string storeName);
        void GetDetailsOnStore(int id,string storeName);
    }
}