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
        private Guid itemID;
        public Guid ItemID { get => itemID; set => itemID = value; }
        
        private string itemName;
        public string ItemName { get => itemName; set => itemName = value; }
        
        private string bidderEmail;
        public string BidderEmail { get => bidderEmail; set => bidderEmail = value; }

        private double offerPrice;
        public double OfferPrice { get => offerPrice; set => offerPrice = value; }
        
        private string[] aprovers;
        public string[] Aprovers { get => aprovers; set => aprovers = value; }
        
        public SBid(Bid bid)
        {
            ItemName = bid.ItemName;
            if (bid.User.GetType() == typeof(Member))
                bidderEmail = ((Member)bid.User).Email;
            else
                bidderEmail = bid.User.UserId.ToString();
            offerPrice = bid.Price;
            List<string> decisions = new List<string>();
            foreach (PromotedMember promotedMember in bid.Decisions.Keys)
            {
                double price;
                if (double.TryParse(bid.Decisions[promotedMember], out price))
                    decisions.Add($"{promotedMember}: give counter offer of {price}");
                else
                    decisions.Add($"{promotedMember}: {bid.Decisions[promotedMember]}");
            }
            aprovers = decisions.ToArray();
        }

    }
}
