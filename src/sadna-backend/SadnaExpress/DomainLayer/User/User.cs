using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Web.UI;
using Newtonsoft.Json;
using SadnaExpress.DataLayer;
using SadnaExpress.DomainLayer.Store;

namespace SadnaExpress.DomainLayer.User
{
    public class User
    {
        protected Guid userId;
        [Key]
        public Guid UserId {get => userId; set => userId = value;}
        protected ShoppingCart shoppingCart;
        
        public string Discriminator { get; set; }

        public ShoppingCart ShoppingCart {get => shoppingCart; set => shoppingCart=value;}
        
        private List<Bid> bids;
        
        [NotMapped]
        public List<Bid> Bids {get=>bids; set=>bids=value;}

        [NotMapped]
        public string BidsJson
        {
            get
            {
                List<Guid> bidsJ = new List<Guid>();
                foreach (Bid bid in Bids)
                    bidsJ.Add(bid.BidId);
                return JsonConvert.SerializeObject(bidsJ);
            }
            set
            {

            }
        }

        public string BidsDB { get; set; }

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
        
        public Bid PlaceBid(Guid storeID, Guid itemID, string itemName, double price)
        {
            Bid oldBid = null;
            foreach (Bid bid in bids)
            {
                if (itemID == bid.ItemID)
                {
                    oldBid = bid;
                    break;
                }
            }
            if (oldBid != null)
            {
                if (oldBid.Price <= price) 
                    throw new Exception("You already have better offer...");
                oldBid.CloseBid();
            }
            Bid newBid = new Bid(this, storeID, itemID, itemName, price);
            bids.Add(newBid);
            if (this.GetType() != typeof(User)) 
                DBHandler.Instance.UpdateBidAndUser(newBid, this);
            return newBid;
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

        public virtual PromotedMember ReactToJobOffer(Guid storeID, Member newOwner, bool offerResponse)
        {
            throw new Exception("The user unauthorised to react job offer");
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
        public virtual Tuple<List<Member>, List<Member>, HashSet<Guid>> RemoveStoreOwner(Guid storeID, Member storeOwner)
        {
            throw new Exception("The user unauthorised to remove store owner");
        }

        public virtual void ReactToBid(Guid storeID, Guid bid, string bidResponse)
        {
            throw new Exception("The user unauthorised to react to bid");
        }
        
        public virtual List<Bid> GetBidsInStore(Guid storeID)
        {
            throw new Exception("The user unauthorised to get bids");
        }
    }
}