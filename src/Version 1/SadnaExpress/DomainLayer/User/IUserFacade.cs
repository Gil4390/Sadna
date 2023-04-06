using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.User
{
    public interface IUserFacade
    {
        int Enter();
        void Exit(int id);
        void Register(int id, string email, string firstName, string lastLame, string password);
        int Login(int id, string email, string password);
        int Logout(int id);
        void AddItemToBag(int id,int storeID, string itemName);
        Dictionary<string,List<string>> getDetailsOnCart();
        void PurchaseCart(int id);
        void AddItemCart(int id,int storeID, string itemName);
        void RemoveCart(int id,int storeID, string itemName);
        void EditItemCart(int id,int storeID, string itemName);
        void OpenStore(int id,int storeID);
        void AddReview(int id,int storeID, string itemName);
        void AddItemInventory(int id,int storeID, string itemName);
        void RemoveItemInventory(int id,int storeID, string itemName);
        void EditItemInventory(int id,int storeID, string itemName);
        void AddOwner(int id,int storeID, string email);
        void AddManager(int id,int storeID, string email);
        void AddPermissionsToManager(int id,int storeID, string email, string Permission);
        void CloseStore(int id,int storeID);
        void GetDetailsOnStore(int id,int storeID);
        void CleanUp();
    }
}