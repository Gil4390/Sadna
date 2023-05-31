using Newtonsoft.Json;
using SadnaExpress.DataLayer;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SadnaExpress.DomainLayer.User
{
    public class Bid
    {
        private User user;
        [NotMapped]
        public User User {get => user; set => user = value;}

        [Key]
        public Guid BidId { get; set; }
        public Guid UserID { get; set; }
        
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
        [NotMapped]
        public Dictionary<PromotedMember, string> Decisions {get => decisions; set => decisions = value;}

        public Bid(User user, Guid storeID, Guid itemID, string itemName, double price, List<PromotedMember> decisionBids)
        {
            BidId = Guid.NewGuid();
            openBid = true;
            this.user = user;
            this.storeID = storeID;
            this.ItemID = itemID;
            this.itemName = itemName;
            this.price = price;
            List<Member> toNodify = new List<Member>();
            decisions = new Dictionary<PromotedMember, string>();
            foreach (PromotedMember promotedMember in decisionBids)
            {
                decisions.Add(promotedMember, "undecided");
                promotedMember.AddBid(this.storeID, this);
                toNodify.Add(promotedMember);
            }
            NotificationSystem.Instance.NotifyObservers(toNodify, storeID, $"You got an offer to change the price of {this.itemName} to {price}", this.user.UserId);
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
            if (user.GetType() != typeof(User))
                DBHandler.Instance.UpdateBidAndUser(this,user);
        }

        public void CloseBid()
        {
            user.Bids.Remove(this);
            foreach (PromotedMember emp in decisions.Keys)
                emp.RemoveBid(storeID, this);
            if (user.GetType() != typeof(User))
                DBHandler.Instance.RemoveBid(this);
        }
        
        public void ReactToBid(PromotedMember promotedMember, string bidResponse)
        {
            decisions[promotedMember] = bidResponse;
            if (bidResponse.Equals("approved"))
                notify();
            else if (bidResponse.Equals("denied"))
            {
                NotificationSystem.Instance.NotifyObserver(user, storeID, $"Your offer on {itemName} denied!");
                CloseBid();
                return;
            }
            else
            {
                decisions[promotedMember] = "approved";
                NotificationSystem.Instance.NotifyObserver(user, storeID,
                    $"Your offer on {itemName} wasn't approved. You get counter offer of this amount {bidResponse}");
                double.TryParse(bidResponse, out price);
            }
            openBid = false;

            if (user.GetType() != typeof(User))
                DBHandler.Instance.UpdateBidAndUser(this,user);
        }

        private void notify()
        {
            if (Approved())
                NotificationSystem.Instance.NotifyObserver(user, storeID, $"Your offer on {itemName} accepted! The price changed to {price}");
        }
        
        [NotMapped]
        public string DecisionJson
        {
            get
            {
                Dictionary<Guid, string> decisions = new Dictionary<Guid, string>();
                if (Decisions != null)
                {
                    foreach (PromotedMember pm in Decisions.Keys)
                        decisions.Add(pm.UserId, Decisions[pm]);
                }
                return JsonConvert.SerializeObject(decisions);
            }
            set
            {

            }
        }

        public string DecisionDB { get; set; }

        public void AddToDB()
        {
            DBHandler.Instance.UpdateBidAndUser(this, user);
            foreach (PromotedMember promotedMember in decisions.Keys)
            {
                DBHandler.Instance.UpdatePromotedMember(promotedMember);
            }
        }

        public Bid()
        {

        }
    }
}