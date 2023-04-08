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
        private string storeName;
        private int itemId;
        private int rating;
        private string description;

        public ItemReview(string username, string storeName, int itemId, int rating, string description)
        {
            this.username = username;
            this.storeName = storeName;
            this.itemId = itemId;
            this.rating = rating;
            this.description = description;
        }

        public string Username { get => username; set => username = value; }
        public string StoreName { get => storeName; set => storeName = value; }

        public int ItemId { get => itemId; set => itemId = value; }
        public int Rating { get => rating; set => rating = value; }
        public string Description { get => description; set => description = value; }

    }
}
