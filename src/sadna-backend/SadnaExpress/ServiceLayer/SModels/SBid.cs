using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.ServiceLayer.SModels
{
    public class SBid
    {
        private string itemName;
        private string bidderEmail;
        private string[] aprovers;


        public SBid()
        {
        }

        public string ItemName { get => itemName; set => itemName = value; }
        public string BidderEmail { get => bidderEmail; set => bidderEmail = value; }
        public string[] Aprovers { get => aprovers; set => aprovers = value; }
    }
}
