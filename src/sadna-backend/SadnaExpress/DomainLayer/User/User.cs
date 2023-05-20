using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Bid> bids;
        public List<Bid> Bids {get=>bids; set=>bids=value;}

        public User()
        {
            userId = Guid.NewGuid();
            shoppingCart = new ShoppingCart();
            bids = new List<Bid>();
        }
        public User(Member member)
        {
            userId = member.userId;
            shoppingCart = member.ShoppingCart;
            bids = member.Bids;
        }
        public void AddItemToCart(Guid storeID, Guid itemID, int itemAmount)
        {
            shoppingCart.AddItemToCart(storeID, itemID, itemAmount);
        }

        public Dictionary<Guid, KeyValuePair<double, bool>> GetBidsOfUser()
        {
            Dictionary<Guid, KeyValuePair<double, bool>> bidsDict = new Dictionary<Guid, KeyValuePair<double, bool>>();
            foreach (Bid bid in bids)
            {
                bidsDict.Add(bid.ItemID, new KeyValuePair<double, bool>(bid.Price, bid.Approved()));
            }
            return bidsDict;
        }
        
        public void PlaceBid(Guid storeID, Guid itemID, string itemName, double price, List<PromotedMember> employees)
        {
            bids.Add(new Bid(this, storeID ,itemID, itemName, price, employees));
        }

        public void RemoveBids()
        {
            foreach (Bid bid in bids)
                bid.CloseBid();
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
        public virtual Member RemoveStoreManagerPermissions(Guid storeID, Member manager, string permission)
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
        public virtual Tuple<List<Member>, List<Member>> RemoveStoreOwner(Guid storeID, Member storeOwner)
        {
            throw new Exception("The user unauthorised to remove store owner");
        }

        public virtual void ReactToBid(Guid storeID, string ItemName, string bidResponse)
        {
            throw new Exception("The user unauthorised to react to bid");
        }
        
        public virtual List<Bid> GetBidsInStore(Guid storeID)
        {
            throw new Exception("The user unauthorised to get bids");
        }
    }
}