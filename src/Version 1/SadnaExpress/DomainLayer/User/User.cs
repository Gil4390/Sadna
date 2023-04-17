using System;
using System.Collections.Generic;
using System.Net;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.DomainLayer.User
{
    public class User
    {
        protected Guid userId;
        public Guid UserId {get => userId; set => userId = value;}
        protected ShoppingCart shoppingCart;
        public ShoppingCart ShoppingCart {get => shoppingCart; set => shoppingCart=value;}

        public User()
        {
            userId = Guid.NewGuid();
            shoppingCart = new ShoppingCart();
        }
        public void AddItemToCart(Guid storeID, Guid itemID, int itemAmount)
        {
            shoppingCart.AddItemToCart(storeID, itemID, itemAmount);
        }

        public void RemoveItemFromCart(Guid storeID, Guid itemID)
        {
            shoppingCart.RemoveItemFromCart(storeID, itemID);
        }
        
        public void EditItemFromCart(Guid storeID, Guid itemID, int itemAmount)
        {
            shoppingCart.EditItemFromCart(storeID, itemID, itemAmount);
        }
        
        public virtual bool hasPermissions(Guid storeID, List<string> listOfPermissions)
        {
            return false;
        }
        public virtual PromotedMember AppointStoreOwner(Guid storeID, Member newOwner)
        {
            throw new Exception("The user unauthorised to add new owner");
        }
        public virtual PromotedMember AppointStoreManager(Guid storeID, Member newManager)
        {
            throw new Exception("The user unauthorised to add new owner");
        }
        public virtual void AddStoreManagerPermissions(Guid storeID, Member manager, string permission)
        {
            throw new Exception("The user unauthorised to remove permissions");
        }
        public virtual void RemoveStoreManagerPermissions(Guid storeID, Member manager, string permission)
        {
            throw new Exception("The user unauthorised to remove permissions");
        }
        public virtual List<PromotedMember> GetEmployeeInfoInStore(Guid storeID)
        {
            throw new Exception("The user unauthorised to add new owner");
        }
        public virtual void CloseStore(Guid storeID)
        {
            throw new Exception("The user unauthorised to close");
        }
    }
}