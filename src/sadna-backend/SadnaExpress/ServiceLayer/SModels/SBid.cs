using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaExpress.DomainLayer.User;

namespace SadnaExpress.ServiceLayer.SModels
{
    public class SBid
    {
        private Guid bidID;
        public Guid BidID { get => bidID; set => bidID = value; }
        
        private Guid itemID;
        public Guid ItemID { get => itemID; set => itemID = value; }
        
        private string itemName;
        public string ItemName { get => itemName; set => itemName = value; }
        
        private string bidderEmail;
        public string BidderEmail { get => bidderEmail; set => bidderEmail = value; }

        private double offerPrice;
        public double OfferPrice { get => offerPrice; set => offerPrice = value; }
        
        private string[] approvers;
        public string[] Approvers { get => approvers; set => approvers = value; }

        private bool isActive; 
        public bool IsActive { get => isActive; set => isActive = value; }

        public SBid(Bid bid)
        {
            bidID = bid.BidId;
            itemID = bid.ItemID;
            ItemName = bid.ItemName;
            if (bid.User.GetType() != typeof(User))
                bidderEmail = ((Member)bid.User).Email;
            else
                bidderEmail = "guest";
            offerPrice = bid.Price;
            List<string> decisions = new List<string>();
            foreach (PromotedMember promotedMember in bid.Decisions.Keys)
            {
                double price;
                if (double.TryParse(bid.Decisions[promotedMember], out price))
                    decisions.Add($"{promotedMember.Email}: given counter offer of {price}");
                else
                    decisions.Add($"{promotedMember.Email}: {bid.Decisions[promotedMember]}");
            }
            approvers = decisions.ToArray();
            isActive = bid.Approved();
        }

    }
}
