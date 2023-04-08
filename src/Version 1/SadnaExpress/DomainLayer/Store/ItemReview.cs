using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaExpress.DomainLayer.Store
{
    public class ItemReview
    {
        private string username;
        private int storeId;
        private int rating;
        private string description;

        public ItemReview(string username, int storeId, int rating, string description)
        {
            this.username = username;
            this.storeId = storeId;
            this.rating = rating;
            this.description = description;
        }

        public string Username { get => username; set => username = value; }
        public int StoreId { get => storeId; set => storeId = value; }
        public int Rating { get => rating; set => rating = value; }
        public string Description { get => description; set => description = value; }

    }
}
