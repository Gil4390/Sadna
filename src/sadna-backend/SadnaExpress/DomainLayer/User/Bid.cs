using System;
using System.Collections.Generic;

namespace SadnaExpress.DomainLayer.User
{
    public class Bid
    {
        private User user;
        public User User {get => user; set => user = value;}
        private Guid storeID;
        public Guid StoreID {get => storeID; set => storeID = value;}
        private Guid itemID;
        public Guid ItemID {get => itemID; set => itemID = value;}
        private string itemName;
        public string ItemName {get => itemName; set => itemName = value;}
        private double price;
        public double Price{get => price; set => price = value;}
        private bool openBid;
        public bool OpenBid{get => openBid; set => openBid = value;}
        private Dictionary<PromotedMember, string> decisions;
        public Dictionary<PromotedMember, string> Decisions {get => decisions; set => decisions = value;}

        public Bid(User user, Guid storeID, Guid itemID, string itemName, double price, List<PromotedMember> decisionBids)
        {
            openBid = true;
            this.user = user;
            this.storeID = storeID;
            this.ItemID = itemID;
            this.itemName = itemName;
            this.price = price;
            List<Member> toNodify = new List<Member>();
            foreach (PromotedMember promotedMember in decisionBids)
            {
                decisions.Add(promotedMember, "undecided");
                promotedMember.AddBid(this.storeID, this);
                toNodify.Add(promotedMember);
            }
            NotificationSystem.Instance.NotifyObservers(toNodify, storeID, $"You get offer to change the price to {this.itemName} to {price}", this.user.UserId);
        }

        public bool Approved()
        {
            foreach (string decision in decisions.Values)
            {
                if (!decision.Equals("approved"))
                    return false;
            }
            return true;
        }
        
        public void RemoveEmployee(PromotedMember promotedMember)
        {
            decisions.Remove(promotedMember);
            notify();
        }

        public void CloseBid()
        {
            user.Bids.Remove(this);
            foreach (PromotedMember emp in decisions.Keys)
                emp.RemoveBid(storeID, this);
        }
        
        public void ReactToBid(PromotedMember promotedMember, string bidResponse)
        {
            decisions[promotedMember] = bidResponse;
            if (bidResponse.Equals("approved"))
                notify();
            else if (bidResponse.Equals("denied"))
            {
                NotificationSystem.Instance.NotifyObserver(user, storeID, $"Your offer on {itemName} denied!");
                user.Bids.Remove(this);
                foreach (PromotedMember emp in decisions.Keys)
                    emp.RemoveBid(storeID, this);
            }
            else
            {
                NotificationSystem.Instance.NotifyObserver(user, storeID,
                    $"Your offer on {itemName} wasn't approved. You get counter offer of this amount {bidResponse}");
                double.TryParse(bidResponse, out price);
            }
            openBid = false;
        }

        private void notify()
        {
            if (Approved())
                NotificationSystem.Instance.NotifyObserver(user, storeID, $"Your offer on {itemName} accepted! The price changed to {price}");
        }

    }
}